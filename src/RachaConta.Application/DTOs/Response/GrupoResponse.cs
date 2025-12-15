namespace RachaConta.Application.DTOs.Response;

public class GrupoResponse
{
    public Guid Id { get; set; }
    public string IdCategoria { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string? OutrasCategorias { get; set; }
    public string? LinkConvite { get; set; }
    public bool Ativo { get; set; }
    public bool Deleted { get; set; }
    public string? ImgGrupo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
