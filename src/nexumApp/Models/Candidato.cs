using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nexumApp.Models
{
    [Table("Candidato")]
    public class Candidato
    {
        [StringLength(3)]
        [Key]
        public int Id { get; set; }


        [StringLength(45)]
        [Required(ErrorMessage = "Obrigatorio informar o nome")]
        public string Nome { get; set; } 
        [StringLength(90)]
        [Required(ErrorMessage = "Obrigatorio informar o email")]
        public string Email { get; set; } 
        [StringLength(11)]
        [Required(ErrorMessage = "Obrigatorio informar o cpf")]
        public string CPF { get; set; } 
        [StringLength(11)]
        [Required(ErrorMessage = "Obrigatorio informar o telefone")]
        public string Telefone { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Data de Inscrição")]
        public DateTime DataInscricao { get; set; }
        [Display(Name = "Tem Experiência?")]
        public bool TemExperiencia { get; set; }
        public string Descricao { get; set; }
        [Display(Name = "Foto do Candidato")]
        public string FotoUrl { get; set; }

        public ICollection<Inscricoes> Inscricoes { get; set; }
    }
}
