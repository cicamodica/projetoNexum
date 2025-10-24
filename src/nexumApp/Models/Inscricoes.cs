using System.ComponentModel.DataAnnotations;

namespace nexumApp.Models
{
    public class Inscricoes
    {

        [Key]
        public int Id { get; set; }
        [StringLength(3)]
        public int IdMeta { get; set; }
        public int IdCandidato { get; set; }


        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataInscricao { get; set; }

    }
}
