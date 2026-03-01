using System.Text;
using chat.net.Models;
using static chat.net.Models.Providers;

namespace chat.net.Parsers;

public static class ResponseParser {
    public static StringBuilder ParseResponse(AiResponseDto result) {
        StringBuilder builder = new StringBuilder();
        switch(result.Provider) {
            case Openai:
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
                return builder;
 
            case Anthropic:
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
                return builder;

            case Google:
            case Xai:
            case Deepseek: 
            default:
                throw new ArgumentException($"No configured provider was found within {nameof(result)}");
        }
    }
}
