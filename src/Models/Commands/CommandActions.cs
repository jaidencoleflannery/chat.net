namespace chat.net.Models;

public record CommandActions : Command;

public enum CommandAction {
    Config,
    Input,
}
