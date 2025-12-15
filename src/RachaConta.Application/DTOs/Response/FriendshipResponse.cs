namespace RachaConta.Application.DTOs.Response;

public class FriendshipResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid AmigoId { get; set; }
    public bool Approved { get; set; }
    public string Convidado { get; set; } = string.Empty;
    public string ConvidadoEmail { get; set; } = string.Empty;
    public DateTime? DataAprovacao { get; set; }
}
