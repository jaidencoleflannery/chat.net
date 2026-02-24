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
        return (Enum.TryParse<CommandAction>(input, true, out var action)) 
            ? action
            : CommandAction.Input;    
    }

    private static Command? ParseConfigCommand(string[] commands) {
        var input = FormatCommand(commands[1]); 

        // see if the enum contains our input
        // if enum requires no arguments
        if(Enum.TryParse<ConfigAction>(input, true, out var action))
            return new Config(action); 
        else
            Console.WriteLine($"Argument {commands[1]} for --config could not be found");

        // if enum requires arguments
        if(Enum.TryParse<ConfigActionRequiresArgument>(input, true, out var actionWithArgument)) {
            if(commands.Length < 3) {
                Console.WriteLine($"--config {input} expects an argument but no argument was provided.");
                return null;
            }
            switch (actionWithArgument) {
                case ConfigActionRequiresArgument.SetProvider:
                    return (VerifyProvider(commands[2])) 
                        ? new Config(action, Value: commands[2])
                        : null;

                case ConfigActionRequiresArgument.SetModel:
                    return new Config(action, Value: commands[2]);

                default:
                    return null;
            }
        }
        return null;
    }

    // check if action matches one of our providers
    private static bool VerifyProvider(string argument) {
        if(!Enum.TryParse<Providers>(argument.ToString(), true, out var result)) {
            Console.WriteLine("Invalid provider argument, valid providers include:");
            foreach(var provider in Enum.GetValues(typeof(Providers)))
                Console.WriteLine(provider); 
            return false;
        }
        return true;
    }

    // convert string into generic format
    public static string? FormatCommand(string command) {
        // if there is no command, do nothing
        if(string.IsNullOrWhiteSpace(command))
            return null;

        // convert the command to match our enum formatting
        return command.Replace("-", "").Trim();
    }
}
