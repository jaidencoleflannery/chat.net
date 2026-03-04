using System.Text.Json.Serialization;

namespace chat.net.Models;

public class ResultResponseDto : ResponseDto {
    public string Message { get; set; }
    public ResultResponseDto(bool isSuccessful, string message) : base(isSuccessful) {
        Message = message ?? string.Empty;
    }  
}
