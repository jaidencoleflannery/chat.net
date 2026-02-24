namespace chat.net.Commands;

public record CommandActions : Command;

public enum CommandAction {
    Config,
    Input,
}
