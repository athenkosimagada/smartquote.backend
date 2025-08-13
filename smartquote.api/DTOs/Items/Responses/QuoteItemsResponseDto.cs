namespace smartquote.api.DTOs.Items.Responses;

public class QuoteItemsResponseDto
{
    public bool Success { get; set; } = true;
    public List<ItemDto>? Items { get; set; }
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
}
