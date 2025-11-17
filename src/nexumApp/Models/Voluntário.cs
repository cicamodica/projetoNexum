using System.ComponentModel.DataAnnotations;

namespace nexumApp.Models
{
    public class VoluntarioModel
    {
        [Key]
        public int IdFormulario { get; set; }

        

        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório")]
        public string CPF { get; set; }

        [Required(ErrorMessage = "O telefone é obrigatório")]
        public string Telefone { get; set; }

        public DateTime? DataNascimento { get; set; }

        public string? Genero { get; set; }

        public string? Habilidades { get; set; }

        public string? ImagemUrl { get; set; }

        public DateTime? DataAprovacao { get; set; }
    }
}