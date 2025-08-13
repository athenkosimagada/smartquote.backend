using smartquote.api.DTOs.Items;
using smartquote.api.DTOs.Items.Requests;
using smartquote.api.DTOs.Items.Responses;

namespace smartquote.api.Services.Interfaces;

public interface IQuoteItemService
{
    Task<CreateQuoteItemResponseDto> CreateQuoteItemAsync(CreateQuoteItemRequestDto request);
    Task<UpdateQuoteItemResponseDto> UpdateQuoteItemAsync(UpdateQuoteItemRequestDto request);
    Task<QuoteItemResponseDto> GetQuoteItemByIdAsync(int id);
    Task<QuoteItemsResponseDto> GetAllQuoteItemsAsync(int pageNumber, int pageSize);
    Task DeleteQuoteItemAsync(int id);
}
