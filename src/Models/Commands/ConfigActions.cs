namespace chat.net.Models;

public record Config(ConfigAction? Action = null, ConfigActionRequiresArgument? ActionArgument = null, string Value = "") : Command; 

public enum ConfigAction {  
    ClearConfig,
}

public enum ConfigActionRequiresArgument {
    SetModel,
    SetProvider,
    SetKey,
    SetPreviousResponseId,
    SetInstructions
}
