namespace RachaConta.Application.DTOs;

public class RegisterUserRequest
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool TermOfUse { get; set; }
    public bool PrivacyPolicy { get; set; }
}
