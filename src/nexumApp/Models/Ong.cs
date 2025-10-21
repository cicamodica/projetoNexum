using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace nexumApp.Models
{
    public class Ong
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Nome { get; set; }
        public string Descriçao { get; set; }
        public string Endereço { get; set; }
        public string CNPJ { get; set; }
        public bool Aprovaçao { get; set; } = false;

        //[Display(Name = "Documento de Comprovação")]
        //[RegularExpression(@"^.*\.(pdf|PDF)$", ErrorMessage = "Arquivo deve ser PDF.")]
        //public byte[] Documento { get; set; }
        //public ICollection<Meta> Metas { get; set; }
    }
}
