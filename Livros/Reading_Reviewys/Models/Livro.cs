using System.ComponentModel.DataAnnotations;

namespace Reading_Reviewys.Models
{
    /// <summary>
    /// Classe representativa da entidade principal do tema
    /// de Reviews.
    /// </summary>
    public class Livro
    {
        /// <summary>
        /// Construtor por defeito da Classe "Livros"
        /// </summary>
        public Livro()
        {
            ListaPublicacao = new HashSet<Reviews>();
            ListaAutores = new HashSet<Autor>();
        }

        /// <summary>
        /// PK
        /// </summary>
        [Key]
        public int IdLivro { get; set;}

        /// <summary>
        /// Imagem do Livro
        /// </summary>
        public string? Capa { get; set;}

        /// <summary>
        /// Título do Livro
        /// </summary>
        [Display(Name = "Título")]
        [StringLength(20)]
        [Required(ErrorMessage = "Escreva o {0} do Livro que pretende registrar!")]
        public string Titulo { get; set;}

        /// <summary>
        /// Género do Livro 
        /// </summary>
        [Display(Name = "Género")]
        [StringLength(15)]
        [Required(ErrorMessage = "É necessário escolher o Género do Livro!")]
        public string Genero { get; set; }

        /// <summary>
        /// Ano de Publicação do Livro
        /// </summary>
        [Display(Name = "Ano de Publicação")]
        [Range(1, 2024, ErrorMessage = "O Ano de Publicação tem de ter entre {1} e {2}!")]
        [Required(ErrorMessage = "É necessário escolher o Ano de Publicação do Livro!")]
        public int AnoPublicacao { get; set; }


        /* ****************************************
         * Construção dos Relacionamentos
         * ****************************************/

        // Relacionamento 1-N com Reviews
        /// <summary>
        /// Lista de Publicações (Reviews) associados ao Livro
        /// </summary>
        public ICollection<Reviews> ListaPublicacao { get; set; }

        // Relacionamento N-M com Autores, sem atributos no relacionamento
        public ICollection<Autor> ListaAutores { get; set; }

    }
}
