using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using smartquote.api.Entities;
using smartquote.api.Services.Interfaces;
using smartquote.api.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace smartquote.api.Services;

public class JwtService : IJwtService
{
    private readonly JwtOptions _jwtSettings;

    public JwtService(IOptions<JwtOptions> jwtOptions)
    {
        _jwtSettings = jwtOptions.Value;
    }

    public string GenerateAccessToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Email!),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            }),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
    {
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,

            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        SecurityToken securityToken;

        if (!tokenHandler.CanReadToken(accessToken))
        {
            throw new SecurityTokenException("Authentication failed.");
        }

        try
        {
            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out securityToken);
            return principal;
        }
        catch (Exception ex) when (
        ex is SecurityTokenExpiredException ||
        ex is SecurityTokenInvalidSignatureException ||
        ex is SecurityTokenException)
        {
            throw new SecurityTokenException("Authentication failed.");
        }
        catch (Exception ex)
        {
            throw new SecurityTokenException("Authentication failed.");
        }
    }
}
