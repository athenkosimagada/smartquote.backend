namespace smartquote.api.DTOs.Quotes.Responses;

public class CreateQuoteResponseDto
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = "Quote created successfully.";
    public int QuoteId { get; set; }
}
