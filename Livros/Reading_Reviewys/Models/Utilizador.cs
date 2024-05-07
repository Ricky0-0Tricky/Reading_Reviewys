using System.ComponentModel.DataAnnotations;

namespace Reading_Reviewys.Models
{
    /// <summary>
    /// Super-classe que define os atributos 
    /// que as suas variantes terão.
    /// </summary>
    public class Utilizador
    {
        public Utilizador()
        {
            ListaReviews = new HashSet<Reviews>();
            ListaComentarios = new HashSet<Comentarios>();
        }

        /// <summary>
        /// Id do Utilizador que age como PK 
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
        public string Role { get; set; }

        /// <summary>
        /// Data de Registo do Utilizador
        /// </summary>
        [Display(Name = "Data de Entrada")]
        public DateOnly Data_Entrada { get; set; }

        /// <summary>
        /// Imagem de Perfil do Utilizador 
        /// </summary>
        [Display(Name = "Imagem de Perfil")]
        public byte[] Imagem_Perfil { get; set; }

        /* ****************************************
         * Construção dos Relacionamentos
         * ****************************************/

        // Relacionamento 1-N com Reviews

        // Lista de Reviews que foram feitas por um Utilizador
        public ICollection<Reviews> ListaReviews { get; set; }

        public ICollection<Comentarios> ListaComentarios { get; set; }
    }
}
