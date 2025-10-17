using System.ComponentModel.DataAnnotations;

namespace nexumApp.Models
{
    public class VoluntarioModel
    {
        [Key]
        public int IdFormulario { get; set; }

        public string Nome { get; set; }
        public string Email { get; set; }
        public string CPF { get; set; }
        public string Telefone { get; set; }
        public DateTime DataAprovacao { get; set; }
    }
}
