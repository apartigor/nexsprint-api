using Microsoft.EntityFrameworkCore;
using nexsprintAPI.Models;

namespace nexsprintAPI.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Modulo> Modulos { get; set; }
        public DbSet<UserModulo> UsersModulos { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
            .HasKey(u => u.NomeUsuario); // Chave primária com NomeUsuario

            modelBuilder.Entity<UserModulo>()
                .HasKey(um => new { um.NomeUsuario, um.ModuloId }); // Mapeia a relação de muitos pra muitos

            modelBuilder.Entity<UserModulo>()
                .HasOne(um => um.User)
                .WithMany(u => u.UsersModulos)
                .HasForeignKey(um => um.NomeUsuario);

            modelBuilder.Entity<UserModulo>()
                .HasOne(um => um.Modulo)
                .WithMany(l => l.UsersModulos)
                .HasForeignKey(um => um.ModuloId);

        }
    }
}