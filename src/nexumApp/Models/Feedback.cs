using System.ComponentModel.DataAnnotations;

namespace nexumApp.Models
{
    public class Feedback
    {
        [Key]
        public int Id { get; set; }
        [StringLength(3)]      
        public string Assunto { get; set; }
        [StringLength(500)]

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataEnvio { get; set; }
    }
}
