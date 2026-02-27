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
                var openaiOutput = openaiResult.Output;
                if(openaiOutput == null || openaiOutput.Length <= 0)
                    throw new InvalidOperationException($"OpenAi response did not match expected format - field {nameof(openaiOutput)} from {nameof(result)}");

                var openaiContent = openaiOutput[0].Content;
                if(openaiContent == null || openaiContent.Length <= 0)
                    throw new InvalidOperationException($"OpenAi response did not match expected format - field {nameof(openaiContent)} from {nameof(result)}");

                var openaiText = openaiContent[0].Text;
                if(string.IsNullOrWhiteSpace(openaiText))
                    throw new InvalidOperationException($"OpenAi response did not match expected format - field {nameof(openaiText)} from {nameof(result)}");
                    
                Console.WriteLine(openaiText);
                break;

            case Providers.Anthropic:
                if(result is AnthropicResponseDto anthropicResult) {
                    var anthropicContent = anthropicResult.Content;
                    if(anthropicContent == null || anthropicContent.Length <= 0)
                        throw new InvalidOperationException($"Anthropic response did not match expected format - field {nameof(anthropicContent)} from {nameof(result)}"); 

                    var anthropicText = anthropicContent[0].Text;
                    if(string.IsNullOrWhiteSpace(anthropicText))
                        throw new InvalidOperationException($"Anthropic response did not match expected format - field {nameof(anthropicText)} from {nameof(result)}");
                        
                    Console.WriteLine(anthropicText);
                    break;
                } else {
                    throw new InvalidOperationException("Failed to cast.");
                }

        }
    }
}
