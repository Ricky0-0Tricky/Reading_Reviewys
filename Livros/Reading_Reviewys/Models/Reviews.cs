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
            ListaComentarios = new HashSet<Comentarios>();
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
        [Display(Name="Descrição da Review")]
        [StringLength(600)]
        [Required(ErrorMessage="Caso queria fazer uma {0}, escreva qualquer coisa!")]
        public string DescricaoReview { get; set;}

        /// <summary>
        /// Última data de alteração da Review
        /// </summary>
        [Display(Name="Data da Última Alteração")]
        public DateOnly DataAlteracao { get; set;}

        /* ****************************************
         * Construção dos Relacionamentos
         * ****************************************/

        // Relacionamento 1-N com Utilizadores

        // Chave Estrangeira vinda de Utilizador
        /// <summary>
        /// FK para Utilizador que escreve a Review
        /// </summary>
        [ForeignKey(nameof(Utilizador))]
        public int UtilizadorFK { get; set;}
        /// <summary>
        /// FK para Utilizador que escreve a Review
        /// </summary>
        public Utilizador Utilizador { get; set;}

        // Relacionamento 1-N com Livros

        // Chave Estrangeira vinda de Livro
        /// <summary>
        /// FK para o Livro objeto da Review
        /// </summary>
        [ForeignKey(nameof(Livro))]
        public int LivroFK { get; set;}
        /// <summary>
        /// FK para o Livro objeto da Review
        /// </summary>
        public Livro Livro { get; set;}

        // Relacionamento 1-N com Comentários

        /// <summary>
        /// Lista de Comentários associados a uma Review
        /// </summary>
        public ICollection<Comentarios> ListaComentarios { get; set;}
    }
}
