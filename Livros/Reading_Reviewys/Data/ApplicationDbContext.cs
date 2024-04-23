using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Reading_Reviewys.Models;

namespace Reading_Reviewys.Data {
    /// <summary>
    /// Classe representativa da BD do projeto
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {
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