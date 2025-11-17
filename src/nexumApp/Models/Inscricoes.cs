using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nexumApp.Models
{
    public class Inscricoes
    {
        [Key]
        public int Id { get; set; }

       
        [Display(Name = "Vaga")]
        public int IdVaga { get; set; }

        [ForeignKey("IdVaga")]
        public virtual Vaga Vaga { get; set; }

       
        [Display(Name = "Candidato")]
        public int IdCandidato { get; set; }

        [ForeignKey("IdCandidato")]
        public virtual Candidato Candidato { get; set; } 

       

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataInscricao { get; set; }

       
        [StringLength(50)]
        public string Status { get; set; } 
    }
}
