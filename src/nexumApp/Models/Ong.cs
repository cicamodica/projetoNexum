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
        //public ICollection<Meta> Metas { get; set; }
    }
}
