namespace smartquote.api.DTOs.Account;

public class ConfirmEmailRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
