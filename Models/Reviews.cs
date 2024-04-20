using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reading_Reviewys.Models {
    /// <summary>
    /// Classe representativa das Reviews que 
    /// os Utilizadores poderão fazer a Livros.
    /// </summary>
    public class Reviews {
        /// <summary>
        /// Construtor por defeito da Classe Reviews
        /// </summary>
        public Reviews() {
            ListaComs = new HashSet<Comentarios>();
        }

        /// <summary>
        /// Id da Review que age como PK
        /// para a Classe Reviews
        /// </summary>
        [Key]
        public int IdReview { get; set;}

        /// <summary>
        /// Conteúdo escrito da Review 
        /// </summary>
        public string DescricaoReview { get; set;}

        /// <summary>
        /// Última data de alteração da Review
        /// </summary>
        public DateOnly DataAlteracao { get; set;}

        /* ****************************************
         * Construção dos Relacionamentos
         * ****************************************/

        // Relacionamento 1-N com Utilizadores

        // Chave Estrangeira vinda de Utilizador
        [ForeignKey(nameof(Utilizador))]
        public int UtilizadorFK { get; set;}
        public int Utilizador { get; set;}

        // Relacionamento 1-N com Livros

        // Chave Estrangeira vinda de Livro
        [ForeignKey(nameof(Livro))]
        public int LivroFK { get; set;}
        public int Livro { get; set;}

        // Relacionamento 1-N com Comentários

        // Lista de Comentários que uma Review tem
        public ICollection<Comentarios> ListaComs { get; set;}
    }
}
