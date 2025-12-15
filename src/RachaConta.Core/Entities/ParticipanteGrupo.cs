namespace RachaConta.Core.Entities
{
    public class ParticipanteGrupo : BaseEntity
    {
        public Guid IdGrupo { get; set; }
        public Guid UserId { get; set; }
        public bool IsAdm { get; set; } = false;
        public bool HasPendent { get; set; } = false;

        public ParticipanteGrupo()
        {
        }

        public ParticipanteGrupo(Guid idGrupo, Guid userId, bool isAdm = false)
        {
            IdGrupo = idGrupo;
            UserId = userId;
            IsAdm = isAdm;
            HasPendent = false;
        }
    }
}
