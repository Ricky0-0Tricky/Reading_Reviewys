using System.ComponentModel.DataAnnotations;

namespace Reading_Reviewys.Models
{
    public class Livro {

        [Key]
        public int IdLivro { get; set;}
        public string Genero { get; set;}
        
        public int AnoPublicacao { get; set;}

        public string NomeAutor { get; set;}
    }
}
