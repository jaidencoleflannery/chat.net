using chat.net.Models;
using System.Text;

namespace chat.net.Conversations;

public static class ResponseService {
    public static void Print(AiResponseDto result) {
        if(result == null) 
            throw new ArgumentNullException(nameof(result));

        var properties = result.GetType().GetProperties();
        if(properties == null)
            throw new InvalidOperationException($"Could not get type or properties from {nameof(properties)}");

        var builder = new StringBuilder();

        switch (result.Provider) {
            case Providers.Openai:
                var openaiResult = ((OpenAiResponseDto)result);
                var openaiOutput = openaiResult.Output;
                if(openaiOutput == null || openaiOutput.Length <= 0)
                    throw new InvalidOperationException($"OpenAi response did not match expected format - field {nameof(openaiOutput)} from {nameof(result)}");
 
                for(int i = 0; i < openaiOutput.Length; i++) {
                    var contents = openaiOutput[i].Content;
                    if(contents == null || contents.Length <= 0)
                        continue;

                    for(int j = 0; j < contents.Length; j++) {
                        var content = contents[j];
                        if(!string.Equals(content.Type, "output_text", StringComparison.OrdinalIgnoreCase))
                            continue;
                        if(!string.IsNullOrWhiteSpace(content.Text)) {
                            if(builder.Length > 0) 
                                builder.AppendLine();
                            builder.Append(content.Text);
                        }
                    }
                }

                if(builder.Length <= 0)
                    throw new InvalidOperationException("Could not find any text within OpenAi response."); 
                PrintToConsole(builder); 
                break; 

            case Providers.Anthropic:
                var anthropicResult = ((AnthropicResponseDto)result);
                var anthropicContent = anthropicResult.Content;
                if(anthropicContent == null || anthropicContent.Length <= 0)
                    throw new InvalidOperationException($"Anthropic response did not match expected format - field {nameof(anthropicContent)} from {nameof(result)}");
 
                for(int i = 0; i < anthropicContent.Length; i++) {
                    var content = anthropicContent[i];
                    if(content == null)
                        continue;

                    if(!string.Equals(content.Type, "text", StringComparison.OrdinalIgnoreCase))
                        continue;
                    if(!string.IsNullOrWhiteSpace(content.Text)) {
                        if(builder.Length > 0) 
                            builder.AppendLine();
                        builder.Append(content.Text);
                    } 
                }

                if(builder.Length <= 0)
                    throw new InvalidOperationException("Could not find any text within Anthropic response."); 
                PrintToConsole(builder);
                break;

        }
    }

    private static void PrintToConsole(StringBuilder builder) {
        Console.WriteLine($"\n\n[chat.net]:");
        Console.WriteLine(builder.ToString());
        Console.WriteLine($"\n");
    }
}
