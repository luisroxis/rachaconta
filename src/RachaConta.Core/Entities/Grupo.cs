namespace RachaConta.Core.Entities
{
    public class Grupo : BaseEntity
    {
        public string IdCategoria { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string? OutrasCategorias { get; set; }
        public string? LinkConvite { get; set; }
        public bool Ativo { get; set; } = true;
        public bool Deleted { get; set; } = false;
        public string? ImgGrupo { get; set; }

        public Grupo()
        {
        }

        public Grupo(string idCategoria, string nome, string descricao = "")
        {
            IdCategoria = idCategoria;
            Nome = nome;
            Descricao = descricao;
            Ativo = true;
            Deleted = false;
        }
    }
}
