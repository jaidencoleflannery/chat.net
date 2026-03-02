using System.Text;
using System.Text.Json;
using chat.net.Models;
using chat.net.Conversations;

namespace chat.net.Configurations;

public static class ConfigurationService {

    // if config exists, leave it alone, otherwise initialize config
    public static bool Init() {
        GetConfigPath(out var dir, out var path);
        Configuration config = GetConfig(dir, path);
        WriteConfig(config, dir, path);
        return true;
    } 

    public static string GetValue(Configuration.ConfigurationAttributes field, Providers? provider = null) {
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
        
        string response = "";
        Dictionary<Providers, string> dict;
        if(property.GetType() == typeof(Dictionary<Providers, string>)) { 
            if(provider == null)
                throw new ArgumentNullException(nameof(provider));

            var value = property.GetValue(config);
            if(value == null)
                throw new InvalidOperationException($"Could not get value of {nameof(property)} from {nameof(config)}.");
            dict = (Dictionary<Providers, string>)value; 
            if(dict == null)
                throw new InvalidOperationException($"Could not convert type of {nameof(value)} from {nameof(property)} to Dictionary.");
            response = (dict[provider.Value] ?? "") as string;
        } else {
            response = ((property.GetValue(config) ?? "") as string)!;
        }

        return response;
    }

    public static Configuration GetConfig(string dir, string path) { 
        Configuration? config;
        if(File.Exists(path)){
            // if this throws, let it bubble
            string json = File.ReadAllText(path);
            if(!string.IsNullOrWhiteSpace(json))
                config = JsonSerializer.Deserialize<Configuration>(json);
            else
                config = new Configuration();
        } else {
            config = new Configuration() { Path = path }; 
        }

        return config!;
    }

    public static bool SetValue(ConfigCommand command, Providers? provider = null) { 
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
        if(prop.PropertyType == typeof(Dictionary<Providers, string>)) {
            if(provider == null)
                throw new ArgumentNullException($"{nameof(provider)}");
            var dict = prop.GetValue(config) as Dictionary<Providers, string> ?? new Dictionary<Providers, string>();
            dict[provider.Value] = command.Value.Trim();
            prop.SetValue(config, dict);
        } else {
            prop.SetValue(config, command.Value.Trim()); 
        }

        config.Path = path;
        WriteConfig(config, dir, path);

        Console.WriteLine(command.Value.Trim());

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

    public static bool AppendToMessageHistory(string message, Providers provider) { 
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentNullException(nameof(message));

        if(!GetConfigPath(out var dir, out var path))
            throw new DirectoryNotFoundException("Could not find or create configuration path - Configuration file not written");

        var config = GetConfig(dir, path); 
        if(config == null)
            throw new InvalidOperationException($"Config could not be read or created.");

        config.MessageHistory ??= new Dictionary<Providers, List<String>>();
        if(!config.MessageHistory.ContainsKey(provider)) 
            config.MessageHistory[provider] = new List<string>();
        config.MessageHistory[provider].Add(message.Trim());

        WriteConfig(config, dir, path); 

        return true;
    }

    public static void ValidateProvider(out Providers? provider) { 
        provider = null;
        if(!GetConfigPath(out var dir, out var path))
            throw new DirectoryNotFoundException("Could not find configuration path - Configuration file not read.");

        var config = GetConfig(dir, path); 
        if(config == null)
            throw new InvalidOperationException("Config could not be read.");

        if(!Enum.TryParse<Providers>(config.Provider, true, out var foundProvider)) {
            StringBuilder builder = new();
            builder.AppendLine("| Provider is not set, run \"ask --config --set-provider <provider>\" to set your provider.");
            builder.AppendLine("| Valid providers include:");
            foreach(var currProvider in Enum.GetValues(typeof(Providers)))
                builder.AppendLine($"| > {currProvider.ToString()}");

            string errorString = builder.ToString();
            ResultResponseDto response = new(false, errorString);
            ResponseService.PrintResult(response);

            throw new InvalidOperationException(errorString);
        }
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
