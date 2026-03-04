using chat.net.Conversations;
using chat.net.Configurations;
using chat.net.Models;

using static chat.net.Conversations.ResponseBuilder;
using static chat.net.Models.ConfigActionRequiresArgument;
using static chat.net.Models.ConfigAction;

namespace chat.net.Commands;

public static class CommandService {

    static Dictionary<Type, Func<Command, string?, Providers?, Task<ResponseDto>>> map = new() {
        [typeof(ConfigCommand)] = (cmd, previousResponseId, provider) => ExecuteConfigCommand((ConfigCommand)cmd, previousResponseId, provider),
        [typeof(InputCommand)] = async (cmd, previousResponseId, provider) => await ExecuteInputCommand((InputCommand)cmd, previousResponseId, provider),
        [typeof(ClearCommand)] = (cmd, previousResponseId, provider) => ExecuteClearCommand((ClearCommand)cmd, previousResponseId, provider),
        [typeof(HelpCommand)] = (cmd, previousResponseId, provider) => ExecuteHelpCommand((HelpCommand)cmd, previousResponseId, provider),
    };

    // Map contains all of our functions, keyed by type
    public static async Task<ResponseDto> Execute(Command command, string? previousResponseId, Providers? provider) =>
        (command == null)
        ? throw new ArgumentNullException(nameof(command))
        : await map[command.GetType()](command, previousResponseId, provider);

    public static async Task<ResponseDto> ExecuteConfigCommand(ConfigCommand command, string? previousResponseId, Providers? provider) {
        // Command can XOR contain: Action (no argument(s)) or ActionArgument (with argument(s))
        if(command.Action != null)
            switch(command.Action) { 
                case ClearConfig:
                    ConfigurationService.ClearConfig();
                    return new ResultResponseDto(true, "Successfully cleared configuration.");
                default:
                    throw new ArgumentException($"Configuration action {command.Action} not recognized.", nameof(command.Action));
            }
        else if(command.ActionArgument != null)
            switch(command.ActionArgument) {
                case SetModel:
                case SetProvider: 
                case SetKey:
                case SetInstructions:
                    ConfigurationService.SetValue(command, provider);
                    return new ResultResponseDto(true, "Successfully updated configuration.");
                default:
                    throw new ArgumentException($"Configuration action with argument {command.ActionArgument} not recognized.", nameof(command.ActionArgument));
            }
        else
            throw new ArgumentNullException($"{nameof(command.Action)} and {nameof(command.ActionArgument)}");
        }

    public static async Task<ResponseDto> ExecuteInputCommand(InputCommand command, string? previousResponseId, Providers? provider) =>
        await ConversationService.Call(command.Text, previousResponseId, provider);

    public static Task<ResponseDto> ExecuteClearCommand(Command command, string? previousResponseId, Providers? provider) {
        ConfigurationService.SetValue(
            new ConfigCommand() {
                ActionArgument = SetPreviousResponseId, 
                Value = string.Empty
            }, 
            provider
        );
        return Task.FromResult((ResponseDto)new ResultResponseDto(true, "Conversation cleared."));
    }

    public static Task<ResponseDto> ExecuteHelpCommand(Command command, string? previousResponseId, Providers? provider) {
        return Task.FromResult((ResponseDto)new ResultResponseDto(true, BuildHelp().ToString()));
    }
}
