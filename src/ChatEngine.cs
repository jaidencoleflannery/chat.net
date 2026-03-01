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
            if(args[0] == "-dev")
                arguments = args[1..];
            else
                arguments = args;

            // validate the input command
            Command? command = CommandValidationService.ValidateCommands(arguments); 
            if(command == null)
                throw new InvalidOperationException("Unexpected error validating command.");

            var providerString = ConfigurationService.GetValue(Configuration.ConfigurationAttributes.Provider, null);
            if(!Enum.TryParse<Providers>(providerString, true, out var provider))
                throw new InvalidOperationException($"Provider ({providerString}) was pulled from config but did not match configured providers.");
 
            // if null, new conversation 
            var previousResponseId = ConfigurationService.GetValue(Configuration.ConfigurationAttributes.PreviousResponseId, provider);
            
            // execute the command
            ResponseDto result = await CommandService.Execute(command, previousResponseId, provider); 

            // push response to user
            ResponseService.PrintResult(result);

        } catch (Exception exception) {
            if(args[0] == "-dev")
                Console.WriteLine(exception);
            return 1;
        }

        return 0;
    }
}
