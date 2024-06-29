using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reading_Reviewys.Models
{
    /// <summary>
    /// Super-classe que define os atributos 
    /// que as suas variantes terão.
    /// </summary>
    public class Utilizador
    {
        /// <summary>
        /// Construtor por defeito da Classe "Utilizador"
        /// </summary>
        public Utilizador()
        {
            ListaReviews = new HashSet<Reviews>();
            ListaComentarios = new HashSet<Comentarios>();
        }

        /// <summary>
        /// PK
        /// </summary>
        [Key]
        public int IdUser { get; set; }

        /// <summary>
        /// Username do Utilizador
        /// </summary>
        [StringLength(20)]
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório!")]
        public string Username { get; set; }

        /// <summary>
        /// Tier do Utilizador
        /// </summary>
        [StringLength(12, ErrorMessage = "Insira um Role Válido!")]
        //[Required(ErrorMessage = "O {0} é de preenchimento obrigatório!")]
        public string Role { get; set; }

        /// <summary>
        /// Data de Registo do Utilizador
        /// </summary>
        [Display(Name = "Data de Entrada")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateOnly Data_Entrada { get; set; }

        /// <summary>
        /// Imagem de Perfil do Utilizador 
        /// </summary>
        [Display(Name = "Imagem de Perfil")]
        public string? Imagem_Perfil { get; set; }

        /// <summary>
        /// Atributo para funcionar como FK 
        /// entre a tabela dos Utilizadores e da Autenticação
        /// </summary>
        public string UserID { get; set; }

        /* ****************************************
         * Construção dos Relacionamentos
         * ****************************************/

        // Relacionamento 1-N com Reviews

        // Lista de Reviews que foram feitas por um Utilizador
        public ICollection<Reviews> ListaReviews { get; set; }

        public ICollection<Comentarios> ListaComentarios { get; set; }
    }
}
