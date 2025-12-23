using smartquote.api.DTOs.Quotes.Requests;
using smartquote.api.DTOs.Quotes.Responses;

namespace smartquote.api.Services.Interfaces;

public interface IQuoteService
{
    Task<QuoteResponseDto> GetQuoteByIdAsync(int id);
    Task<QuotesResponseDto> GetQuotesAsync(int pageNumber = 1, int pageSize = 10);
    Task<CreateQuoteResponseDto> CreateQuoteAsync(string userId, CreateQuoteRequestDto request);
    Task<UpdateQuoteResponseDto> UpdateQuoteAsync(string userId, UpdateQuoteRequestDto request);
    Task DeleteQuoteAsync(int id);
}
