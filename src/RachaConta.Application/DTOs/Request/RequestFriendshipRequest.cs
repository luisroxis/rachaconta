using System.ComponentModel.DataAnnotations;

namespace RachaConta.Application.DTOs.Request;

public class RequestFriendshipRequest
{
    [Required(ErrorMessage = "O ID do amigo é obrigatório")]
    public Guid AmigoId { get; set; }
}
