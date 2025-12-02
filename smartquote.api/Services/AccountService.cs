using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using smartquote.api.DTOs.Account;
using smartquote.api.DTOs.Account.Responses;
using smartquote.api.Entities;
using smartquote.api.Exceptions;
using smartquote.api.Repositories.Interfaces;
using smartquote.api.Services.Interfaces;
using smartquote.api.Options;
using System.Security.Authentication;
using System.Security.Claims;

namespace smartquote.api.Services;

public class AccountService : IAccountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly JwtOptions _jwtSettings;
    private readonly IConfiguration _configuration;

    private readonly UserManager<User> _userManager;

    public AccountService(
        IUnitOfWork unitOfWork,
        IPasswordHasher<User> passwordHasher,
        IJwtService jwtService,
        IEmailService emailService,
        IMapper mapper,
        UserManager<User> userManager,
        IOptions<JwtOptions> jwtSettings,
        IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _emailService = emailService;
        _mapper = mapper;
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
        _configuration = configuration;
    }
    public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var existingUser = await _unitOfWork.Users.GetByEmailAsync(normalizedEmail);

        if (existingUser != null)
        {
            return new RegisterResponseDto
            {
                Success = true,
                Message = "If this email can be used, you’ll receive an email with next steps shortly."
            };
        }

        var user = _mapper.Map<User>(request);
        user.Email = normalizedEmail;
        user.UserName = normalizedEmail;
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = System.Web.HttpUtility.UrlEncode(token);

        var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:5173";
        var confirmationLink = $"{frontendUrl}/auth/confirm-email?token={encodedToken}&email={normalizedEmail}";

        var body = $@"
        <html><body>
        <p>Dear {user.FirstName},</p>
        <p>Please confirm your email to activate your account:</p>
        <p><a href='{confirmationLink}' style='padding:10px 20px;background:#1d4ed8;color:white;text-decoration:none;border-radius:5px;'>Confirm Email</a></p>
        <p>This link will expire in 5 minutes.</p>
        </body></html>";

        await _emailService.SendEmailAsync(user.Email!, "Confirm Your Email", body);

        return new RegisterResponseDto
        {
            Success = true,
            Message = "Registration successful. Please check your email to confirm your account."
        };
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _unitOfWork.Users.GetByEmailAsync(normalizedEmail);

        if (user == null ||
            _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, request.Password)
            == PasswordVerificationResult.Failed)
        {
            throw new InvalidCredentialsException(
               "Invalid login attempt. Please check your email and password."
            );
        }

        if (!user.EmailConfirmed)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = System.Web.HttpUtility.UrlEncode(token);
            var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:5173";
            var confirmationLink = $"{frontendUrl}/auth/confirm-email?token={encodedToken}&email={normalizedEmail}";

            var body = $@"<html><body>
            <p>Dear {user.FirstName},</p>
            <p>Please confirm your email to continue:</p>
            <p><a href='{confirmationLink}' style='padding:10px 20px;background:#1d4ed8;color:white;text-decoration:none;border-radius:5px;'>Confirm Email</a></p>
            </body></html>";

            await _emailService.SendEmailAsync(user.Email!, "Confirm Your Email", body);

            return new LoginResponseDto
            {
                Success = false,
                Message = "Please confirm your email. A new confirmation message has been sent."
            };
        }

        var acessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _unitOfWork.SaveChangesAsync();

        return new LoginResponseDto
        {
            TokenType = "Bearer",
            AccessToken = acessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
        };
    }

    public async Task<ResendConfirmationEmailResponseDto> ResendConfirmationEmailAsync(ResendConfirmationEmailRequestDto request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _unitOfWork.Users.GetByEmailAsync(normalizedEmail);

        if (user == null || user.EmailConfirmed)
        {
            return new ResendConfirmationEmailResponseDto
            {
                Success = true,
                Message = "If this email is linked to an account, a confirmation message has been sent."
            };
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = System.Web.HttpUtility.UrlEncode(token);
        var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:5173";
        var confirmationLink = $"{frontendUrl}/auth/confirm-email?token={encodedToken}&email={normalizedEmail}";

        var body = $@"<html><body>
        <p>Dear {user.FirstName},</p>
        <p>Please confirm your email:</p>
        <p><a href='{confirmationLink}' style='padding:10px 20px;background:#1d4ed8;color:white;'>Confirm Email</a></p>
        </body></html>";

        await _emailService.SendEmailAsync(user.Email!, "Confirm Your Email", body);

        return new ResendConfirmationEmailResponseDto
        {
            Success = true,
            Message = "If this email exists, a confirmation message has been sent."
        };
    }

    public async Task<ConfirmEmailResponseDto> ConfirmEmailAsync(ConfirmEmailRequestDto request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _unitOfWork.Users.GetByEmailAsync(normalizedEmail);

        if (user == null || user.EmailConfirmed)
        {
            return new ConfirmEmailResponseDto
            {
                Success = true,
                Message = "Your email has been confirmed. You're all set!"
            };
        }

        var result = await _userManager.ConfirmEmailAsync(user, System.Web.HttpUtility.UrlDecode(request.Token));
        if (!result.Succeeded)
        {
            return new ConfirmEmailResponseDto
            {
                Success = false,
                Message = "Your confirmation link is invalid or expired. Please request a new link."
            };
        }

        return new ConfirmEmailResponseDto
        {
            Success = true,
            Message = "Your email has been confirmed. You're all set!"
        };
    }

    public async Task<ForgotPasswordResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _unitOfWork.Users.GetByEmailAsync(normalizedEmail);

        if (user == null || !user.EmailConfirmed)
        {
            return new ForgotPasswordResponseDto
            {
                Success = true,
                Message = "If this email is associated with an account, a reset link will be sent shortly."
            };
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = System.Web.HttpUtility.UrlEncode(token);
        var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:5173";
        var resetLink = $"{frontendUrl}/auth/reset-password?token={encodedToken}&email={normalizedEmail}";

        await _emailService.SendEmailAsync(
            user.Email!,
            "Password Reset Request",
            $@"<html><body>
            <p>Hello {user.FirstName},</p>
            <p>Click below to reset your password:</p>
            <a href='{resetLink}'>Reset Password</a>
            </body></html>"
        );

        return new ForgotPasswordResponseDto
        {
            Success = true,
            Message = "If this email is associated with an account, a reset link will be sent shortly."
        };
    }

    public async Task<ChangePasswordResponseDto> ChangePasswordAsync(string userEmail, ChangePasswordRequestDto request)
    {
        var normalizedEmail = userEmail.Trim().ToLowerInvariant();
        var user = await _unitOfWork.Users.GetByEmailAsync(normalizedEmail);

        if (user == null)
            throw new AuthenticationException("Authentication failed.");

        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, request.CurrentPassword);
        if (passwordVerificationResult == PasswordVerificationResult.Failed)
            throw new InvalidCredentialsException("Current password is incorrect.");

        user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return new ChangePasswordResponseDto();
    }

    public async Task<ResetPasswordResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _unitOfWork.Users.GetByEmailAsync(normalizedEmail);

        if (user == null)
        {
            return new ResetPasswordResponseDto
            {
                Success = true,
                Message = "Your password has been reset successfully."
            };
        }

        var result = await _userManager.ResetPasswordAsync(user, request.Code, request.NewPassword);
        if (!result.Succeeded)
            throw new BadRequestException("Password reset failed.");

        return new ResetPasswordResponseDto();
    }

    public async Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
            throw new AuthenticationException("Authentication failed.");

        var userEmail = principal.FindFirstValue(ClaimTypes.Email)?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(userEmail))
            throw new AuthenticationException("Authentication failed.");

        var user = await _unitOfWork.Users.GetByEmailAsync(userEmail);
        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            throw new AuthenticationException("Authentication failed.");

        var newAccessToken = _jwtService.GenerateAccessToken(user);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);
        await _unitOfWork.SaveChangesAsync();

        return new RefreshTokenResponseDto
        {
            TokenType = "Bearer",
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
        };
    }

    public async Task<AccountInfoResponseDto> GetAccountDetailsAsync(string email)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();
        var user = await _unitOfWork.Users.GetByEmailAsync(normalizedEmail);
        if (user == null) throw new NotFoundException("User not found.");

        var accountDetails = _mapper.Map<AccountDetailsDto>(user);
        return new AccountInfoResponseDto
        {
            AccountDetails = accountDetails
        };
    }

    public async Task<LogoutResponseDto> LogoutAsync(LogoutRequestDto request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await _unitOfWork.Users.GetByEmailAsync(normalizedEmail);

        if (user != null)
        {
            user.RefreshToken = string.Empty;
            user.RefreshTokenExpiryTime = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();
        }

        return new LogoutResponseDto();
    }
}
