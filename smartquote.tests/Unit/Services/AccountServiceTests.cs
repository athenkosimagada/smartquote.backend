using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Moq;
using smartquote.api;
using smartquote.api.DTOs.Account;
using smartquote.api.Entities;
using smartquote.api.Exceptions;
using smartquote.api.Repositories.Interfaces;
using smartquote.api.Services;
using smartquote.api.Services.Interfaces;
using smartquote.api.Options;

namespace smartquote.tests.Unit.Services;

public class AccountServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWork;
    private readonly Mock<IUserRepository> _userRepository;
    private readonly Mock<IJwtService> _jwtService;
    private readonly Mock<IEmailService> _emailService;
    private readonly Mock<IMapper> _mapper;
    private readonly Mock<UserManager<User>> _userManager;

    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IOptions<JwtOptions> _jwtOptions;
    private readonly IAccountService _authService;

    public AccountServiceTests()
    {
        _userRepository = new Mock<IUserRepository>();
        _unitOfWork = new Mock<IUnitOfWork>();
        _jwtService = new Mock<IJwtService>();
        _emailService = new Mock<IEmailService>();
        _mapper = new Mock<IMapper>();

        _passwordHasher = new PasswordHasher<User>();

        var store = new Mock<IUserStore<User>>();
        _userManager = new Mock<UserManager<User>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        _passwordHasher = new PasswordHasher<User>();

        _jwtOptions = Options.Create(new JwtOptions
        {
            SecretKey = "ThisIsASecretKeyForJwt",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpirationMinutes = 60
        });

        _unitOfWork.Setup(u => u.Users).Returns(_userRepository.Object);

        _authService = new AccountService(
            _unitOfWork.Object,
            _passwordHasher,
            _jwtService.Object,
            _emailService.Object,
            _mapper.Object,
            _userManager.Object,
            _jwtOptions);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateNewUserWithHashedPassword_WhenEmailIsNotInUse()
    {
        var request = new RegisterRequestDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "janesmith@example.com",
            Password = "SecurePassword@123"
        };

        _mapper
            .Setup(m => m.Map<User>(It.IsAny<RegisterRequestDto>()))
            .Returns(new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                FullName = $"{request.FirstName} {request.LastName}"
            });

        _userRepository
            .Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync((User)null!);

        User capturedUser = null!;
        _userRepository
            .Setup(r => r.AddAsync(It.IsAny<User>()))
            .Callback<User>(u => capturedUser = u)
            .Returns(Task.CompletedTask);

        var result = await _authService.RegisterAsync(request);

        Assert.NotNull(capturedUser);
        Assert.NotEqual(request.Password, capturedUser.PasswordHash);
        Assert.False(string.IsNullOrWhiteSpace(capturedUser.PasswordHash));

        result.Should().NotBeNull();
        var verificationResult = _passwordHasher.VerifyHashedPassword(
         capturedUser,
         capturedUser.PasswordHash,
         request.Password);
        Assert.Equal(PasswordVerificationResult.Success, verificationResult);

        _userRepository.Verify(r => r.AddAsync(It.Is<User>(u =>
            u.Email == request.Email &&
            u.FirstName == request.FirstName &&
            u.LastName == request.LastName &&
            u.FullName == $"{request.FirstName} {request.LastName}"
        )), Times.Once);
        _unitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_ShouldThrowAlreadyExistException_WhenEmailAlreadyExists()
    {
        var request = new RegisterRequestDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "janesmith@example.com",
            Password = "SecurePassword@123"
        };

        var existingUser = new User
        {
            Email = request.Email,
            FirstName = "Existing",
            LastName = "User",
            FullName = "Existing User",
            PasswordHash = _passwordHasher.HashPassword(null!, "OldPassword@123")
        };

        _userRepository
            .Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync(existingUser);

        await Assert.ThrowsAsync<AlreadyExistException>(() =>
           _authService.RegisterAsync(request));
        _userRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnLoginResponse_WhenCredentialsAreValid()
    {
        var request = new LoginRequestDto
        {
            Email = "janesmith@example.com",
            Password = "SecurePassword@123"
        };

        var hashedPassword = _passwordHasher.HashPassword(null!, request.Password);

        _userRepository
            .Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync(new User
            {
                Email = request.Email,
                PasswordHash = hashedPassword,
                EmailConfirmed = true
            });

        _jwtService
            .Setup(j => j.GenerateAccessToken(It.IsAny<User>()))
            .Returns("access_token");

        _jwtService
            .Setup(j => j.GenerateRefreshToken())
            .Returns("refresh_token");

        var result = await _authService.LoginAsync(request);

        _userRepository.Verify(r => r.GetByEmailAsync(request.Email), Times.Once);
        _jwtService.Verify(j => j.GenerateRefreshToken(), Times.Once);

        result.Should().NotBeNull();
        result.AccessToken.Should().Be("access_token");
        result.RefreshToken.Should().Be("refresh_token");
        result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowInvalidCredentialsException_WhenEmailDoesNotExist()
    {
        var request = new LoginRequestDto
        {
            Email = "janesmith@example.com",
            Password = "SecurePassword@123"
        };

        _userRepository
            .Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync((User)null!);

        await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
            _authService.LoginAsync(request));

        _userRepository.Verify(r => r.GetByEmailAsync(request.Email), Times.Once);
        _jwtService.Verify(j => j.GenerateAccessToken(It.IsAny<User>()), Times.Never);
        _jwtService.Verify(j => j.GenerateRefreshToken(), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_ShouldThrowInvalidCredentialsException_WhenPasswordIsIncorrect()
    {
        var request = new LoginRequestDto
        {
            Email = "janesmith@example.com",
            Password = "WrongPassword@123"
        };

        var existingUser = new User
        {
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(null!, "SecurePassword@123"),
            EmailConfirmed = true
        };

        _userRepository
            .Setup(r => r.GetByEmailAsync(request.Email))
            .ReturnsAsync(existingUser);

        await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
            _authService.LoginAsync(request));

        _userRepository.Verify(r => r.GetByEmailAsync(request.Email), Times.Once);
        _jwtService.Verify(j => j.GenerateAccessToken(It.IsAny<User>()), Times.Never);
        _jwtService.Verify(j => j.GenerateRefreshToken(), Times.Never);
    }
}
