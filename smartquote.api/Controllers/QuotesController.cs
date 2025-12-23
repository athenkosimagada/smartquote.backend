using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using smartquote.api.DTOs.Quotes.Requests;
using smartquote.api.DTOs.Quotes.Responses;
using smartquote.api.Services;
using smartquote.api.Services.Interfaces;
using System.Security.Claims;

namespace smartquote.api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class QuotesController : ControllerBase
{
    private readonly IQuoteService _quoteService;
    private readonly IValidator<CreateQuoteRequestDto> _createQuoteValidator;
    private readonly IValidator<UpdateQuoteRequestDto> _updateQuoteValidator;

    public QuotesController(
        IQuoteService quoteService,
        IValidator<CreateQuoteRequestDto> createQuoteValidator,
        IValidator<UpdateQuoteRequestDto> updateQuoteRequestDto)
    {
        _quoteService = quoteService;
        _createQuoteValidator = createQuoteValidator;
        _updateQuoteValidator = updateQuoteRequestDto;
    }

    [HttpGet]
    public async Task<IActionResult> GetQuotes()
    {
        var response = await _quoteService.GetQuotesAsync();
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetQuoteById(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                Success = false,
                Message = "Quote ID must be greater than 0."
            });
        }

        var response = await _quoteService.GetQuoteByIdAsync(id);
        return Ok(response);
    }


    [HttpPost]
    public async Task<IActionResult> CreateQuote([FromBody] CreateQuoteRequestDto request)
    {
        await _createQuoteValidator.ValidateAndThrowAsync(request);

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new
            {
                Success = false,
                Message = "Unauthorized"
            });
        }

        var response = await _quoteService.CreateQuoteAsync(userId, request);
        return CreatedAtAction(nameof(GetQuoteById), new { id = response.QuoteId }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQuote(int id, [FromBody] UpdateQuoteRequestDto request)
    {
        await _updateQuoteValidator.ValidateAndThrowAsync(request);

        if (id <= 0)
        {
            return BadRequest(new
            {
                Success = false,
                Message = "Quote ID must be greater than 0."
            });
        }

        request.Id = id;

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new
            {
                Success = false,
                Message = "Unauthorized"
            });
        }

        var response = await _quoteService.UpdateQuoteAsync(userId, request);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuote(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                Success = false,
                Message = "Quote ID must be greater than 0."
            });
        }
        await _quoteService.DeleteQuoteAsync(id);
        return NoContent();
    }
}
