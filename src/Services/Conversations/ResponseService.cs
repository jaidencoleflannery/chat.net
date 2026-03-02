using System.Text;
using static chat.net.Printers.Printer;
using static chat.net.Parsers.ResponseParser;
using chat.net.Models;

namespace chat.net.Conversations;

public static class ResponseService {
    public static void PrintResult(ResponseDto result) {
        if(result == null) 
            throw new ArgumentNullException(nameof(result));
        StringBuilder response = ParseResponse(result); 
        PrintToConsole(response);
    } 

    public static bool PrintHelp() {
        StringBuilder builder = new();
        builder.AppendLine("Expected usage:\nask \"<text>\"\nask {command}");
        builder.AppendLine("Potential Commands:");
        var commandOptions = (CommandAction[])Enum.GetValues(typeof(CommandAction)); // getvalues returns the base class Array, so we have to cast to an actual array
        for(int command = 0; command < commandOptions.Length; command++) {
            if(commandOptions[command] != CommandAction.Input)
                if(commandOptions[command] != CommandAction.Config) {
                    builder.Append($"| {commandOptions[command]}");
                } else {
                    builder.Append($"| {commandOptions[command]} <argument>");
                    var configOptions = (ConfigAction[])Enum.GetValues(typeof(ConfigAction));
                    var configOptionsArgumentRequired = (ConfigActionRequiresArgument[])Enum.GetValues(typeof(ConfigActionRequiresArgument));
                    builder.Append("| Potential Arguments (no input required):");
                    for(int argument = 0; argument < configOptions.Length; argument++) {
                        builder.Append($"| {configOptions[argument]}");
                    }
                    builder.Append("Potential Arguments (input required):");
                    for(int argument = 0; argument < configOptions.Length; argument++) {
                        builder.Append($"| {configOptions[argument]} <input>");
                    }
                }
        }
        return PrintToConsole(builder); 
    }
}
