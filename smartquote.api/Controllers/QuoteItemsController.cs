using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using smartquote.api.DTOs.Items.Requests;
using smartquote.api.Services.Interfaces;

namespace smartquote.api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class QuoteItemsController : ControllerBase
{
    private readonly IQuoteItemService _quoteItemService;
    public QuoteItemsController(IQuoteItemService quoteItemService)
    {
        _quoteItemService = quoteItemService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAllQuoteItems(int pageNumber = 1, int pageSize = 10)
    {
        var response = await _quoteItemService.GetAllQuoteItemsAsync(pageNumber, pageSize);
        return Ok(response);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetQuoteItemById(int id)
    {
        var response = await _quoteItemService.GetQuoteItemByIdAsync(id);
        return Ok(response);
    }
    [HttpPost]
    public async Task<IActionResult> CreateQuoteItem([FromBody] CreateQuoteItemRequestDto request)
    {
        var response = await _quoteItemService.CreateQuoteItemAsync(request);
        return CreatedAtAction(nameof(GetQuoteItemById), new { id = response.ItemId }, response);
    }
}
