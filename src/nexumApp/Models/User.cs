using Microsoft.AspNetCore.Identity;

namespace nexumApp.Models
{
    public class User : IdentityUser
    {
        public ICollection<Ong> Ongs { get; set; }
    }
}
