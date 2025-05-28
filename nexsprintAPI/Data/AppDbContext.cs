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
                .HasKey(ul => new { ul.NomeUsuario, ul.ModuloId }); // Mapeia a relação de muitos pra muitos

            modelBuilder.Entity<UserModulo>()
                .HasOne(ul => ul.User)
                .WithMany(u => u.UsersModulos)
                .HasForeignKey(ul => ul.NomeUsuario);

            modelBuilder.Entity<UserModulo>()
                .HasOne(ul => ul.Modulo)
                .WithMany(l => l.UsersModulos)
                .HasForeignKey(ul => ul.ModuloId);

        }
    }
}