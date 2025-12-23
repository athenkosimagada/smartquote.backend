using smartquote.api.DTOs.Items;
using smartquote.api.Entities;

namespace smartquote.api.DTOs.Quotes;

public class QuoteDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public List<ItemDto> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
