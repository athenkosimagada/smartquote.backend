namespace smartquote.api.DTOs.Quotes.Responses;

public class UpdateQuoteResponseDto
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = "Quote updated successfully.";
    public int QuoteId { get; set; }
}
