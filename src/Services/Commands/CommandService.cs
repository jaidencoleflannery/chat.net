using chat.net.Conversations;
using chat.net.Configurations;
using chat.net.Models;

namespace chat.net.Commands;

public static class CommandService {

    static Dictionary<Type, Func<Command, string?, Task<ResponseDto>>> map = new() {
        [typeof(Config)] = (cmd, previousResponseId) => ExecuteConfigCommand((Config)cmd, previousResponseId),
        [typeof(Input)] = async (cmd, previousResponseId) => await ExecuteInputCommand((Input)cmd, previousResponseId),
    };

    public static async Task<ResponseDto> Execute(Command command, string? previousResponseId) {
        if(command == null) {
            Console.WriteLine("Execute failure, command is null.");
            Environment.Exit(1);
        }
        // map contains all of our functions, keyed by type
        return await map[command.GetType()](command, previousResponseId);
    }

    public static async Task<ResponseDto> ExecuteConfigCommand(Config command, string? previousResponseId) {
        if(command.Action != null) {
            switch(command.Action) { 
                case ConfigAction.ClearConfig:
                    return new ResponseDto(ConfigurationService.ClearConfig());
            }
        } else if(command.ActionArgument != null) {
            return new ResponseDto(ConfigurationService.SetValue(command));        
        }
        return new ResponseDto(false);
    }

    public static async Task<ResponseDto> ExecuteInputCommand(Input command, string? previousResponseId) =>
        await ConversationService.Call(command.Text, previousResponseId);
}
