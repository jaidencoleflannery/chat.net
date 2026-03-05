namespace chat.net.Models;

public enum CommandAction {
    Config,
    Clear,
    Help,

    [PrivateArgument]
    Input 
}
