using System.Text;
using System.Text.Json;
using System.Net.Http.Json;
using chat.net.Configurations;
using chat.net.Models;

namespace chat.net.Conversations;

public static class ConversationService {
 
    static Dictionary<Providers, Func<string, string, string?, string, Task<ResponseDto>>> map = new() {
        [Providers.Openai] = (input, model, previousResponseId, key) => OpenAiCall(input, model, previousResponseId, key),
        [Providers.Anthropic] = (input, model, previousResponseId, key) => AnthropicCall(input, model, previousResponseId, key),
        [Providers.Google] = (input, model, previousResponseId, key) => GoogleCall(input, model, previousResponseId, key),
        [Providers.Xai] = (input, model, previousResponseId, key) => XaiCall(input, model, previousResponseId, key),
        [Providers.Deepseek] = (input, model, previousResponseId, key) => DeepseekCall(input, model, previousResponseId, key),
    };

    public static async Task<ResponseDto> Call(string input, string? PreviousResponseId) { 
        var providerString = ConfigurationService.GetValue(Configuration.ConfigurationAttributes.Provider)!;
        if(string.IsNullOrWhiteSpace(providerString))
            throw new InvalidOperationException("Could not pull value for provider from config.");
        if(!Enum.TryParse<Providers>(providerString, out var provider))
            throw new InvalidOperationException("Could not verify provider value pulled from config.");

        var model = ConfigurationService.GetValue(Configuration.ConfigurationAttributes.Model)!;
        if(model == null)
            throw new InvalidOperationException("Could not pull value for model from config.");

        var key = ConfigurationService.GetValue(Configuration.ConfigurationAttributes.Key);
        if(key == null)
            throw new InvalidOperationException("Failed to get key from config. Set your API key for your desired model with 'ask --config --set-key <key>'");

        return await map[provider](input, model, PreviousResponseId, key);
    }

    public static async Task<ResponseDto> OpenAiCall(string input, string model, string? previousResponseId, string key) =>
        await SendRequest("https://api.openai.com/v1/responses", input, model, previousResponseId, key);

    public static async Task<ResponseDto> AnthropicCall(string input, string model, string? previousResponseId, string key) =>
        await SendRequest("url", input, model, previousResponseId, key);
        
    public static async Task<ResponseDto> GoogleCall(string input, string model, string? previousResponseId, string key) =>
        await SendRequest("url", input, model, previousResponseId, key);

    public static async Task<ResponseDto> XaiCall(string input, string model, string? previousResponseId, string key) =>
        await SendRequest("url", input, model, previousResponseId, key);

    public static async Task<ResponseDto> DeepseekCall(string input, string model, string? previousResponseId, string key) =>
        await SendRequest("url", input, model, previousResponseId, key);


    public static async Task<OpenAiResponseDto> SendRequest(string url, string input, string model, string? PreviousResponseId, string key) {
        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Add("Authorization", $"Bearer {key}");

        var json = JsonSerializer.Serialize(new {
            model = model,
            input = input,
            previous_response_id = PreviousResponseId 
        });

        request.Content = new StringContent(
            json,
            Encoding.UTF8,
            "application/json"
        ); 

        using var client = new HttpClient();
        var response = await client.SendAsync(request);

        response.EnsureSuccessStatusCode(); 

        var body = await response.Content.ReadFromJsonAsync<OpenAiResponseDto>();
        if(body == null)
            throw new InvalidOperationException($"Body could not be parsed from response. {body?.ToString()}"); 

        return body;
    }
}
