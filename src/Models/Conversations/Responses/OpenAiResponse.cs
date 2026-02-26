using System.Text.Json.Serialization;

namespace chat.net.Models;

public class OpenAiResponseDto : ResponseDto {
    public OpenAiResponseDto() { }
    
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("created_at")]
    public int? CreatedAt { get; init; } = null;

    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;  

    [JsonPropertyName("completed_at")]
    public int? CompletedAt { get; init; } = null;

    [JsonPropertyName("instructions")]
    public string Instructions { get; init; } = string.Empty;

    [JsonPropertyName("output")]
    public OutputDto[]? Output { get; init; } = null;

    public class OutputDto {
        public class ContentDto {
            public class AnnotationsDto {
                [JsonPropertyName("type")]
                public string Type { get; init; } = string.Empty;

                [JsonPropertyName("file_id")]
                public string? FileId { get; init; } = null;

                [JsonPropertyName("filename")]
                public string? FileName { get; init; } = null;

                [JsonPropertyName("index")]
                public int? Index { get; init; } = null;

                [JsonPropertyName("start_index")]
                public int? StartIndex { get; init; } = null;

                [JsonPropertyName("end_index")]
                public int? EndIndex { get; init; } = null;

                [JsonPropertyName("url")]
                public string? Url { get; init; } = null;

                [JsonPropertyName("container_id")]
                public string? ContainerId { get; init; } = null;
            }

            [JsonPropertyName("type")]
            public string Type { get; init; } = string.Empty;

            [JsonPropertyName("annotations")]
            public AnnotationsDto[]? Annotations { get; init; } = null;

            [JsonPropertyName("text")]
            public string Text { get; init; } = string.Empty;
        }

        [JsonPropertyName("id")]
        public string Id { get; init; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type { get; init; } = string.Empty;

        [JsonPropertyName("status")]
        public string Status { get; init; } = string.Empty;

        [JsonPropertyName("content")]
        public ContentDto[]? Content { get; init; } = null;

        [JsonPropertyName("role")]
        public string Role = string.Empty;
    }
}
