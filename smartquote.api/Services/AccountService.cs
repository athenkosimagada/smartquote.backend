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
                Message = "We have received your registration request. If this email can be used, you will receive a confirmation shortly."
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
        <p>Thank you for registering. Click the button below to confirm your email:</p>
        <p><a href='{confirmationLink}' style='padding:10px 20px;background:#1d4ed8;color:white;text-decoration:none;border-radius:5px;'>Confirm Email</a></p>
        <p>This link will expire in 5 minutes.</p>
        <p>If you received this by mistake, you can ignore this email.</p>
        <p>Kind regards,<br/>SmartQuote Team</p>
        </body></html>";

        await _emailService.SendEmailAsync(user.Email!, "Confirm Your Email", body);

        return new RegisterResponseDto
        {
            Success = true,
            Message = "We have received your registration request. You will receive a confirmation email shortly."
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
            throw new InvalidCredentialsException("Invalid email or password");
        }

        if (!user.EmailConfirmed)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = System.Web.HttpUtility.UrlEncode(token);
            var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:5173";
            var confirmationLink = $"{frontendUrl}/auth/confirm-email?token={encodedToken}&email={normalizedEmail}";

            var body = $@"
                <html><body>
                <p>Dear {user.FirstName},</p>
                <p>Your account needs email confirmation. Click the button below to confirm it:</p>
                <p><a href='{confirmationLink}' style='padding:10px 20px;background:#1d4ed8;color:white;text-decoration:none;border-radius:5px;'>Confirm Email</a></p>
                <p>This link will expire in 5 minutes.</p>
                <p>If you received this email by mistake, you can ignore it.</p>
                <p>Kind regards,<br/>SmartQuote Team</p>
                </body></html>";

            await _emailService.SendEmailAsync(user.Email!, "Confirm Your Email", body);

            return new LoginResponseDto
            {
                Success = false,
                Message = "Your account is not confirmed. A confirmation email has been sent to you."
            };
        }

        var acessToken = _jwtService.GenerateAccessToken(user);
        var refreshToken = _jwtService.GenerateRefreshToken();

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
                Message = "You will receive a confirmation email if this email address belongs to an account."
            };
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = System.Web.HttpUtility.UrlEncode(token);
        var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:5173";
        var confirmationLink = $"{frontendUrl}/auth/confirm-email?token={encodedToken}&email={normalizedEmail}";

        var body = $@"
        <html><body>
        <p>Dear {user.FirstName},</p>
        <p>Thank you for registering. Click the button below to confirm your email:</p>
        <p><a href='{confirmationLink}' style='padding:10px 20px;background:#1d4ed8;color:white;text-decoration:none;border-radius:5px;'>Confirm Email</a></p>
        <p>This link will expire in 5 minutes.</p>
        <p>If you received this by mistake, you can ignore this email.</p>
        <p>Kind regards,<br/>SmartQuote Team</p>
        </body></html>";

        await _emailService.SendEmailAsync(user.Email!, "Confirm Your Email", body);

        return new ResendConfirmationEmailResponseDto
        {
            Success = true,
            Message = "You will receive a confirmation email if this email address belongs to an account."
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
                Message = "If this is your account, your email is now confirmed. Thank you!"
            };
        }

        var result = await _userManager.ConfirmEmailAsync(user, request.Code);
        if (!result.Succeeded)
        {
            return new ConfirmEmailResponseDto
            {
                Success = false,
                Message = "Your confirmation link is invalid or has expired. Please request a new confirmation email."
            };
        }

        return new ConfirmEmailResponseDto
        {
            Success = true,
            Message = "Your email has been successfully confirmed. Welcome aboard!"
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
                Message = "If this email is associated with an account, you will receive a password reset link shortly."
            };
        }

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = System.Web.HttpUtility.UrlEncode(token);
        var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:5173";
        var resetLink = $"{frontendUrl}/auth/reset-password?token={encodedToken}&email={normalizedEmail}";

        var body = $@"
            <html><body>
            <p>Dear {user.FirstName},</p>
            <p>You requested to reset your password. Click the button below to reset it:</p>
            <p><a href='{resetLink}' style='padding:10px 20px;background:#1d4ed8;color:white;text-decoration:none;border-radius:5px;'>Reset Password</a></p>
            <p>This link will expire in 5 minutes.</p>
            <p>If you received this email by mistake, you can safely ignore it.</p>
            <p>Kind regards,<br/>SmartQuote Team</p>
            </body></html>";

        await _emailService.SendEmailAsync(user.Email!, "Password Reset Request", body);

        return new ForgotPasswordResponseDto
        {
            Success = true,
            Message = "If this email is associated with an account, you will receive a password reset link shortly."
        };
    }

    public async Task<ChangePasswordResponseDto> ChangePasswordAsync(string userEmail, ChangePasswordRequestDto request)
    {
        var normalizedEmail = userEmail.Trim().ToLowerInvariant();
        var user = await _unitOfWork.Users.GetByEmailAsync(normalizedEmail);

        if (user == null) throw new AuthenticationException("Authentication failed.");

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
                Message = "If this email is associated with an account, your password has been reset successfully."
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
