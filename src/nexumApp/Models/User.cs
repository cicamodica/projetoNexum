using Microsoft.AspNetCore.Identity;

namespace nexumApp.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string CPF { get; set; }
        public ICollection<Ong> Ongs { get; set; }
    }
}
