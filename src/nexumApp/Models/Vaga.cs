using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nexumApp.Models
{
    [Table("Vaga")]
    public class Vaga
    {
        [Key]
        public int IdVaga { get; set; }

        [Required]
        public int IdONG { get; set; }

        [ForeignKey("IdONG")]
        public virtual Ong Ong { get; set; }

        public bool Ativa { get; set; } = true;

        [Required]
        [StringLength(90)]
        public string Titulo { get; set; }

        [Required]
        [StringLength(500)]
        public string Descricao { get; set; }

        [Display(Name = "Data de Início")]
        [DataType(DataType.Date)] 
        public DateTime DataInicio { get; set; }

        [Display(Name = "Data Final")]
        [DataType(DataType.Date)]
        public DateTime DataFim { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        public string ImagemUrl { get; set; }

        public ICollection<Inscricoes> Inscricoes { get; set; }
    }
}
