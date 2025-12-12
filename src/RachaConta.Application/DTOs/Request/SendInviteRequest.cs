using System.ComponentModel.DataAnnotations;

namespace RachaConta.Application.DTOs.Request;

public class SendInviteRequest
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "O email é obrigatório")]
    [EmailAddress(ErrorMessage = "O email é inválido")]
    public string Email { get; set; } = string.Empty;
    
    public string? CorpoEmail { get; set; } = string.Empty;    
}
