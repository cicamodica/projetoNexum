using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace nexumApp.Models
{
    public class Filial
    {
        public int Id { get; set; }
        public Ong Ong { get; set; }
        public int OngId { get; set; }
        public string UserId { get; set; }
        public string Descriçao { get; set; }
        public string Nome { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o CEP!")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "O CEP deve conter 8 números")]
        public string CEP { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o Endereço!")]
        [StringLength(300)]
        public string Endereço { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o complemento!")]
        [StringLength(100)]
        [NotMapped]
        public string Complemento { get; set; }

        [Required(ErrorMessage = "Obrigatório informar o CNPJ!")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "O CNPJ deve conter 14 números")]
        public string CNPJ { get; set; }
    }
}
