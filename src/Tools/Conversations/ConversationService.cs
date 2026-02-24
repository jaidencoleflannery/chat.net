using System.Net.Http;
using System.Text;
using System.Text.Json;
using chat.net.Configurations;
using chat.net.Commands;

namespace chat.net.Conversations;

public static class ConversationService {

    static Dictionary<Providers, Func<string, string>> map = new() {
        [Providers.Openai] = cmd => OpenAiCall(cmd),
        [Providers.Anthropic] = cmd => AnthropicCall(cmd),
        [Providers.Google] = cmd => GoogleCall(cmd),
        [Providers.Xai] = cmd => XaiCall(cmd),
        [Providers.Deepseek] = cmd => DeepseekCall(cmd),
    };

    static string key = "";

    public static string? Call(string input) {
        string? providerString = ConfigurationService.GetProvider();
        if(providerString == null)
            return null;
        if(!Enum.TryParse<Providers>(providerString, out var provider))
            return null;

        key = GetKey(provider);

        return map[provider](input);
    }

    public static string OpenAiCall(string input) {
        string url = "https://api.openai.com/v1/responses";
        Console.WriteLine(input);
        return SendRequest(url, input);
    }

    public static string AnthropicCall(string input) {
        Console.WriteLine(input);
        return "";
    }

    public static string GoogleCall(string input) {
        Console.WriteLine(input);
        return "";
    }

    public static string XaiCall(string input) {
        Console.WriteLine(input);
        return "";
    }

    public static string DeepseekCall(string input) {
        Console.WriteLine(input);
        return "";
    }

    public static string? GetKey(Providers provider) =>
        Environment.GetEnvironmentVariable($"{provider.ToString().ToLower()}-key:")
            ?? null;

    public static string SendRequest(string url, string input, string model, string token) { 
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("Content-Type", "application/json");
        request.Headers.Add("Authorization", $"Bearer ${token}");

        var json = JsonSerializer.Serialize(new {
            model,
            input
        });

        request.Content = new StringContent(
            json,
            Encoding.UTF8,
            "application/x-www-form-urlencoded"
        );

        using var client = new HttpClient();

        return "";
    }
}
