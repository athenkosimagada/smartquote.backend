namespace smartquote.api.DTOs.Quotes.Requests;

public class UpdateQuoteRequestDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public decimal Total { get; set; }
}
