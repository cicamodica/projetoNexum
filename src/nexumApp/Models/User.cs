using Microsoft.AspNetCore.Identity;

namespace nexumApp.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string Cnpj { get; set; }
        public Ong[] Ongs { get; set; }
    }
}
