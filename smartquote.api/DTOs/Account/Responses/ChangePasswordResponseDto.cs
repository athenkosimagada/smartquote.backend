namespace smartquote.api.DTOs.Account.Responses;

public class ChangePasswordResponseDto
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = "Password has been changed successfully.";
}
