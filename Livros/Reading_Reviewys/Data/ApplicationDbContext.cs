using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Reading_Reviewys.Models;

namespace Reading_Reviewys.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Imposição de Roles
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "com", Name = "Comum", NormalizedName = "COMUM" },
                new IdentityRole { Id = "priv", Name = "Priveligiado", NormalizedName = "PRIVELIGIADO" },
                new IdentityRole { Id = "aut", Name = "Autor", NormalizedName = "AUTOR" },
                new IdentityRole { Id = "admin", Name = "Administrador", NormalizedName = "ADMINISTRADOR" }
            );
        }

        /* ********************************************
         * Definição das Tabelas da BD
         * ******************************************** */

        public DbSet<Utilizador> Utilizador { get; set; }
        public DbSet<Comum> Comum { get; set; }
        public DbSet<Priveligiado> Priveligiado { get; set; }
        public DbSet<Autor> Autor { get; set; }
        public DbSet<Admin> Admin { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<Comentarios> Comentarios { get; set; }
        public DbSet<Livro> Livro { get; set; }
    }
}