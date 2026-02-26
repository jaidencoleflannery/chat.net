using System.Text;
using System.Text.Json;
using System.Net.Http.Json;
using chat.net.Configurations;
using chat.net.Models;

namespace chat.net.Conversations;

public static class ConversationService {

    static Dictionary<Providers, Func<string, string, string, Task<string>>> map = new() {
        [Providers.Openai] = (input, model, key) => OpenAiCall(input, model, key),
        [Providers.Anthropic] = (input, model, key) => AnthropicCall(input, model, key),
        [Providers.Google] = (input, model, key) => GoogleCall(input, model, key),
        [Providers.Xai] = (input, model, key) => XaiCall(input, model, key),
        [Providers.Deepseek] = (input, model, key) => DeepseekCall(input, model, key),
    };

    public static async Task<string?> Call(string input) { 
        var providerString = ConfigurationService.GetValue(Configuration.ConfigurationAttributes.Provider)!;
        if(!Enum.TryParse<Providers>(providerString, out var provider))
            providerString = "";

        var model = ConfigurationService.GetValue(Configuration.ConfigurationAttributes.Model)!;
        if(model == null)
            model = "";

        var key = ConfigurationService.GetValue(Configuration.ConfigurationAttributes.Key);
        if(key == null) {
            Console.WriteLine("Failed to get key. Set your API key for your desired model with 'ask --config --set-key <key>'");
            Environment.Exit(1);
        }
        return await map[provider](input, model, key);
    }

    public static async Task<string> OpenAiCall(string input, string model, string key) =>
        await SendRequest("https://api.openai.com/v1/responses", input, model, key);

    public static async Task<string> AnthropicCall(string input, string model, string key) {
        Console.WriteLine("anthropic");
        return "";
    }

    public static async Task<string> GoogleCall(string input, string model, string key) {
        Console.WriteLine("google");
        return "";
    }

    public static async Task<string> XaiCall(string input, string model, string key) {
        Console.WriteLine("xai");
        return "";
    }

    public static async Task<string> DeepseekCall(string input, string model, string key) {
        Console.WriteLine("deepseek");
        return "";
    }

    public static async Task<string> SendRequest(string url, string input, string model, string key) { 
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("Authorization", $"Bearer {key}");

        var json = JsonSerializer.Serialize(new {
            model,
            input
        });

        request.Content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json"
        );
 

        using var client = new HttpClient();
        var response = await client.SendAsync(request);

        try {
            response.EnsureSuccessStatusCode();
        } catch {
            Console.WriteLine($"Response was not successful. {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
        }
        var body = await response.Content.ReadFromJsonAsync<OpenAiResponseDto>();
        Console.WriteLine($"{body?.Output?[0]?.Content?[0]?.Text}");

        return "";
    }
}
