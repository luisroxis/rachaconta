using System.ComponentModel.DataAnnotations;

namespace RachaConta.Application.DTOs.Request;

public class PromoteParticipanteRequest
{
    [Required(ErrorMessage = "O ID do grupo é obrigatório")]
    public Guid GrupoId { get; set; }

    [Required(ErrorMessage = "O ID do participante é obrigatório")]
    public Guid ParticipanteId { get; set; }
}
