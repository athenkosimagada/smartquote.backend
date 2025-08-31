namespace smartquote.api.DTOs.Account.Responses;

public class AccountInfoResponseDto
{
    public bool Success { get; set; } = true;
    public AccountDetailsDto? AccountDetails { get; set; }
}
