namespace smartquote.api.DTOs.Account.Responses;

public class ResetPasswordResponseDto
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = "Password has been reset successfully. You may now log in with your new password.";
}
