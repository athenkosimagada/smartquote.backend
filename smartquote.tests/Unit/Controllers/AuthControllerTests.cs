using FluentAssertions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using smartquote.api.Controllers;
using smartquote.api.DTOs.Auth;
using smartquote.api.DTOs.Auth.Responses;
using smartquote.api.Services.Interfaces;
using smartquote.api.Validators;

namespace smartquote.tests.Unit.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authService;
    private readonly IValidator<RegisterRequestDto> _registerValidator;
    private readonly IValidator<LoginRequestDto> _loginValidator;

    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        _authService = new Mock<IAuthService>();
        _registerValidator = new RegisterRequestDtoValidator();
        _loginValidator = new LoginRequestDtoValidator();

        _authController = new AuthController(
            _authService.Object,
            _registerValidator,
            _loginValidator);
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

        _authService
            .Setup(x => x.RegisterAsync(request))
            .ReturnsAsync(new RegisterResponseDto());

        // Act
        var result = await _authController.Register(request);

        // Assert
        result.Should().NotBeNull();

        result.Result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();

        (result.Result as OkObjectResult)!.Value.Should().NotBeNull();
        (result.Result as OkObjectResult)!.Value.Should().BeOfType<RegisterResponseDto>();
        (result.Result as OkObjectResult)!.Value.Should().BeEquivalentTo(new RegisterResponseDto());
        (result.Result as OkObjectResult)!.StatusCode.Should().Be(StatusCodes.Status200OK);

        _authService.Verify(x => x.RegisterAsync(request), Times.Once);
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
        Func<Task> act = async () => await _authController.Register(request);

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

        _authService
            .Setup(x => x.RegisterAsync(request))
            .ThrowsAsync(new Exception("Internal server error"));

        // Act
        Func<Task> act = async () => await _authController.Register(request);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Internal server error");
    }
}
