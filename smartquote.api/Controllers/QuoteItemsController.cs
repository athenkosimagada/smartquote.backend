using FluentValidation;
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
    private readonly IValidator<CreateQuoteItemRequestDto> _createQuoteItemValidator;
    private readonly IValidator<UpdateQuoteItemRequestDto> _updateQuoteItemValidator;
    public QuoteItemsController(
        IQuoteItemService quoteItemService,
        IValidator<CreateQuoteItemRequestDto> createQuoteItemValidator,
        IValidator<UpdateQuoteItemRequestDto> updateQuoteItemValidator)
    {
        _quoteItemService = quoteItemService;
        _createQuoteItemValidator = createQuoteItemValidator;
        _updateQuoteItemValidator = updateQuoteItemValidator;
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
        await _createQuoteItemValidator.ValidateAndThrowAsync(request);

        var response = await _quoteItemService.CreateQuoteItemAsync(request);
        return CreatedAtAction(nameof(GetQuoteItemById), new { id = response.ItemId }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQuoteItem(int id, [FromBody] UpdateQuoteItemRequestDto request)
    {
        await _updateQuoteItemValidator.ValidateAndThrowAsync(request);

        if (id <= 0)
        {
            return BadRequest(new
            {
                Success = false,
                Message = "Item ID must be greater than 0."
            });
        }

        request.Id = id;

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
