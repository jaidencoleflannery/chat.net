using chat.net.Conversations;
using chat.net.Configurations;
using chat.net.Models;

using static chat.net.Models.ConfigActionRequiresArgument;
using static chat.net.Models.ConfigAction;

namespace chat.net.Commands;

public static class CommandService {

    static Dictionary<Type, Func<Command, string?, Task<ResponseDto>>> map = new() {
        [typeof(ConfigCommand)] = (cmd, previousResponseId) => ExecuteConfigCommand((ConfigCommand)cmd, previousResponseId),
        [typeof(InputCommand)] = async (cmd, previousResponseId) => await ExecuteInputCommand((InputCommand)cmd, previousResponseId),
        [typeof(ClearCommand)] = (cmd, previousResponseId) => ExecuteClearCommand((ClearCommand)cmd, previousResponseId),
        [typeof(HelpCommand)] = (cmd, previousResponseId) => ExecuteHelpCommand((HelpCommand)cmd, previousResponseId),
    };

    public static async Task<ResponseDto> Execute(Command command, string? previousResponseId) {
        if(command == null)
            throw new ArgumentNullException(nameof(command));
        // Map contains all of our functions, keyed by type
        return await map[command.GetType()](command, previousResponseId);
    }

    public static async Task<ResponseDto> ExecuteConfigCommand(ConfigCommand command, string? previousResponseId) {
        // Command can XOR contain: Action (no argument(s)) or ActionArgument (with argument(s))
        if(command.Action != null)
            switch(command.Action) { 
                case ClearConfig:
                    return new ResponseDto(ConfigurationService.ClearConfig());
                default:
                    throw new ArgumentException($"Configuration action {command.Action} not recognized.", nameof(command.Action));
            }
        else if(command.ActionArgument != null)
            switch(command.ActionArgument) {
                case SetModel:
                case SetProvider: 
                case SetKey:
                case SetInstructions:
                    return new ResponseDto(ConfigurationService.SetValue(command));
                default:
                    throw new ArgumentException($"Configuration action with argument {command.ActionArgument} not recognized.", nameof(command.ActionArgument));
            }
        else
            throw new ArgumentNullException($"{nameof(command.Action)} and {nameof(command.ActionArgument)}");
        }

    public static async Task<ResponseDto> ExecuteInputCommand(InputCommand command, string? previousResponseId) =>
        await ConversationService.Call(command.Text, previousResponseId);

    public static Task<ResponseDto> ExecuteClearCommand(Command command, string? previousResponseId) {
        ConfigCommand? configUpdate = new ConfigCommand() { ActionArgument = SetPreviousResponseId, Value = "empty" };
        return Task.FromResult(new ResponseDto(ConfigurationService.SetValue(configUpdate)));
    }

    public static Task<ResponseDto> ExecuteHelpCommand(Command command, string? previousResponseId) {
        ConfigCommand? configUpdate = new ConfigCommand() { ActionArgument = SetPreviousResponseId, Value = "empty" };
        return Task.FromResult(new ResponseDto(ConfigurationService.SetValue(configUpdate)));
    }
}
