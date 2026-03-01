using chat.net.Models;
using chat.net.Commands;
using chat.net.Conversations;
using chat.net.Configurations;

namespace Program;

/// <summary>
/// Provides an engine for running CLI commands to chat with AI in a scoped lifetime.
/// </summary>

public class Program {
    static async Task<int> Main(string[] args) {
        try {
            // validate the input command
            Command? command = CommandValidationService.ValidateCommands(args); 
            if(command == null)
                throw new InvalidOperationException("Unexpected error validating command.");
 
            // if null, new conversation
            var previousResponseId = ConfigurationService.GetValue(Configuration.ConfigurationAttributes.PreviousResponseId);
            
            // execute the command
            ResponseDto result = await CommandService.Execute(command, previousResponseId); 

            // push response to user
            if(result is AiResponseDto ai)
                ResponseService.PrintResult(ai);

        } catch (Exception exception) {
            Console.WriteLine($"Exception encountered: {exception}");
            return 1;
        }

        return 0;
    }
}
