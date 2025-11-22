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
            modelBuilder.Entity<Ong>()
                .HasMany(Ong => Ong.Filials)
                .WithOne(Filial => Filial.Ong)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Inscricoes>()
                .HasOne(i => i.Vaga)
                .WithMany() 
                .HasForeignKey(i => i.IdVaga)
                .OnDelete(DeleteBehavior.Restrict);

         
            modelBuilder.Entity<Inscricoes>()
                .HasOne(i => i.Candidato)
                .WithMany() 
                .HasForeignKey(i => i.IdCandidato)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<Ong> Ongs {  get; set; }
        public DbSet<Filial> Filials { get; set; }
        public DbSet<Candidato> Candidatos { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<AdministradorModel> Administradores { get; set; }
        public DbSet<Doacao> Doacoes { get; set; }
        public DbSet<Meta> Metas { get; set; }
        public DbSet<FaleConoscoModel> FaleConoscoModels=> Set<FaleConoscoModel>();
        public DbSet<ConviteAdministrador> ConviteAdministradors { get; set; } = null!;
        public DbSet<Vaga> Vagas { get; set; } //Não e uma lista generica de objetos!
        public DbSet<Inscricoes> Inscricoes { get; set; }
    }
}
