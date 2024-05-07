using System.ComponentModel.DataAnnotations;

namespace Reading_Reviewys.Models
{
    /// <summary>
    /// Tipo de Utilizador que administra 
    /// todas as outras variantes de Utilizador.
    /// </summary>
    public class Admin : Utilizador
    {
        /// <summary>
        /// Email do Administrador
        /// </summary>
        [StringLength(100)]
        public string Email { get; set; }
    }
}
