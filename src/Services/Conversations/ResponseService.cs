using chat.net.Models;

namespace chat.net.Conversations;

public static class ResponseService {
    public static void Print(AiResponseDto result) {
        if(result == null) 
            throw new ArgumentNullException(nameof(result));

        var properties = result.GetType().GetProperties();
        if(properties == null)
            throw new InvalidOperationException($"Could not get type or properties from {nameof(properties)}");
        switch (result.Provider) {
            case Providers.Openai:
                var openaiResult = ((OpenAiResponseDto)result);
                var output = openaiResult.Output;
                if(output == null || output.Length <= 0)
                    throw new InvalidOperationException($"OpenAi response did not match expected format - field {nameof(output)} from {nameof(result)}");

                var content = openaiResult.Output![0].Content;
                if(content == null || content.Length <= 0)
                    throw new InvalidOperationException($"OpenAi response did not match expected format - field {nameof(content)} from {nameof(result)}");

                var text = content[0].Text;
                if(string.IsNullOrWhiteSpace(text))
                    throw new InvalidOperationException($"OpenAi response did not match expected format - field {nameof(text)} from {nameof(result)}");
                    
                Console.WriteLine(text);
                break;

        }
    }
}
