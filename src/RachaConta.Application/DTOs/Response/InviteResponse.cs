namespace RachaConta.Application.DTOs.Response;

public class InviteResponse
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CorpoEmail { get; set; } = string.Empty;
    public DateTime DataEnvio { get; set; }
    public string AmigoId { get; set; } = string.Empty;
    public bool Reenvio { get; set; }
    public DateTime? DataReenvio { get; set; }
    public bool Aceite { get; set; }
}
