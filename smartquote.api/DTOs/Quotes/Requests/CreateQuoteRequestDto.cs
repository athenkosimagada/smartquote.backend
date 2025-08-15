namespace smartquote.api.DTOs.Quotes.Requests;

public class CreateQuoteRequestDto
{
    public string UserId { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public decimal Total { get; set; } = 0;
}
