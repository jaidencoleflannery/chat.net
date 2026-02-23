using static chat.net.Commands.CommandActions;

namespace chat.net.Commands;

public static class CommandValidationService {

    public static Command? ValidateCommands(string[] commands) {

        if(commands.Length <= 0)
            return null;

        int index = 0;
        foreach(var com in commands) {
            Console.WriteLine($"{index}: {com}");
            index++;
        }

        // this will be Input (normal chat) if the command is not found
        CommandAction? commandAction = GetCommandType(commands[0]);
        Command? command = null;

        // check if the provided command is one we support, and then call a subroutine for the subcommand
        // or, return the command if it's just the user's message
        switch (commandAction) {
            // if it is a command, return the command
            case CommandAction.Config:
                if(commands.Length < 3) {
                    Console.WriteLine("Argument and value expected for --config.");
                    return null;
                }
                command = ParseConfigCommand(commands[1], commands[2]);
                Console.WriteLine($"Command Found: {command}");
                break; 
            // if it is just text, return the text
            case CommandAction.Input:
                // process the input
                command = new Input(commands[0]);
                Console.WriteLine($"Command Found: {command}");
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

    private static Command? ParseConfigCommand(string command, string value) {
        var input = FormatCommand(command); 
        Console.WriteLine($"Formatted command: {input}");

        // see if the enum contains our input, if so, return the command type
        return Enum.TryParse<ConfigAction>(input, true, out var action)
            ? new Config(action, value)
            : null;
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
