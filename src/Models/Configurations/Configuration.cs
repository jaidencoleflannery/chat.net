namespace chat.net.Models;

public class Configuration {
    public string Path { get; set; } = String.Empty;
    public string Model { get; set; } = String.Empty;
    public string Provider { get; set; } = String.Empty;
    public string Key { get; set; } = String.Empty;
    public string PreviousResponseId { get; set; } = String.Empty;
    public string Instructions { get; set; } = String.Empty;

    public enum ConfigurationAttributes {
        Path,
        Model,
        Provider,
        Key,
        PreviousResponseId,
        Instructions
    }
}
