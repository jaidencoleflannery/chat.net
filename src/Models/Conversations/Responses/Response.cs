namespace chat.net.Models;

public class ResponseDto {
    public ResponseDto(bool isSuccessful) {
        IsSuccessful = isSuccessful;
    }
    public bool IsSuccessful { get; protected set; } = false;
}
