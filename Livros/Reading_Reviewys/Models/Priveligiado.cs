using System.ComponentModel.DataAnnotations;

namespace Reading_Reviewys.Models
{
    /// <summary>
    /// Tipo de Utilizador que pagou uma subscrição
    /// para ter acesso a certos privilégios e à entidade Autor.
    /// </summary>
    public class Priveligiado : Utilizador
    {
        /// <summary>
        /// Data em que a subscrição foi efetuada
        /// </summary>
        [Display(Name = "Data de Subscrição")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateOnly Data_Subscricao { get; set; }
    }
}
