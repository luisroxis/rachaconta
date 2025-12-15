namespace RachaConta.Core.Entities;

public class Friendship : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid AmigoId { get; set; }
    public bool Approved { get; set; }
    public string Convidado { get; set; } = string.Empty;
    public string ConvidadoEmail { get; set; } = string.Empty;
    public DateTime? DataAprovacao { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual User Amigo { get; set; } = null!;

    public Friendship()
    {
        Approved = false;
    }

    public Friendship(Guid userId, Guid amigoId, string convidado, string convidadoEmail)
    {
        UserId = userId;
        AmigoId = amigoId;
        Convidado = convidado;
        ConvidadoEmail = convidadoEmail;
        Approved = false;
    }
}
