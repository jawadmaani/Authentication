namespace Authentication.Dto;

public class AuthResponseDto
{
    public string Message { get; set; }
    public string? AccessToken { get; set; }
    public int? ExpiresIn { get; set; }
}