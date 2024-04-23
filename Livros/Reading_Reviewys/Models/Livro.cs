using System.ComponentModel.DataAnnotations;

namespace Reading_Reviewys.Models {
    /// <summary>
    /// Classe representativa da entidade principal do tema
    /// de Reviews.
    /// </summary>
    public class Livro {
        /// <summary>
        /// Construtor por defeito da Classe de Livros
        /// </summary>
        public Livro() {
            ListaPublicacao = new HashSet<Reviews>();
            ListaAutores = new HashSet<Autor>();
        }

        /// <summary>
        /// Id do Livro que age
        /// como PK para a Classe Livro
        /// </summary>
        [Key]
        public int IdLivro { get; set;}

        /// <summary>
        /// Género do Livro 
        /// </summary>
        public string Genero { get; set;}

        /// <summary>
        /// Ano de Publicação do Livro
        /// </summary>        
        public int AnoPublicacao { get; set;}


        /* ****************************************
         * Construção dos Relacionamentos
         * ****************************************/

        // Relacionamento 1-N com Reviews
        /// <summary>
        /// Lista de Publicações (Reviews) associados ao Livro
        /// </summary>
        public ICollection<Reviews> ListaPublicacao { get; set;}

        // Relacionamento N-M com Autores, sem atributos no relacionamento
        public ICollection<Autor> ListaAutores { get; set;}

    }
}
