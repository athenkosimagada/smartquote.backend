namespace smartquote.api.DTOs.Items;

public class ItemDto
{
    public int Id { get; set; }
    public int QuoteId { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
