using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using smartquote.api.DTOs.Account;
using smartquote.api.DTOs.Account.Responses;
using smartquote.api.Services.Interfaces;
using smartquote.api.Validators;
using System.Security.Claims;

namespace smartquote.api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IValidator<RegisterRequestDto> _registerValidator;
    private readonly IValidator<LoginRequestDto> _loginValidator;
    private readonly IValidator<RefreshTokenRequestDto> _refreshTokenValidator;
    private readonly IValidator<ResendConfirmationEmailRequestDto> _resendConfirmationEmailValidator;
    private readonly IValidator<ForgotPasswordRequestDto> _forgotPasswordValidator;
    private readonly IValidator<ResetPasswordRequestDto> _resetPasswordValidator;
    private readonly IValidator<ChangePasswordRequestDto> _changePasswordValidator;
    private readonly IValidator<LogoutRequestDto> _logoutValidator;

    public AccountController(
        IAccountService authService, 
        IValidator<RegisterRequestDto> registerValidator,
        IValidator<LoginRequestDto> loginValidator,
        IValidator<RefreshTokenRequestDto> refreshTokenValidator,
        IValidator<ResendConfirmationEmailRequestDto> resendConfirmationEmailValidator,
        IValidator<ForgotPasswordRequestDto> forgotPasswordValidator,
        IValidator<ResetPasswordRequestDto> resetPasswordValidator,
        IValidator<ChangePasswordRequestDto> changePasswordValidator,
        IValidator<LogoutRequestDto> logoutValidator)
    {
        _accountService = authService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
        _refreshTokenValidator = refreshTokenValidator;
        _resendConfirmationEmailValidator = resendConfirmationEmailValidator;
        _forgotPasswordValidator = forgotPasswordValidator;
        _resetPasswordValidator = resetPasswordValidator;
        _changePasswordValidator = changePasswordValidator;
        _logoutValidator = logoutValidator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponseDto>> Register(RegisterRequestDto request)
    {
        await _registerValidator.ValidateAndThrowAsync(request);

        var response = await _accountService.RegisterAsync(request);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
    {
        await _loginValidator.ValidateAndThrowAsync(request);

        var response = await _accountService.LoginAsync(request);
        return Ok(response);
    }

    [HttpPost("refreshToken")]
    public async Task<ActionResult<RefreshTokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
    {
        await _refreshTokenValidator.ValidateAndThrowAsync(request);

        var response = await _accountService.RefreshTokenAsync(request);
        return Ok(response);
    }

    [HttpPost("resendConfirmationEmail")]
    public async Task<ActionResult<ResendConfirmationEmailResponseDto>> ResendConfirmationEmail(ResendConfirmationEmailRequestDto request)
    {
        await _resendConfirmationEmailValidator.ValidateAndThrowAsync(request);

        var response = await _accountService.ResendConfirmationEmailAsync(request);
        return Ok(response);
    }

    [HttpPost("confirmEmail")]
    public async Task<ActionResult<ConfirmEmailResponseDto>> ConfirmEmail(ConfirmEmailRequestDto result)
    {
        var response = await _accountService.ConfirmEmailAsync(result);
        return Ok(response);
    }

    [HttpPost("forgotPassword")]
    public async Task<ActionResult<ForgotPasswordResponseDto>> ForgotPassword(ForgotPasswordRequestDto request)
    {
        await _forgotPasswordValidator.ValidateAndThrowAsync(request);

        var response = await _accountService.ForgotPasswordAsync(request);
        return Ok(response);
    }

    [HttpPost("resetPassword")]
    public async Task<ActionResult<ResetPasswordResponseDto>> ResetPassword(ResetPasswordRequestDto request)
    {
        await _resetPasswordValidator.ValidateAndThrowAsync(request);

        var response = await _accountService.ResetPasswordAsync(request);
        return Ok(response);
    }

    [HttpPost("changePassword")]
    [Authorize]
    public async Task<ActionResult<ChangePasswordResponseDto>> ChangePassword(ChangePasswordRequestDto request)
    {
        var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized(new
            {
                Success = false,
                Message = "Unauthorized"
            });
        }

        var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(userEmail))
        {
            return Unauthorized(new
            {
                Success = false,
                Message = "Unauthorized"
            });
        }

        await _changePasswordValidator.ValidateAndThrowAsync(request);

        var response = await _accountService.ChangePasswordAsync(userEmail, request);
        return Ok(response);
    }

    [HttpGet("manage/info")]
    [Authorize]
    public async Task<ActionResult<AccountInfoResponseDto>> GetUserInfor()
    {
        var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(accessToken))
        {
            return Unauthorized(new
            {
                Success = false,
                Message = "Unauthorized"
            });
        }

        var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;

        if (string.IsNullOrEmpty(userEmail))
        {
            return Unauthorized(new
            {
                Success = false,
                Message = "Unauthorized"
            });
        }

        var response = await _accountService.GetAccountDetailsAsync(userEmail);
        return Ok(response);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<LogoutResponseDto>> Logout(LogoutRequestDto request)
    {
        await _logoutValidator.ValidateAndThrowAsync(request);

        var response = await _accountService.LogoutAsync(request);
        return Ok(response);
    }
}
