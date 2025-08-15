using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using smartquote.api.DTOs.Quotes.Requests;
using smartquote.api.DTOs.Quotes.Responses;
using smartquote.api.Services;
using smartquote.api.Services.Interfaces;

namespace smartquote.api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class QuotesController : ControllerBase
{
    private readonly IQuoteService _quoteService;
    private readonly IValidator<CreateQuoteRequestDto> _createQuoteValidator;

    public QuotesController(
        IQuoteService quoteService,
        IValidator<CreateQuoteRequestDto> createQuoteValidator)
    {
        _quoteService = quoteService;
        _createQuoteValidator = createQuoteValidator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllQuotes(int pageNumber = 1,  int pageSize = 10)
    {
        if (pageNumber < 1)
        {
            return BadRequest(new
            {
                Success = false,
                Message = "Page number must be greater than or equal to 1."
            });
        }

        if (pageSize < 1 || pageSize > 100)
        {
            return BadRequest(new
            {
                Success = false,
                Message = "Page size must be between 1 and 100."
            });
        }

        var response = await _quoteService.GetQuotesAsync(pageNumber, pageSize);
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
    public async Task<IActionResult> CreateQuote(CreateQuoteRequestDto request)
    {
        await _createQuoteValidator.ValidateAndThrowAsync(request);

        var response = await _quoteService.CreateQuoteAsync(request);
        return CreatedAtAction(nameof(GetQuoteById), new { id = response.QuoteId }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQuote(int id, UpdateQuoteRequestDto request)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                Success = false,
                Message = "Quote ID must be greater than 0."
            });
        }

        if (id != request.Id)
        {
            return BadRequest(new
            {
                Success = false,
                Message = "Quote ID in the URL does not match the ID in the request body."
            });
        }

        var response = await _quoteService.UpdateQuoteAsync(request);
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
