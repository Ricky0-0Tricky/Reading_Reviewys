using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Reading_Reviewys.Models
{
    public class Rel_4 {
        // Definição da PK e FKs
        [Key, Column(Order = 1)]
        [ForeignKey(nameof(Autor))]
        public int AutorFK { get; set; }
        public Autor Autor { get; set; }

        [Key, Column(Order = 2)]
        [ForeignKey(nameof(Livro))]
        public int LivroFK { get; set; }
        public Livro Livro { get; set; }
    }
}
