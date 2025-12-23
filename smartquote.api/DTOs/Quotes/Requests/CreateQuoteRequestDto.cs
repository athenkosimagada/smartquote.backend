
using smartquote.api.DTOs.Items.Requests;

namespace smartquote.api.DTOs.Quotes.Requests;

public class CreateQuoteRequestDto
{
    public string CustomerName { get; set; } = string.Empty;
    public List<CreateQuoteItemRequestDto> Items { get; set; } = new();
}
