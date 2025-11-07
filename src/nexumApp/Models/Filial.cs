using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "Obrigatório informar o Endereço!")]
        [StringLength(300)]
        public string Endereço { get; set; }
        [Required(ErrorMessage = "Obrigatório informar o CNPJ!")]
        [StringLength(14, MinimumLength = 14)]
        public string CNPJ { get; set; }
    }
}
