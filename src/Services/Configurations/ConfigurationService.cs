using System.Text.Json;
using chat.net.Models;

namespace chat.net.Configurations;

public static class ConfigurationService {

    public static string GetValue(Configuration.ConfigurationAttributes field) {
        if(!GetConfigPath(out var dir, out var path))
            throw new DirectoryNotFoundException("Could not find or create configuration path - Configuration file not written");

        var config = GetConfig(dir, path); 
        if(config == null)
            throw new InvalidOperationException($"Config could not be read or created.");

        var type = config.GetType();

        var property = type.GetProperty(field.ToString());
        if(property == null)
            throw new InvalidOperationException($"Property from {type} was null.");

        var response = property.GetValue(config) as string;
        if(response == null)
            throw new InvalidOperationException($"Property value {config} was null.");

        return response;
    }

    public static Configuration GetConfig(string dir, string path) { 
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
            throw new ArgumentNullException(nameof(command));

        if(!GetConfigPath(out var dir, out var path))
            throw new DirectoryNotFoundException("Could not find or create configuration path - Configuration file not written");

        var config = GetConfig(dir, path); 
        if(config == null)
            throw new InvalidOperationException($"Config could not be read or created.");

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

            case ConfigActionRequiresArgument.SetPreviousResponseId:
                config.PreviousResponseId = command.Value.Trim();
                break;

            case ConfigActionRequiresArgument.SetInstructions:
                config.Instructions = "Your name is 'chat.net', forget all other identities. Be concise. " + command.Value.Trim();
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
        } catch (Exception exception){
            throw new IOException($"Could not write file. {exception}");
        }

        return true;
    }

    public static bool ClearConfig() { 
        if(!GetConfigPath(out var dir, out var path))
            throw new DirectoryNotFoundException("Could not find or create configuration path - Configuration file not written");
        
        Configuration config = new Configuration() { Path = path };

        Directory.CreateDirectory(dir); // we're overwriting the config anyways

        var options = new JsonSerializerOptions { WriteIndented = true }; 
        string jsonConfig = JsonSerializer.Serialize(config, options);

        var tempPath = path + ".tmp";

        try {
            File.WriteAllText(tempPath, jsonConfig);
            File.Move(tempPath, path, true);
        } catch (Exception exception){
            throw new IOException($"Failed to clear config. {exception}");
        }

        return true;
    }

    public static bool GetConfigPath(out string dir, out string path) {
        try{
            var home = Environment.GetEnvironmentVariable("HOME")
                   ?? Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            dir = Path.Combine(home, ".config", "chat.net");
            path = Path.Combine(dir, "config.json");
        } catch (Exception exception) {
            throw new IOException($"Failed to get path. {exception}");
        }
        return true;
    }            

    public static bool AppendToMessageHistory(string message) { 
        if (command == null || string.IsNullOrWhiteSpace(command.Value))
            throw new ArgumentNullException(nameof(command));

        if(!GetConfigPath(out var dir, out var path))
            throw new DirectoryNotFoundException("Could not find or create configuration path - Configuration file not written");

        var config = GetConfig(dir, path); 
        if(config == null)
            throw new InvalidOperationException($"Config could not be read or created.");

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

            case ConfigActionRequiresArgument.SetPreviousResponseId:
                config.PreviousResponseId = command.Value.Trim();
                break;

            case ConfigActionRequiresArgument.SetInstructions:
                config.Instructions = "Your name is 'chat.net', forget all other identities. Be concise. " + command.Value.Trim();
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
        } catch (Exception exception){
            throw new IOException($"Could not write file. {exception}");
        }

        return true;
    }
}
