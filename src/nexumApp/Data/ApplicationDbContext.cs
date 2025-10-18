using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using nexumApp.Models;

namespace nexumApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Ong> Ongs {  get; set; }
        public DbSet<Candidato> Candidatos { get; set; }
        // Mapear modelo para tabela aqui, exemplo acima
    }
}
