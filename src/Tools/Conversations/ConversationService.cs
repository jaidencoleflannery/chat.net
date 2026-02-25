using System.Net.Http;
using System.Text;
using System.Text.Json;
using chat.net.Configurations;
using chat.net.Commands;

namespace chat.net.Conversations;

public static class ConversationService {

    static Dictionary<Providers, Func<string, string, string, string>> map = new() {
        [Providers.Openai] = (input, model, key) => OpenAiCall(input, model, key),
        [Providers.Anthropic] = (input, model, key) => AnthropicCall(input, model, key),
        [Providers.Google] = (input, model, key) => GoogleCall(input, model, key),
        [Providers.Xai] = (input, model, key) => XaiCall(input, model, key),
        [Providers.Deepseek] = (input, model, key) => DeepseekCall(input, model, key),
    };

    public static string? Call(string input) { 
        var providerString = ConfigurationService.GetValue(Configuration.ConfigurationAttributes.Provider)!;
        if(!Enum.TryParse<Providers>(providerString, out var provider))
            providerString = "";

        var model = ConfigurationService.GetValue(Configuration.ConfigurationAttributes.Model)!;
        if(model == null)
            model = "";

        if(!GetKey(provider, out var key)) {
            Console.WriteLine("Failed to get key. Set your API key for your desired model with 'ask --config --set-key <key>'");
            return null;
        }

        return map[provider](input, model, key);
    }

    public static string OpenAiCall(string input, string model, string key) =>
        SendRequest("https://api.openai.com/v1/responses", input, model, key);

    public static string AnthropicCall(string input, string model, string key) {
        Console.WriteLine(input);
        return "";
    }

    public static string GoogleCall(string input, string model, string key) {
        Console.WriteLine(input);
        return "";
    }

    public static string XaiCall(string input, string model, string key) {
        Console.WriteLine(input);
        return "";
    }

    public static string DeepseekCall(string input, string model, string key) {
        Console.WriteLine(input);
        return "";
    }

    public static bool GetKey(Providers provider, out string key) {
        key = Environment.GetEnvironmentVariable($"{provider.ToString().ToLower()}-key:")!;
        return !(key == null);
    }

    public static string SendRequest(string url, string input, string model, string key) { 
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("Content-Type", "application/json");
        request.Headers.Add("Authorization", $"Bearer ${key}");

        var json = JsonSerializer.Serialize(new {
            model,
            input
        });

        request.Content = new StringContent(
            json,
            Encoding.UTF8,
            "application/x-www-form-urlencoded"
        );

        Console.WriteLine(request);

        using var client = new HttpClient();

        return "";
    }
}
