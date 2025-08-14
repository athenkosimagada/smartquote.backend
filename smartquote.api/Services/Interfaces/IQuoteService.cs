using smartquote.api.DTOs.Quotes.Requests;
using smartquote.api.DTOs.Quotes.Responses;

namespace smartquote.api.Services.Interfaces;

public interface IQuoteService
{
    Task<QuoteResponseDto> GetQuoteByIdAsync(int id);
    Task<QuotesResponseDto> GetQuotesAsync(int pageNumber, int pageSize);
    Task<CreateQuoteResponseDto> CreateQuoteAsync(CreateQuoteRequestDto request);
    Task<UpdateQuoteResponseDto> UpdateQuoteAsync(UpdateQuoteRequestDto request);
    Task DeleteQuoteAsync(int id);
}
