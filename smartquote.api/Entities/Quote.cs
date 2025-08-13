namespace smartquote.api.Entities;

public class Quote : BaseEntity
{
    public int UserId { get; set; }
    public string Customer { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public List<Item> Items { get; set; } = new List<Item>();
}
