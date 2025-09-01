namespace smartquote.api.DTOs.Account.Responses;

public class ForgotPasswordResponseDto
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = "Password reset code was sent successfully.";
}
