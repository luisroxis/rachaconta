namespace RachaConta.Application.DTOs.Response;

public class ParticipanteGrupoResponse
{
    public Guid Id { get; set; }
    public Guid IdGrupo { get; set; }
    public Guid UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public bool IsAdm { get; set; }
    public bool HasPendent { get; set; }
    public DateTime CreatedAt { get; set; }
}
