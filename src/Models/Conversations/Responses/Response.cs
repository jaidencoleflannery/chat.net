using System.Text.Json.Serialization;

namespace chat.net.Models;

public abstract class ResponseDto() {
    public Providers Provider { get; set; }

    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    public bool IsSuccessful { get; set; } = false;
}
