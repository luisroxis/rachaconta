using System.ComponentModel.DataAnnotations;

namespace RachaConta.Application.DTOs.Request;

public class AddParticipantesGrupoRequest
{
    [Required(ErrorMessage = "A lista de IDs de participantes é obrigatória")]
    public List<Guid> IdParticipantes { get; set; } = new();

    [Required(ErrorMessage = "O ID do grupo é obrigatório")]
    public Guid GrupoId { get; set; }
}
