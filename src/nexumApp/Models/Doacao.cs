namespace nexumApp.Models
{
    public class Doacao
    {
        public int IdDoacao { get; set; }
        public string Tipo { get; set; }
        public DateTime Data { get; set; }
        public string Status { get; set; }
        public int Quantidade { get; set; }
        public string NomeRazaoSocial { get; set; }
        public string Email { get; set; }
        public int IdOngDestinataria { get; set; }
        public int IdOngDoadora { get; set; }

        // Chave Estrangeira e Propriedade de Navegação:
        // Isso representa o relacionamento com a Meta.
        // Uma Doação pertence a exatamente uma Meta.
        public int IdMeta { get; set; }
        public virtual Meta Meta { get; set; }
    }
}
