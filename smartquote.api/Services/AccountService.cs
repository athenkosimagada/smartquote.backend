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

    private readonly UserManager<User> _userManager;

    public AccountService(
        IUnitOfWork unitOfWork,
        IPasswordHasher<User> passwordHasher,
        IJwtService jwtService,
        IEmailService emailService,
        IMapper mapper,
        UserManager<User> userManager,
        IOptions<JwtOptions> jwtSettings)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _emailService = emailService;
        _mapper = mapper;
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
    }
    public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        var existingUser = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        if (existingUser != null) throw new AlreadyExistException("Email already in use.");

        var user = _mapper.Map<User>(request);
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return new RegisterResponseDto();
    }
    
    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        if (user == null) throw new InvalidCredentialsException();

        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash!, request.Password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
            throw new InvalidCredentialsException();

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
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        if (user == null) throw new NotFoundException("User with this email not found.");

        if(user.EmailConfirmed) throw new BadRequestException("Email is already confirmed.");

        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

        var body = $"<html>" +
            $"<body>" +
            $"<p>Dear {user.FirstName},</p>" +
            $"<p>Thank you for registering with us. Please use the confirmation code below to verify your email address:</p>" +
            $"<h2>{code}</h2>" +
            $"</body></html>";

        await _emailService.SendEmailAsync(user.Email!, "Email Confirmation", body);
        return new ResendConfirmationEmailResponseDto();
    }

    public Task<ConfirmEmailResponseDto> ConfirmEmailAsync(ConfirmEmailRequestDto request)
    {
        throw new NotImplementedException();
    }

    public Task<ForgotPasswordResponseDto> ForgotPasswordAsync(ForgotPasswordRequestDto request)
    {
        throw new NotImplementedException();
    }

    public Task<ChangePasswordResponseDto> ChangePasswordAsync(ChangePasswordRequestDto request)
    {
        throw new NotImplementedException();
    }

    public Task<ResetPasswordResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request)
    {
        throw new NotImplementedException();
    }

    public async Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        ClaimsPrincipal principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);

        if (principal == null)
        {
            throw new AuthenticationException("Authentication failed.");
        }

        string userEmail = principal.FindFirstValue(ClaimTypes.Email)!;

        if (string.IsNullOrWhiteSpace(userEmail))
        {
            throw new AuthenticationException("Authentication failed.");
        }

        var user = await _unitOfWork.Users.GetByEmailAsync(userEmail);

        if (user == null)
        {
            throw new AuthenticationException("Authentication failed.");
        }

        if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new AuthenticationException("Authentication failed.");
        }

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

    public async Task<LogoutResponseDto> LogoutAsync(LogoutRequestDto request)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);

        if (user == null) return new LogoutResponseDto();

        user.RefreshToken = string.Empty;
        user.RefreshTokenExpiryTime = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync();

        return new LogoutResponseDto();
    }

    public async Task<AccountDetailsResponseDto> GetAccountDetailsAsync(string email)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(email);
        if (user == null) throw new NotFoundException("User not found.");
        var accountDetails = _mapper.Map<AccountDetailsDto>(user);
        return new AccountDetailsResponseDto
        {
            AccountDetails = accountDetails
        };
    }
}
