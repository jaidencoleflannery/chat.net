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
            // verify that the configuration file is good to go
            ConfigurationService.Init();

            // setup special flag for debugging
            string[] arguments;
            if(args[0] == "-debug")
                arguments = args[1..];
            else
                arguments = args;

            Providers? provider = null; 
            if(!(arguments.Length > 1) || CommandValidationService.FormatCommand(arguments[0]) != CommandAction.Config.ToString().ToLower() 
            || CommandValidationService.FormatCommand(arguments[1]) != ConfigActionRequiresArgument.SetProvider.ToString().ToLower()) 
                ConfigurationService.ValidateProvider(out provider);

            // validate the input command
            Command? command = CommandValidationService.ValidateCommands(arguments); 
            if(command == null)
                throw new InvalidOperationException("Unexpected error validating command."); 

            // if null, new conversation 
            var previousResponseId = ConfigurationService.GetValue(Configuration.ConfigurationAttributes.PreviousResponseId, provider);

            // execute the command
            ResponseDto result = await CommandService.Execute(command, previousResponseId, provider); 

            // push response to user
            ResponseService.PrintResult(result);

        } catch (Exception exception) {
            if(exception is ProviderNotSetException providerNotSetException)
                Console.WriteLine("Provider has to be set before this command can be executed."); 
            if(exception is InvalidProviderException invalidProviderException)
                ResponseService.PrintResult(new ResultResponseDto(false, invalidProviderException.Message));
            if(args[0] == "-debug")
                Console.WriteLine(exception);
            return 1;
        }

        return 0;
    }
}
