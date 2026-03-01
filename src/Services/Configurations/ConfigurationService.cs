using System.Text.Json;
using chat.net.Models;

namespace chat.net.Configurations;

public static class ConfigurationService {

    public static string GetValue(Configuration.ConfigurationAttributes field) {
        if(!GetConfigPath(out var dir, out var path))
            throw new DirectoryNotFoundException("Could not find or create configuration path - Configuration file not written");
        
        // this is enefficient, but our configuration is relatively tiny
        // if the configuration grows, replace this with a more efficient implementation
        var config = GetConfig(dir, path); 
        if(config == null)
            throw new InvalidOperationException($"Config could not be read or created.");

        var type = config.GetType(); // will only throw if config is null

        var property = type.GetProperty(field.ToString());
        if(property == null)
            throw new InvalidOperationException($"Property {field} from {type} was null.");

        var response = property.GetValue(config) as string;
        if(response == null)
            throw new InvalidOperationException($"Property value {config} was null.");

        return response;
    }

    public static Configuration GetConfig(string dir, string path) { 
        Configuration? config;
        if(File.Exists(path)){
            // if this throws, let it bubble
            string json = File.ReadAllText(path);
            config = JsonSerializer.Deserialize<Configuration>(json)
                ?? new Configuration() { Path = path };
        } else {
            config = new Configuration() { Path = path }; 
        }

        return config;
    }

    public static bool SetValue(ConfigCommand command) { 
        if (command == null || string.IsNullOrWhiteSpace(command.Value) || command.ActionArgument == null)
            throw new ArgumentException($"{nameof(command)} invalid.", nameof(command));

        if(!GetConfigPath(out var dir, out var path))
            throw new DirectoryNotFoundException("Could not find or create configuration path - Configuration file not written");

        var config = GetConfig(dir, path); 
        if(config == null)
            throw new InvalidOperationException($"Config could not be read or created.");

        // match actionargument to a configuration attribute and then push the command.Value onto that field
        var actionArgument = command.ActionArgument.Value.ToString();
        actionArgument = actionArgument.AsSpan(3).ToString(); // strip the "Set" from the beginning of the string so it matches the config attribute name
        var type = config.GetType();
        var prop = type.GetProperty(actionArgument);
        if(prop == null)
            throw new InvalidOperationException($"Failed to reflect {nameof(command)} and edit config.");
        prop.SetValue(config, command.Value.Trim()); 

        config.Path = path;
        WriteConfig(config, dir, path);

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
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentNullException(nameof(message));

        if(!GetConfigPath(out var dir, out var path))
            throw new DirectoryNotFoundException("Could not find or create configuration path - Configuration file not written");

        var config = GetConfig(dir, path); 
        if(config == null)
            throw new InvalidOperationException($"Config could not be read or created.");

        config.MessageHistory.Add(message); 
        WriteConfig(config, dir, path); 

        return true;
    }

    public static void WriteConfig(Configuration config, string dir, string path) {
        config.Path = path;
        Directory.CreateDirectory(dir); // no-op if dir exists

        var options = new JsonSerializerOptions { WriteIndented = true }; 
        string jsonConfig = JsonSerializer.Serialize(config, options);

        var tempPath = path + ".tmp";
        
        // atomic write
        try {
            File.WriteAllText(tempPath, jsonConfig);
            File.Move(tempPath, path, true);
        } catch (Exception exception){
            throw new IOException("Could not write file.", exception);
        }
    }
}
