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
        [StringLength(35)]
        [RegularExpression("\\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\\.[A-Z|a-z]{2,}\\b", ErrorMessage = "Insira um email válido!")]
        [Required(ErrorMessage = "O {0} é de preenchimento obrigatório!")]
        public string Email { get; set; }
    }
}
