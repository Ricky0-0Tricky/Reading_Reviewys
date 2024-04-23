using System.ComponentModel.DataAnnotations;

namespace Reading_Reviewys.Models {
    /// <summary>
    /// Tipo de Utilizador que está asssociado à entidade Livros.
    /// </summary>
    public class Autor : Utilizador {
        /// <summary>
        /// Construtor da Classe Autor
        /// </summary>
        public Autor() {
            ListaLivros = new HashSet<Livro>();
        }

        /// <summary>
        /// Nome Real do Utilizador
        /// </summary>
        public string Nome { get; set; }

        /* ****************************************
         * Construção dos Relacionamentos
         *****************************************/

        // Relacionamento N-M com Livro, sem atributos no relacionamento
        public ICollection<Livro> ListaLivros { get; set; }
    }
}
