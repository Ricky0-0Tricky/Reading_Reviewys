using System.ComponentModel.DataAnnotations;

namespace Reading_Reviewys.Models
{
    public class Livro {

        public Livro() {
            ListaPublicacao = new HashSet<Rel_3>();
        }

        [Key]
        public int IdLivro { get; set;}
        public string Genero { get; set;}
        
        public int AnoPublicacao { get; set;}

        public string NomeAutor { get; set;}

        /* ****************************************
         * Construção dos Relacionamentos
         * *************************************** */

        // relacionamento N-M, com atributos no relacionamento
        public ICollection<Rel_3> ListaPublicacao { get; set;}

    }
}
