namespace smartquote.api.DTOs.Auth.Responses;

public class RegisterResponseDto
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = "User registered successfully.";
}
