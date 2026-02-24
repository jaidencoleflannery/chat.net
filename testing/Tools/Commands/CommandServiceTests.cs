using Xunit;
using chat.net.Commands;
using chat.net.Configurations;

namespace chat.net.Testing.Commands;

public class CommandServiceTests {

    [Theory]
    [InlineData("gemini")]
    [InlineData("google")]
    [InlineData("uncle rodrick's super sick local llm cluster")]    
    public void ExecuteConfigSetModelChangesConfig(string val) {
        Config command = new Config(ActionArgument: ConfigActionRequiresArgument.SetModel, Value: val);
        CommandService.Execute(command);

        Configuration? config = ConfigurationService.GetConfig();
        Assert.NotNull(config);
        Assert.NotNull(config.Model);
        Assert.Equal(command.Value, config.Model);
    }

    [Fact] 
    public void ExecuteConfigSetModelBadDoesntChangeConfig() {
        Config command1 = new Config(ActionArgument: ConfigActionRequiresArgument.SetModel, Value: "gemini");
        CommandService.Execute(command1);

        Configuration? config = ConfigurationService.GetConfig();
        Assert.NotNull(config);
        Assert.NotNull(config.Model);
        Assert.Equal(command1.Value, config.Model);

        Config command2 = new Config(ActionArgument: ConfigActionRequiresArgument.SetModel, Value: "");
        CommandService.Execute(command2);

        config = ConfigurationService.GetConfig();
        Assert.NotNull(config);
        Assert.NotNull(config.Model);
        Assert.Equal(command1.Value, config.Model);
    }
}
