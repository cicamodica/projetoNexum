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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
                .HasMany(User => User.Ongs)
                .WithOne(Ong => Ong.User)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<Ong> Ongs {  get; set; }
        public DbSet<Candidato> Candidatos { get; set; }
        public DbSet<VoluntarioModel> Voluntarios { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<AdministradorModel> Administradores { get; set; }
        public DbSet<Doacao> Doacao { get; set; }
        public DbSet<Meta> Metas { get; set; }
        public DbSet<FaleConoscoModel> FaleConoscoModels=> Set<FaleConoscoModel>();
    }
}
