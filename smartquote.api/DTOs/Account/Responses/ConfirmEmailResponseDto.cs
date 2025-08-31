namespace smartquote.api.DTOs.Account.Responses;

public class ConfirmEmailResponseDto
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = "Email confirmed successfully.";
}
