using chat.net.Conversations;
using chat.net.Configurations;

namespace chat.net.Commands;

public static class CommandService {

    static Dictionary<Type, Func<Command, Task>> map = new() {
        [typeof(Config)] = cmd => ExecuteConfigCommand((Config)cmd),
        [typeof(Input)] = async cmd => await ExecuteInputCommand((Input)cmd),
    };

    public static async Task Execute(Command command) {
        if(command == null) {
            Console.WriteLine("Execute failure, command is null.");
            Environment.Exit(1);
        }
        // map contains all of our functions, keyed by type
        await map[command.GetType()](command);
    }

    public static async Task ExecuteConfigCommand(Config command) {
        if(command.Action != null) {
            switch(command.Action) { 
                case ConfigAction.ClearConfig:
                    ConfigurationService.ClearConfig();
                    break;
            }
        } else if(command.ActionArgument != null) {
            ConfigurationService.SetValue(command);
        }
    }

    public static async Task ExecuteInputCommand(Input command) {
        await ConversationService.Call(command.Text);
    }
}
