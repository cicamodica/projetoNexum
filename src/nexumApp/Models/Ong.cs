using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nexumApp.Models
{
    public class Ong
    {
        public int Id { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
        [Required(ErrorMessage = "Obrigatório informar a Razão Social!")]
        [Display(Name = "Razão Social")]
        [StringLength(50)]
        public string Nome { get; set; }
        [Required(ErrorMessage = "Obrigatório informar a Descrição!")]
        [Display(Name = "Descrição de atividades")]
        [StringLength(300)]
        public string Descriçao { get; set; }
        [Required(ErrorMessage = "Obrigatório informar o Endereço!")]
        [StringLength(300)]
        public string Endereço { get; set; }
        [Required(ErrorMessage = "Obrigatório informar o CNPJ!")]
        [StringLength(14, MinimumLength = 14)] 
        public string CNPJ { get; set; }
        public bool Aprovaçao { get; set; } = false;
        public ICollection<Filial> Filials { get; set; }

        // ====== Campos do PDF ======

        [Display(Name = "Nome do arquivo PDF")]
        public string DocumentoNome { get; set; }

        [Display(Name = "Tipo do arquivo")]
        public string DocumentoTipo { get; set; }

        [Display(Name = "Arquivo PDF (dados)")]
        public byte[] DocumentoDados { get; set; }

        [NotMapped]
        //[Required(ErrorMessage = "Anexe o documento PDF para aprovação")]
        //[Display(Name = "Documento PDF para aprovação")]
        public IFormFile DocumentoPdf { get; set; }
    }
    public class OngEditViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Razão Social")]
        [StringLength(50)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Obrigatório informar a Descrição!")]
        [Display(Name = "Descrição de atividades")]
        [StringLength(300)]
        public string Descriçao { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o Endereço!")]
        [StringLength(300)]
        public string Endereço { get; set; }
    }
}
