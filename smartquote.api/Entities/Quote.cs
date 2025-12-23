namespace smartquote.api.Entities;

public class Quote : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal Total { get; set; } = 0;
    public List<Item> Items { get; set; } = new();
}
