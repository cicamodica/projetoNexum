using System.ComponentModel.DataAnnotations;

namespace nexumApp.Models
{
    public class Meta
    {
        [Key]
        public int Id { get; set; }
        public string Status { get; set; }
        public int IdRecurso { get; set; } 
        public string Recurso { get; set; }
        public int QuantidadeNecessaria { get; set; }
        public int QuantidadeReservada { get; set; }
        public int QuantidadeDisponivel { get; set; }

        // Uma Meta pode ter várias Doações.
        // Isso representa o relacionamento (0..*) do diagrama.
        public virtual ICollection<Doacao> Doacoes { get; set; }
    }
}
