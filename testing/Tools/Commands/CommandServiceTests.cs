using Xunit;
using chat.net.Commands;

namespace chat.net.Testing.Commands;

public class CommandValidationServiceTests {

    // REPLACE THESE

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

}
