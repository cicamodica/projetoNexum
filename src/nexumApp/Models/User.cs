using Microsoft.AspNetCore.Identity;

namespace nexumApp.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string CNPJ { get; set; }
    }
}
