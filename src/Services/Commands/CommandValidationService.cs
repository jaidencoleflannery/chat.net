using System.Text;
using chat.net.Models;

namespace chat.net.Commands;

public static class CommandValidationService {

    /// <summary>
    /// Provides a service for converting an array of strings into a Command object.
    /// </summary>
    /// <param name="commands">An array of strings representing the command, typically the user's input.</param>
    /// <returns>A Command type object that represents the input.</returns>
    
    public static Command? ValidateCommands(string[] commands) {
        if(commands.Length <= 0 || string.IsNullOrWhiteSpace(commands[0]))
            throw new ArgumentException($"Arguments expected but no arguments were provided. \nUse -help for a list of commands.", nameof(commands));

        // this will be Input (normal chat) if the command is not found
        CommandAction commandType = GetCommandType(commands[0]); 

        // call routine for the type of command
        Command? command = null;
        switch (commandType) {
            // if it is a config, it needs atleast 1 argument
            case CommandAction.Config:
                if(commands.Length < 2)
                    throw new ArgumentException("Argument expected for --config.");
                command = ParseConfigCommand(commands);
                break; 

            // if no command is found, we assume it is the message - return it as Input
            case CommandAction.Input:
                command = new Input(commands[0]);
                break;

            default:
                throw new ArgumentException("Command found but not configured.");

        };
        
        return command;
    } 

    private static CommandAction GetCommandType(string command) {
        if(string.IsNullOrWhiteSpace(command))
            throw new ArgumentException($"Argument type could not be found, argument is null or empty.", nameof(command));
        
        var input = FormatCommand(command); 
        // if command is in commandactions, return that, else it is input for the bot
        return (Enum.TryParse<CommandAction>(input, true, out var action)) 
            ? action
            : CommandAction.Input;
    }

    private static Command ParseConfigCommand(string[] commands) {
        /*
         * command[0] == --config
         * command[1] == {ConfigCommand}
         * command[2] == {Value}
        */

        var configCommand = FormatCommand(commands[1]); 

        // see if either enum contains our configuration command:

        // if(enum requires no arguments) else if(it does require arguments) else throw
        if(Enum.TryParse<ConfigAction>(configCommand, true, out var action)) {
            return new Config(action);
        } else if(Enum.TryParse<ConfigActionRequiresArgument>(configCommand, true, out var actionWithArgument)) {
            if(commands.Length < 3)
                throw new ArgumentException($"--config {configCommand} expects an argument but no argument was provided.", nameof(commands));
            if(string.IsNullOrWhiteSpace(commands[2]))
                throw new ArgumentException("Provided argument is null or empty", nameof(commands));
             
            var arg = commands[2]; 
            switch (actionWithArgument) {
                // provider has to match a value from Providers enum 
                case ConfigActionRequiresArgument.SetProvider:
                    // make sure provided Provider type exists in our enum of valid providers
                    if(Enum.TryParse<Providers>(arg, true, out var result)) {
                        return new Config(ActionArgument: actionWithArgument, Value: arg);
                    } else {
                        // build an error response that contains valid input options
                        StringBuilder builder = new();
                        builder.Append($"Invalid provider. Valid providers include:");
                        foreach(var provider in Enum.GetValues(typeof(Providers))) 
                            builder.Append(provider);
                        throw new ArgumentException(builder.ToString(), nameof(commands));
                    }

                case ConfigActionRequiresArgument.SetKey:
                    return new Config(ActionArgument: actionWithArgument, Value: arg); 

                // model can be anything (user's input is expected to match the endpoints expected value for model)
                case ConfigActionRequiresArgument.SetModel:
                    return new Config(ActionArgument: actionWithArgument, Value: arg);

                default:
                    throw new ArgumentException("Config argument not recognized.", nameof(configCommand));
            }
        }
        throw new Exception("Unexpected error encountered while parsing configuration command.");
    }

    // convert string into generic format
    private static string? FormatCommand(string command) {
        if(string.IsNullOrWhiteSpace(command))
            throw new ArgumentException($"Argument {command} could not be formatted, argument is null or empty.", nameof(command));
        // convert the command to match our enum formatting
        return command.Replace("-", "").Trim();
    }
}
