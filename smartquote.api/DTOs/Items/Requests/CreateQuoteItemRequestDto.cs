namespace smartquote.api.DTOs.Items.Requests;

public class CreateQuoteItemRequestDto
{
    public int QuoteId { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
