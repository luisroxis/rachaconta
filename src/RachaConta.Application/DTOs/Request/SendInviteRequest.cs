namespace RachaConta.Application.DTOs.Request;

public class SendInviteRequest
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CorpoEmail { get; set; } = string.Empty;
    public string AmigoId { get; set; } = string.Empty;
}
