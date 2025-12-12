namespace RachaConta.Core.Entities
{
    public class Invite : BaseEntity
    {
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CorpoEmail { get; set; } = string.Empty;
        public DateTime DataEnvio { get; set; }        
        public string AmigoId { get; set; } = string.Empty;        
        public bool Reenvio { get; set; }
        public DateTime? DataReenvio { get; set; }
        public bool Aceite { get; set; }       
       

        public Invite()
        {
            DataEnvio = DateTime.UtcNow;
            Reenvio = false;
            Aceite = false;
        }

        public Invite(string nome, string email, string corpoEmail, string amigoId)
        {
            Nome = nome;
            Email = email;
            CorpoEmail = corpoEmail;
            AmigoId = amigoId;           
            DataEnvio = DateTime.UtcNow;
            Reenvio = false;
            Aceite = false;
        }
    }
}
