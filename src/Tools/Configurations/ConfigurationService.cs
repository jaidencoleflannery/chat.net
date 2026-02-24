using System.Text.Json;
using chat.net.Commands;

namespace chat.net.Configurations;

public static class ConfigurationService {

    public static string? GetProvider() {
        if(!GetConfigPath(out var dir, out var path)) {
            Console.WriteLine("Could not create configuration path - Configuration file not written.");
            return null;
        }

        Configuration? config;

        if(File.Exists(path)){
            try {
                string json = File.ReadAllText(path);
                config = JsonSerializer.Deserialize<Configuration>(json)
                    ?? new Configuration() { Path = path };
            } catch (Exception) {
                config = new Configuration() { Path = path };
            }
        } else {
            config = new Configuration() { Path = path }; 
        }

        return config.Provider;
    }

    public static Configuration? GetConfig() {
        if(!GetConfigPath(out var dir, out var path)) {
            Console.WriteLine("Could not create configuration path - Configuration file not written.");
            return null;
        }

        Configuration? config;

        if(File.Exists(path)){
            try {
                string json = File.ReadAllText(path);
                config = JsonSerializer.Deserialize<Configuration>(json)
                    ?? new Configuration() { Path = path };
            } catch (Exception) {
                config = new Configuration() { Path = path };
            }
        } else {
            config = new Configuration() { Path = path }; 
        }

        return config;
    }

    public static void SetValue(Config command) { 
        if (command == null || string.IsNullOrWhiteSpace(command.Value))
            return;

        if(!GetConfigPath(out var dir, out var path)) {
            Console.WriteLine("Could not create configuration path - Configuration file not written.");
            return;
        }

        Configuration? config;

        if(File.Exists(path)){
            try {
                string json = File.ReadAllText(path);
                config = JsonSerializer.Deserialize<Configuration>(json)
                    ?? new Configuration();
            } catch (Exception) {
                config = new Configuration();
            }
        } else {
            config = new Configuration(); 
        }

        switch (command.ActionArgument) {
            case ConfigActionRequiresArgument.SetProvider:
                config.Provider = command.Value.Trim();
                break;

            case ConfigActionRequiresArgument.SetModel:
                config.Model = command.Value.Trim();
                break;
        }

        config.Path = path;

        Directory.CreateDirectory(dir); // we're overwriting the config anyways

        var options = new JsonSerializerOptions { WriteIndented = true }; 
        string jsonConfig = JsonSerializer.Serialize(config, options);

        var tempPath = path + ".tmp";

        File.WriteAllText(tempPath, jsonConfig);
        File.Move(tempPath, path, true);
        return;
    }

    public static void ClearConfig() { 
        if(!GetConfigPath(out var dir, out var path)) {
            Console.WriteLine("Could not create configuration path - Configuration file not written.");
            return;
        }
        
        Configuration config = new Configuration() { Path = path };

        Directory.CreateDirectory(dir); // we're overwriting the config anyways

        var options = new JsonSerializerOptions { WriteIndented = true }; 
        string jsonConfig = JsonSerializer.Serialize(config, options);

        var tempPath = path + ".tmp";

        File.WriteAllText(tempPath, jsonConfig);
        File.Move(tempPath, path, true);
        return;
    }

    public static bool GetConfigPath(out string dir, out string path) {
        try{
            var home = Environment.GetEnvironmentVariable("HOME")
                   ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            dir = Path.Combine(home, ".config", "chat.net");
            path = Path.Combine(dir, "config.json");
        } catch (Exception) {
            dir = string.Empty;
            path = string.Empty;
            return false;
        }
        return true;
    }
}
