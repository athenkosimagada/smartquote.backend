namespace smartquote.api.DTOs.Items.Responses;

public class UpdateQuoteItemResponseDto
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = "Quote item updated successfully.";
    public int ItemId { get; set; }
}
