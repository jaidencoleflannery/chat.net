using chat.net.Models;
using chat.net.Commands;
using chat.net.Conversations;
using chat.net.Configurations;

using static chat.net.Commands.CommandValidationService;
using static chat.net.Configurations.ConfigurationService;


namespace Program;

/// <summary>
/// Provides an engine wrapper for running CLI commands to chat with AI in a scoped lifetime.
/// </summary>

public class Program {
    static async Task<int> Main(string[] args) {
        try {
            if(args.Length < 1 || string.IsNullOrWhiteSpace(args[0]))
                throw new ArgumentException($"Arguments expected but no arguments were provided. \nUse -help for a list of commands.", nameof(args));
            // verify that the configuration file is good to go
            ConfigurationService.Init();

            // setup special flag for debugging (this is verified here so that other wrappers can have their own debug handler)
            string[] arguments;
            if(args[0] == "-debug")
                arguments = args[1..];
            else
                arguments = args;

            // validate user is either setting the provider, or config provider is valid (we do this here so the wrapper can share provider between calls)            
            // provider will never actually be null since ValidateProvider throws
            Providers? provider = null;
            if(args.Length <= 1 || !FormatCommand(args[0])!.Equals(CommandAction.Config.ToString(), StringComparison.OrdinalIgnoreCase) 
            || !FormatCommand(args[1])!.Equals(ConfigActionRequiresArgument.SetProvider.ToString(), StringComparison.OrdinalIgnoreCase)) 
                ValidateProvider(out provider);
            else
                ValidateProvider(out provider, args[2]);
 
            // validate the input command and grab the current provider
            Command? command = ValidateCommands(arguments);
            if(command == null)
                throw new InvalidOperationException("Unexpected error parsing command.");

            // if previousResponseId is null => new conversation (this is handled here so that other wrappers can cache the value)
            var previousResponseId = GetValue(Configuration.ConfigurationAttributes.PreviousResponseId, provider);

            // execute the command
            ResponseDto result = await CommandService.Execute(command, previousResponseId, provider); 

            // push response to user
            ResponseService.PrintResult(result);

        } catch (Exception exception) {  
            ResponseService.HandleException(exception, (args.Length > 0) ? args[0] == "-debug" : false);
        }

        return 0;
    }
}
