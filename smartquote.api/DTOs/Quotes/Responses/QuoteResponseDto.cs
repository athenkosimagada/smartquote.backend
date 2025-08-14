namespace smartquote.api.DTOs.Quotes.Responses;

public class QuoteResponseDto
{
    public bool Success { get; set; } = true;
    public QuoteDto? Quote { get; set; }
}
