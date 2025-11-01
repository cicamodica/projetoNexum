using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nexumApp.Models
{
    public class Meta
    {
        [Key]
        public int Id { get; set; }
        public string Status { get; set; }
        public string Descricao { get; set; }
        public string Recurso { get; set; }
        public int ValorAlvo { get; set; }
        public int QuantidadeReservada { get; set; }
        public int ValorAtual { get; set; }

        // Chave Estrangeira para a ONG
        public int OngId { get; set; }

        [ForeignKey("OngId")]
        public virtual Ong Ong { get; set; } // Propriedade de navegação
    }
}
