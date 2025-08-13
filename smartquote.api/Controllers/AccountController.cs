using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using smartquote.api.DTOs.Account;
using smartquote.api.DTOs.Account.Responses;
using smartquote.api.Services.Interfaces;
using System.Security.Claims;

namespace smartquote.api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountService _authService;
    private readonly IValidator<RegisterRequestDto> _registerValidator;
    private readonly IValidator<LoginRequestDto> _loginValidator;

    public AccountController(
        IAccountService authService, 
        IValidator<RegisterRequestDto> registerValidator,
        IValidator<LoginRequestDto> loginValidator)
    {
        _authService = authService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponseDto>> Register(RegisterRequestDto request)
    {
        _registerValidator.ValidateAndThrow(request);
        var response = await _authService.RegisterAsync(request);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
    {
        _loginValidator.ValidateAndThrow(request);
        var response = await _authService.LoginAsync(request);
        return Ok(response);
    }

    [HttpGet("details")]
    [Authorize]
    public async Task<ActionResult<AccountDetailsResponseDto>> GetUserDetails()
    {
        var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized(new { message = "Unauthorized" });
        }

        var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(userEmail))
        {
            return Unauthorized(new { message = "Unauthorized" });
        }

        var response = await _authService.GetAccountDetailsAsync(userEmail);
        return Ok(response);
    }
}
