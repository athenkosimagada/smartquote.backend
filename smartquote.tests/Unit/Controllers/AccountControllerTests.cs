using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using smartquote.api.Controllers;
using smartquote.api.DTOs.Account;
using smartquote.api.DTOs.Account.Responses;
using smartquote.api.Services.Interfaces;
using smartquote.api.Validators;

namespace smartquote.tests.Unit.Controllers;

public class AccountControllerTests
{
    private readonly Mock<IAccountService> _accountService;
    private readonly IValidator<RegisterRequestDto> _registerValidator;
    private readonly IValidator<LoginRequestDto> _loginValidator;
    private readonly IValidator<RefreshTokenRequestDto> _refreshTokenValidator;
    private readonly IValidator<ResendConfirmationEmailRequestDto> _resendConfirmationEmailValidator;
    private readonly IValidator<ForgotPasswordRequestDto> _forgetPasswordValidator;
    private readonly IValidator<ResetPasswordRequestDto> _resetPasswordValidator;
    private readonly IValidator<ChangePasswordRequestDto> _changePasswordValidator;
    private readonly IValidator<LogoutRequestDto> _logoutValidator;

    private readonly AccountController _accountController;

    public AccountControllerTests()
    {
        _accountService = new Mock<IAccountService>();
        _registerValidator = new RegisterRequestDtoValidator();
        _loginValidator = new LoginRequestDtoValidator();
        _refreshTokenValidator = new RefreshTokenRequestDtoValidator();
        _resendConfirmationEmailValidator = new ResendConfirmationEmailRequestDtoValidator();
        _forgetPasswordValidator = new ForgotPasswordRequestDtoValidator();
        _resetPasswordValidator = new ResetPasswordRequestDtoValidator();
        _changePasswordValidator = new ChangePasswordRequestDtoValidator();
        _logoutValidator = new LogoutRequestDtoValidator();


        _accountController = new AccountController(
            _accountService.Object,
            _registerValidator,
            _loginValidator,
            _refreshTokenValidator,
            _resendConfirmationEmailValidator,
            _forgetPasswordValidator,
            _resetPasswordValidator,
            _changePasswordValidator,
            _logoutValidator);
    }

    [Fact]
    public async Task Register_ShouldReturnOkWithRegisterResponseDto_WhenRequestIsValid()
    {
        // Arrange
        RegisterRequestDto request = new()
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "johndoe@example.com",
            Password = "Password123!"
        };

        _accountService
            .Setup(x => x.RegisterAsync(request))
            .ReturnsAsync(new RegisterResponseDto());

        // Act
        var result = await _accountController.Register(request);

        // Assert
        result.Should().NotBeNull();

        result.Result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();

        (result.Result as OkObjectResult)!.Value.Should().NotBeNull();
        (result.Result as OkObjectResult)!.Value.Should().BeOfType<RegisterResponseDto>();
        (result.Result as OkObjectResult)!.Value.Should().BeEquivalentTo(new RegisterResponseDto());
        (result.Result as OkObjectResult)!.StatusCode.Should().Be(StatusCodes.Status200OK);

        _accountService.Verify(x => x.RegisterAsync(request), Times.Once);
    }

    [Fact]
    public async Task Register_ShouldThrowValidationException_WhenRequestIsInvalid()
    {
        // Arrange
        RegisterRequestDto request = new()
        {
            FirstName = "",
            LastName = "Doe",
            Email = "invalid-email",
            Password = "short"
        };

        // Act
        Func<Task> act = async () => await _accountController.Register(request);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Register_ShouldThrowException_WhenInternalErrorOccurs()
    {
        // Arrange
        RegisterRequestDto request = new()
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "johndoe@example.com",
            Password = "Password123!"
        };

        _accountService
            .Setup(x => x.RegisterAsync(request))
            .ThrowsAsync(new Exception("Internal server error"));

        // Act
        Func<Task> act = async () => await _accountController.Register(request);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Internal server error");
    }
}
