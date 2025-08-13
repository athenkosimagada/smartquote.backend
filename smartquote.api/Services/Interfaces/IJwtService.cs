using smartquote.api.DTOs.Auth;

namespace smartquote.api.Services.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(LoginRequestDto request);
    string GenerateRefreshToken();
}
