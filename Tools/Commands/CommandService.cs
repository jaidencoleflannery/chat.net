using chat.net.Configurations;

namespace chat.net.Commands;

public static class CommandService {

    static Dictionary<Type, Action<Command>> map = new() {
        [typeof(Config)] = cmd => ExecuteConfigCommand((Config)cmd),
        [typeof(Input)] = cmd => ExecuteInputCommand((Input)cmd)
    };

    public static void Execute(Command command) {
        if(command == null)
            throw new ArgumentNullException(nameof(command));
        // map contains all of our functions by type
        map[command.GetType()](command);
    }

    public static void ExecuteConfigCommand(Config command) {
        Console.WriteLine("Config Command Ran");
        switch(command.Action) {
            case ConfigAction.SetModel:
                ConfigurationService.SetModel(command);
                break;
            case ConfigAction.ClearConfig:
                ConfigurationService.ClearConfig(command);
                break;
        }
    }

    public static void ExecuteInputCommand(Input command) {
        Console.WriteLine("Input Command Ran");
    }
}
