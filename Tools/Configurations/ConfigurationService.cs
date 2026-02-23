using chat.net.Commands;

namespace chat.net.Configurations;

public static class ConfigurationService {
    public static void SetModel(Config command) {
        Console.WriteLine("SetModel() called");
        Console.WriteLine(command.Value);
    }

    public static void ClearConfig(Config command) {
        Console.WriteLine("ClearConfig() called");
        Console.WriteLine(command.Value);
    }
}
