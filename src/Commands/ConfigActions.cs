namespace chat.net.Commands;

public record Config(ConfigAction? Action = null, ConfigActionRequiresArgument? ActionArgument = null, string? Value = null) : Command; 

public enum ConfigAction {  
    ClearConfig,
}

public enum ConfigActionRequiresArgument {
    SetModel,
    SetProvider,
    SetKey
}
