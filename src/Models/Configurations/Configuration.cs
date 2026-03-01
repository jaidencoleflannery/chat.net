namespace chat.net.Models;

public class Configuration {
    // dictionaries are used to store values for each individual provider, so hotswapping is easier
    public string Path { get; set; } = String.Empty;
    public string Provider { get; set; } = String.Empty; 
    public string Instructions { get; set; } = String.Empty; // should Instructions be per provider? 

    // warning, code is tightly coupled to this type being Dictionary<Providers
    public Dictionary<Providers, string>? Model { get; set; }
    public Dictionary<Providers, string>? Key { get; set; }
    public Dictionary<Providers, string>? PreviousResponseId { get; set; }
    public Dictionary<Providers, List<string>>? MessageHistory { get; set; }


    public enum ConfigurationAttributes {
        Path,
        Model,
        Provider,
        Key,
        PreviousResponseId,
        Instructions,
        MessageHistory
    }
}
