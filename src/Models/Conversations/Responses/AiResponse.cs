using System.Text.Json.Serialization;

namespace chat.net.Models;

public class AiResponseDto : ResponseDto {

    private string _id = string.Empty;

    public AiResponseDto(bool isSuccessful, Providers provider) : base(isSuccessful) {
        Provider = provider;
    }    

    public Providers Provider { get; set; }

    [JsonPropertyName("id")]
    public string Id { 
        get => _id; 
        init => _id = value ?? string.Empty; 
    }
}
