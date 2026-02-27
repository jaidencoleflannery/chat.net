using System.Text.Json.Serialization;

namespace chat.net.Models;

public class AnthropicResponseDto : AiResponseDto {
    public AnthropicResponseDto (bool isSuccessful, Providers provider) : base(isSuccessful, provider) { }
    
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("type")]
    public string? Type { get; init; } = string.Empty;

    [JsonPropertyName("role")]
    public string? Role { get; init; } = string.Empty;  

    [JsonPropertyName("content")]
    public ContentDto[]? Content { get; init; } = null;

    [JsonPropertyName("model")]
    public string? Model { get; init; } = string.Empty;

    [JsonPropertyName("usage")]
    public UsageDto? Usage { get; init; } = null;

    public class ContentDto {
        [JsonPropertyName("type")]
        public string? Type { get; init; } = string.Empty;

        [JsonPropertyName("text")]
        public string? Text { get; init; } = string.Empty;
    }

    public class UsageDto {
        [JsonPropertyName("input_tokens")]
        public int InputTokens { get; init; } = -1;

        [JsonPropertyName("output_tokens")]
        public int OutputTokens { get; init; } = -1;
    }
}
