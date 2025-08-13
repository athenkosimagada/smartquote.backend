using smartquote.api.DTOs.Account;

namespace smartquote.api.Services.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(LoginRequestDto request);
    string GenerateRefreshToken();
}
