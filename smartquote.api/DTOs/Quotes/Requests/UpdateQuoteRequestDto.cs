using smartquote.api.DTOs.Items.Requests;

namespace smartquote.api.DTOs.Quotes.Requests;

public class UpdateQuoteRequestDto
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public List<UpdateQuoteItemRequestDto> Items { get; set; } = new();
}
