using chat.net.Models;
using chat.net.Commands;

namespace Program;

public class Program {
    static async Task<int> Main(string[] args) {
        // validate the input command
        Command? command = CommandValidationService.ValidateCommands(args); 
        if(command == null)
            return 1;

        await CommandService.Execute(command);

        return 0;
    }
}
