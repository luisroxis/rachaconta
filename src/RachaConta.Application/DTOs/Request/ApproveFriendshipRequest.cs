using System.ComponentModel.DataAnnotations;

namespace RachaConta.Application.DTOs.Request;

public class ApproveFriendshipRequest
{
    [Required(ErrorMessage = "A aprovação (true/false) é obrigatória")]
    public bool Approved { get; set; }
}
