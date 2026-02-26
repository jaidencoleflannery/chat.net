using System.Text.Json;
using chat.net.Models;

namespace chat.net.Configurations;

public static class ConfigurationService {

    public static string GetValue(Configuration.ConfigurationAttributes field) {
        if(!GetConfigPath(out var dir, out var path)) {
            Console.WriteLine("Could not create configuration path - Configuration file not written.");
            Environment.Exit(1);
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

        var type = config.GetType();
        var property = type.GetProperty(field.ToString());

        if(property == null) {
            Console.WriteLine("Property not found.");
            Environment.Exit(1);
        }

        var response = property.GetValue(config) as string;
        return response;
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

    public static bool SetValue(Config command) { 
        if (command == null || string.IsNullOrWhiteSpace(command.Value))
            return false;

        if(!GetConfigPath(out var dir, out var path)) {
            Console.WriteLine("Could not create configuration path - Configuration file not written.");
            return false;
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

            case ConfigActionRequiresArgument.SetKey:
                config.Key = command.Value.Trim();
                break;
        }

        config.Path = path;

        Directory.CreateDirectory(dir); // we're overwriting the config anyways

        var options = new JsonSerializerOptions { WriteIndented = true }; 
        string jsonConfig = JsonSerializer.Serialize(config, options);

        var tempPath = path + ".tmp";

        try {
            File.WriteAllText(tempPath, jsonConfig);
            File.Move(tempPath, path, true);
        } catch {
            Console.WriteLine("Failed to write config.");
            return false;
        }

        return true;
    }

    public static bool ClearConfig() { 
        if(!GetConfigPath(out var dir, out var path)) {
            Console.WriteLine("Could not create configuration path - Configuration file not written.");
            return false;
        }
        
        Configuration config = new Configuration() { Path = path };

        Directory.CreateDirectory(dir); // we're overwriting the config anyways

        var options = new JsonSerializerOptions { WriteIndented = true }; 
        string jsonConfig = JsonSerializer.Serialize(config, options);

        var tempPath = path + ".tmp";

        try {
            File.WriteAllText(tempPath, jsonConfig);
            File.Move(tempPath, path, true);
        } catch {
            Console.WriteLine("Failed to clear config.");
            return false;
        }

        return true;
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
