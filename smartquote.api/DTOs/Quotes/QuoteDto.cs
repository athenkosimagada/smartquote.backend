using smartquote.api.Entities;

namespace smartquote.api.DTOs.Quotes;

public class QuoteDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public List<Item> Items { get; set; } = new List<Item>();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
