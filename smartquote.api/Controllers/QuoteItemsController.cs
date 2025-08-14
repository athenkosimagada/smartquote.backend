using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using smartquote.api.DTOs.Items.Requests;
using smartquote.api.Services.Interfaces;

namespace smartquote.api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class QuoteItemsController : ControllerBase
{
    private readonly IQuoteItemService _quoteItemService;
    public QuoteItemsController(IQuoteItemService quoteItemService)
    {
        _quoteItemService = quoteItemService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllQuoteItems(int pageNumber, int pageSize)
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

        var response = await _quoteItemService.GetAllQuoteItemsAsync(pageNumber, pageSize);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetQuoteItemById(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                Success = false,
                Message = "Item ID must be greater than 0."
            });
        }

        var response = await _quoteItemService.GetQuoteItemByIdAsync(id);
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> CreateQuoteItem([FromBody] CreateQuoteItemRequestDto request)
    {
        var response = await _quoteItemService.CreateQuoteItemAsync(request);
        return CreatedAtAction(nameof(GetQuoteItemById), new { id = response.ItemId }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQuoteItem(int id, [FromBody] UpdateQuoteItemRequestDto request)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                Success = false,
                Message = "Item ID must be greater than 0."
            });
        }

        if (id != request.Id)
        {
            return BadRequest(new
            {
                Success = false,
                Message = "Item ID in the URL does not match the ID in the request body."
            });
        }

        var response = await _quoteItemService.UpdateQuoteItemAsync(request);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuoteItem(int id)
    {
        if (id <= 0)
        {
            return BadRequest(new
            {
                Success = false,
                Message = "Item ID must be greater than 0."
            });
        }

        await _quoteItemService.DeleteQuoteItemAsync(id);
        return NoContent();
    }
}
