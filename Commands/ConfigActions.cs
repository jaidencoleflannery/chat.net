namespace chat.net.Commands;

public record Config(ConfigAction Action, string Value) : Command; 

public enum ConfigAction { 
    SetModel,
    ClearConfig,
}
