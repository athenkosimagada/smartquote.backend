namespace smartquote.api.DTOs.Items.Responses;

public class CreateQuoteItemResponseDto
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = "Quote item created successfully.";
}
