using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using smartquote.api.Exceptions;
using System.Security.Authentication;

namespace smartquote.api.ExceptionHandlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private ILogger<ValidationExceptionHandler> _logger;
    private IProblemDetailsService _problemDetailsService;
    public GlobalExceptionHandler(
        ILogger<ValidationExceptionHandler> logger, 
        IProblemDetailsService problemDetailsService)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, 
        Exception exception, 
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Unhandled exception occurred");

        httpContext.Response.StatusCode = exception switch
        {
            AlreadyExistException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            InvalidCredentialsException => StatusCodes.Status401Unauthorized,
            AuthenticationException => StatusCodes.Status401Unauthorized,
            SecurityTokenException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Type = exception.GetType().Name,
                Title = "An error occurred",
                Detail = exception.Message,
                Status = httpContext.Response.StatusCode
            }
        });
    }
}
