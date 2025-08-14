using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using smartquote.api.DTOs.Quotes.Requests;
using smartquote.api.DTOs.Quotes.Responses;
using smartquote.api.Services;
using smartquote.api.Services.Interfaces;

namespace smartquote.api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QuotesController : ControllerBase
{
    private readonly IQuoteService _quoteService;

    public QuotesController(
        IQuoteService quoteService)
    {
        _quoteService = quoteService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllQuotes(int pageNumber = 1,  int pageSize = 10)
    {
        var response = await _quoteService.GetQuotesAsync(pageNumber, pageSize);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetQuoteById(int id)
    {
        var response = await _quoteService.GetQuoteByIdAsync(id);
        return Ok(response);
    }


    [HttpPost]
    public async Task<IActionResult> CreateQuote(CreateQuoteRequestDto request)
    {
        var response = await _quoteService.CreateQuoteAsync(request);
        return CreatedAtAction(nameof(GetQuoteById), new { id = response.QuoteId }, response);
    }
}
