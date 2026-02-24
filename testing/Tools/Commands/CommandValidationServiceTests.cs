using Xunit;
using chat.net.Commands;

namespace chat.net.Testing.Commands;

public class CommandValidationServiceTests {
   
    // Config

    // SetModel Commands
    [Fact]
    public void ValidateCommandsSetModelReturnsConfigCommand() {
        string[] args = { "--config", "--set-model", "gippity" };
        Config response = (Config)CommandValidationService.ValidateCommands(args);

        Assert.IsType<Config>(response);
        Assert.IsType<ConfigActionRequiresArgument>(response.ActionArgument);
        Assert.Equal(ConfigActionRequiresArgument.SetModel, response.ActionArgument);
        Assert.Equal("gippity", response.Value);
    }

    [Theory]
    [InlineData("--config", "--set-model")]
    [InlineData("--config", "")]
    public void ValidateCommandsSetModelBadReturnsNull(params string[] args) {
        var response = CommandValidationService.ValidateCommands(args);

        Assert.Null(response);
    }

    [Fact]
    public void ValidateCommandsSetProviderReturnsConfigCommand() {
        string[] args = { "--config", "--set-provider", "google" };
        Config response = (Config)CommandValidationService.ValidateCommands(args);

        Assert.IsType<Config>(response);
        Assert.IsType<ConfigActionRequiresArgument>(response.ActionArgument);
        Assert.Equal(ConfigActionRequiresArgument.SetProvider, response.ActionArgument);
        Assert.Equal("google", response.Value);
    }

    [Theory]
    [InlineData("--config", "--set-provider", "random")]
    [InlineData("--config", "--set-provider")]
    [InlineData("--config", "")]
    public void ValidateCommandsSetProviderBadReturnsNull(params string[] args) {
        var response = CommandValidationService.ValidateCommands(args);

        Assert.Null(response);
    }

    [Fact]
    public void ValidateCommandsSendMessagesReturnsInputCommand() {
        string[] args = { "hey!" };
        Input response = (Input)CommandValidationService.ValidateCommands(args);

        Assert.IsType<Input>(response);
        Assert.Equal("hey!", response.Text);
    }
}
