using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nexumApp.Models
{
    public class Meta
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O status é obrigatório")]
        public string Status { get; set; }

        [Required(ErrorMessage = "A descrição é obrigatória")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "O recurso é obrigatório")]
        public string Recurso { get; set; }

        [Required(ErrorMessage = "O valor alvo é obrigatório")]
        public int ValorAlvo { get; set; }
        public int QuantidadeReservada { get; set; }
        public int ValorAtual { get; set; }

        [Display(Name = "Imagem da Meta")]
        public string ImagemUrl { get; set; } // Armazena o CAMINHO da imagem (ex: /uploads/metas/imagem.jpg)

        [Display(Name = "Data de Encerramento")]
        [DataType(DataType.Date)]
        public DateTime? DataFim { get; set; } // O '?' torna a data opcional (nullable)
        public bool Ativa { get; set; } = true;

        // Chave Estrangeira para a ONG
        public int OngId { get; set; }

        [ForeignKey("OngId")]
        public virtual Ong Ong { get; set; } // Propriedade de navegação

        public ICollection<Doacao> Doacoes { get; set; } = new List<Doacao>();

        public int? FilialId { get; set; }

        public Filial Filial { get; set; }
    }
}
