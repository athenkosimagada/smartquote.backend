namespace smartquote.api.DTOs.Account.Responses;

public class LogoutResponseDto
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = "Logged out successfully.";
}
