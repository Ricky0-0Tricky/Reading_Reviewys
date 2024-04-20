using System.ComponentModel.DataAnnotations;

namespace Reading_Reviewys.Models {
    /// <summary>
    /// Super-classe que define os atributos 
    /// que as suas variantes terão.
    /// </summary>
    public class Utilizador {
        public Utilizador() {
            ListaReviews = new HashSet<Reviews>();
            
        }
        /// <summary>
        /// Id do Utilizador que age como PK 
        /// </summary>
        [Key]
        public int IdUser { get; set;}

        /// <summary>
        /// Username do Utilizador
        /// </summary>
        public string Username { get; set;}

        /// <summary>
        /// Password do Utilizador
        /// </summary>
        public string Password { get; set;}

        /// <summary>
        /// Tier do Utilizador
        /// </summary>
        public string Role { get; set;}

        /// <summary>
        /// Data de Registo do Utilizador
        /// </summary>
        public DateOnly Data_Entrada { get; set;}

        /// <summary>
        /// Imagem de Perfil do Utilizador 
        /// </summary>
        public byte[] Imagem_Perfil { get; set;}

        /* ****************************************
         * Construção dos Relacionamentos
         * ****************************************/

        // Relacionamento 1-N com Reviews

        // Lista de Reviews que foram feitas por um Utilizador
        public ICollection<Reviews> ListaReviews { get; set;}
    }
}
