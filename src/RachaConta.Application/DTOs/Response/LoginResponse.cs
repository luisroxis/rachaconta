namespace RachaConta.Application.DTOs.Response;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public int ExpiresIn { get; set; } = 3600; // 1 hour in seconds
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string? Avatar { get; set; }
    public string Cod { get; set; } = string.Empty; // Encrypted userId
}
