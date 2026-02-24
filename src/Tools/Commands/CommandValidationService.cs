namespace chat.net.Commands;

public static class CommandValidationService {

    /// <summary>
    /// Takes an array of strings and find's the matching Command.
    /// Will return null on failure.
    /// </summary>
    /// <param name="commands">An array of strings representing the command, should be the user's input.</param>
    /// <returns>A Command on success. Null on failure.</returns>
    
    public static Command? ValidateCommands(string[] commands) {
        if(commands.Length <= 0) {
            Console.WriteLine($"Arguments expected but no arguments were provided. \nUse -help for a list of commands.");
            return null;
        }

        // this will be Input (normal chat) if the command is not found
        CommandAction? commandType = GetCommandType(commands[0]); 

        // call routine for the type of command
        Command? command = null;
        switch (commandType) {
            // if it is a config, it needs atleast 1 argument
            case CommandAction.Config:
                if(commands.Length < 2) {
                    Console.WriteLine("Argument expected for --config.");
                    return null;
                }
                command = ParseConfigCommand(commands);
                break; 

            // if no command is found, we assume it is the message - return it as Input
            case CommandAction.Input:
                command = new Input(commands[0]);
                break;

            default:
                Console.WriteLine("Command found but not configured.");
                break;

            case null:
                Console.WriteLine("Unexpected error in command validation.");
                break;
        };
        
        // on failure, command == null
        return command;
    } 

    private static CommandAction? GetCommandType(string command) {
        var input = FormatCommand(command); 
        // if command is in commandactions, return that, else it is input for the bot
        return (Enum.TryParse<CommandAction>(input, true, out var action)) 
            ? action
            : CommandAction.Input;
    }

    private static Command? ParseConfigCommand(string[] commands) {
        /*
         * command[0] == --config
         * command[1] == {ConfigCommand}
         * command[2] == {Value}
        */

        var configCommand = FormatCommand(commands[1]); 

        // see if either enum contains our configuration command 
        // if enum requires no arguments
        if(Enum.TryParse<ConfigAction>(configCommand, true, out var action))
            return new Config(action);
        else
            Console.WriteLine($"Argument {commands[1]} for --config could not be found");

        // if enum requires an argument / input
        if(Enum.TryParse<ConfigActionRequiresArgument>(configCommand, true, out var actionWithArgument)) {
            if(commands.Length < 3) {
                Console.WriteLine($"--config {configCommand} expects an argument but no argument was provided.");
                return null;
            }
            switch (actionWithArgument) {
                // provider has to match Providers enum
                case ConfigActionRequiresArgument.SetProvider:
                    return (VerifyProvider(commands[2])) 
                        ? new Config(ActionArgument: actionWithArgument, Value: commands[2])
                        : null;

                // model can be anything (user's input is expected to match the endpoints expected value for model)
                case ConfigActionRequiresArgument.SetModel:
                    return new Config(ActionArgument: actionWithArgument, Value: commands[2]);

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
    private static string? FormatCommand(string command) {
        // if there is no command, do nothing
        if(string.IsNullOrWhiteSpace(command))
            return null;

        // convert the command to match our enum formatting
        return command.Replace("-", "").Trim();
    }
}
