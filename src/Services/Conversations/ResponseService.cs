using chat.net.Models;

namespace chat.net.Conversations;

public static class ResponseService {
    public static void Print(AiResponseDto result) {
        if(result == null) {
            Console.WriteLine("Result is null - cannot print.");
            return;
        }
        var properties = result.GetType().GetProperties();
        switch (result.Provider) {
            case Providers.Openai:
                Console.WriteLine(((OpenAiResponseDto)result).Output[0].Content[0].Text);
                break;

        }
    }
}
