namespace chat.net.Models;

public class ResultResponseDto : ResponseDto {

    public string Text { get; set; }= string.Empty;

    public ResultResponseDto(bool isSuccessful) : base(isSuccessful) { } 
}
