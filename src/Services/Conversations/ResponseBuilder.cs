using System.Text;
using chat.net.Models;
using Microsoft.VisualBasic;

namespace chat.net.Conversations;

public class ResponseBuilder {
    const int ResponseWidth = 60;

    public static StringBuilder BuildChatResponse(StringBuilder builder) {
        var message = builder.ToString().Trim();
        StringBuilder response = new();
        response.AppendLine($"{message}");

        return response;
    }

    public static StringBuilder BuildException(Exception exception, bool debug) {
        StringBuilder response = new();
        // handle user error case
        if(exception is ProviderNotSetException providerNotSetException)
            response.AppendLine($"| {providerNotSetException.Message}"); 
        if(exception is InvalidProviderException invalidProviderException)
            response.AppendLine($"| {invalidProviderException.Message}"); 
        
        // print if in debug mode
        if(debug) {
            response.AppendLine("*-LOGGER-------------------------------------------------------*");
            var message = exception.Message;
            foreach(var line in message.Split("\n"))
                response.AppendLine($"| {line}");
            response.AppendLine("*--------------------------------------------------------------*");
        }
        return response;
    }
    public static StringBuilder BuildHelp() {
        StringBuilder response = new();
        response.AppendLine("*-HELP---------------------------------------------------------*");
        response.AppendLine("| Expected usage:\n> ask \"<text>\"\n> ask {command}");
        response.AppendLine("| Potential Commands:");
        var commandOptions = (CommandAction[])Enum.GetValues(typeof(CommandAction)); // getvalues returns the base class Array, so we have to cast to an actual array
        for(int command = 0; command < commandOptions.Length; command++) {
            if(commandOptions[command] != CommandAction.Input)
                if(commandOptions[command] != CommandAction.Config) {
                    response.Append($"| {commandOptions[command]}");
                } else {
                    response.Append($"| {commandOptions[command]} <argument>");
                    var configOptions = (ConfigAction[])Enum.GetValues(typeof(ConfigAction));
                    response.Append("| Potential Arguments (no input required):");
                    for(int argument = 0; argument < configOptions.Length; argument++) {
                        response.Append($"| {configOptions[argument]}");
                    }
                    response.Append("| Potential Arguments (input required):");
                    for(int argument = 0; argument < configOptions.Length; argument++) {
                        response.Append($"| {configOptions[argument]} <input>");
                    }
                }
        }
        response.AppendLine("*--------------------------------------------------------------*");
        return response;
    }
}