namespace smartquote.api.DTOs.Account.Responses;

public class LoginResponseDto
{

    public bool Success { get; set; } = true;
    public string TokenType { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public string Message { get; set; } = "User logged in successfully.";
}
