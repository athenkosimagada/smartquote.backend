namespace smartquote.api.DTOs.Quotes.Responses;

public class QuotesResponseDto
{
    public bool Success { get; set; } = true;
    public List<QuoteDto>? Quotes { get; set; }
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}
