namespace smartquote.api.DTOs.Account.Responses;

public class ResendConfirmationEmailResponseDto
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = "Confirmation email was sent successfully.";
}
