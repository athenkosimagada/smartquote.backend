namespace smartquote.api.DTOs.Items.Responses;

public class QuoteItemResponseDto
{
    public bool Success { get; set; } = true;
    public ItemDto? Item { get; set; }
}
