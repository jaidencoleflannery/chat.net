namespace chat.net.Models;

public class AiResponseDto : ResponseDto {
    public AiResponseDto(bool isSuccessful, Providers provider) : base(isSuccessful) {
        Provider = provider;
    }
    
    public Providers Provider { get; set; }
}
