using static chat.net.Commands.CommandActions;

namespace chat.net.Commands;

public static class CommandValidationService {

    public static Command? ValidateCommands(string[] commands) {

        if(commands.Length <= 0)
            return null;

        // this will be Input (normal chat) if the command is not found
        CommandAction? commandAction = GetCommandType(commands[0]);
        Command? command = null;

        // check if the provided command is one we support, and then call a subroutine for the subcommand
        // or, return the command if it's just the user's message
        switch (commandAction) {
            // if it is a command, return the command
            case CommandAction.Config:
                if(commands.Length < 2) {
                    Console.WriteLine("Argument expected for --config.");
                    return null;
                }
                command = ParseConfigCommand(commands);
                break; 
            // if it is just text, return the text
            case CommandAction.Input:
                // process the input
                command = new Input(commands[0]);
                break;
            case null:
                throw new Exception("Unexpected error in command validation.");
        };
 
        return command;
    } 

    public static CommandAction? GetCommandType(string command) {
        var input = FormatCommand(command);
        
        // if command is in commandactions, return that, else it is input for the bot
        return (Enum.TryParse<CommandActions.CommandAction>(input, true, out var action)) 
            ? action
            : CommandActions.CommandAction.Input;    
    }

    private static Command? ParseConfigCommand(string[] commands) {
        var input = FormatCommand(commands[1]); 

        // see if the enum contains our input, if so, return the command type
        Console.WriteLine($"!!{input}");
        if(Enum.TryParse<ConfigAction>(input, true, out var action))
            return new Config(action, (commands.Length > 2) ? commands[2] : "");
        else {
            Console.WriteLine(action);
            Console.WriteLine($"Argument {commands[1]} for --config could not be found");
            return null;
        }
    }

    // convert string into generic format
    public static string? FormatCommand(string command) {
        // if there is no command, do nothing
        if(string.IsNullOrWhiteSpace(command))
            return null;

        // convert the command to match our enum formatting
        string input = command.Replace("-", "").Trim();

        return input;
    }
}
