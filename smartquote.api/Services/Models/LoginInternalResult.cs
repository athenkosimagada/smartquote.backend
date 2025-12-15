namespace smartquote.api.Services.Models;

public sealed class LoginInternalResult
{
    public bool Success { get; set; } = true;
    public string TokenType { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiryTime { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public string Message { get; set; } = "User logged in successfully.";
}
