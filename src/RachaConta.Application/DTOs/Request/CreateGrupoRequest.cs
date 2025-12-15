using System.ComponentModel.DataAnnotations;

namespace RachaConta.Application.DTOs.Request;

public class CreateGrupoRequest
{
    [Required(ErrorMessage = "O ID da categoria é obrigatório")]
    public string IdCategoria { get; set; } = string.Empty;

    [Required(ErrorMessage = "O nome do grupo é obrigatório")]
    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    public List<Guid>? Participantes { get; set; }
}
