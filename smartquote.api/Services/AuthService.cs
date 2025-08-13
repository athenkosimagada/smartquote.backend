using AutoMapper;
using Microsoft.AspNetCore.Identity;
using smartquote.api.DTOs.Auth;
using smartquote.api.DTOs.Auth.Responses;
using smartquote.api.Entities;
using smartquote.api.Exceptions;
using smartquote.api.Repositories.Interfaces;
using smartquote.api.Services.Interfaces;

namespace smartquote.api.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IJwtService _jwtService;
    private readonly IMapper _mapper;

    public AuthService(
        IUnitOfWork unitOfWork,
        IPasswordHasher<User> passwordHasher,
        IJwtService jwtService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _jwtService = jwtService;
        _mapper = mapper;
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

        var acessToken = _jwtService.GenerateAccessToken(request);
        var refreshToken = _jwtService.GenerateRefreshToken();

        return new LoginResponseDto
        {
            AccessToken = acessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
        };
    }

    public Task<ResendConfirmationEmailResponseDto> ResendConfirmationEmailAsync(ResendConfirmationEmailRequestDto request)
    {
        throw new NotImplementedException();
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

    public Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        throw new NotImplementedException();
    }

    public Task<LogoutResponseDto> LogoutAsync(LogoutRequestDto request)
    {
        throw new NotImplementedException();
    }
}
