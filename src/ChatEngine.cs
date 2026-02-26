using chat.net.Models;
using chat.net.Commands;
using chat.net.Conversations;

namespace Program;

public class Program {
    static async Task<int> Main(string[] args) {
        // validate the input command
        Command? command = CommandValidationService.ValidateCommands(args); 
        if(command == null)
            return 1;

        // grab the last ResponseId (this is handled here so we can rely on caching in other implementations)
        string? previousResponseId = null;
        
        // execute the command
        ResponseDto result = await CommandService.Execute(command, previousResponseId);

        // push response to user
        if(result is AiResponseDto ai)
            ResponseService.Print(ai);

        return 0;
    }
}
