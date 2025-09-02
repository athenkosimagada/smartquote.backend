using smartquote.api.DTOs.Account;
using smartquote.api.DTOs.Account.Responses;

namespace smartquote.api.Services.Interfaces;

public interface IAccountService
{
    Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
    Task<LogoutResponseDto> LogoutAsync(LogoutRequestDto request);
    Task<ResendConfirmationEmailResponseDto> ResendConfirmationEmailAsync(ResendConfirmationEmailRequestDto request);
    Task<ConfirmEmailResponseDto> ConfirmEmailAsync(ConfirmEmailRequestDto request);
    Task<ForgotPasswordResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto request);
    Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
    Task<ChangePasswordResponseDto> ChangePasswordAsync(string userEmail, ChangePasswordRequestDto request);
    Task<ResetPasswordResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request);

    Task<AccountInfoResponseDto> GetAccountDetailsAsync(string email);
}
