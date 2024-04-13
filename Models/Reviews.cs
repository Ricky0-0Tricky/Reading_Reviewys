using System.ComponentModel.DataAnnotations;

namespace Reading_Reviewys.Models
{
    public class Reviews {

        public Reviews() {
            ListaComs = new HashSet<Comentarios>();
        }

        [Key]
        public int IdReview { get; set;}

        public string DescricaoReview { get; set;}

        public DateOnly DataAlteracao { get; set;}

        /*****************************
         * Construção de Relacionamentos  
         *****************************/

        // relacionamento 1-N com Comentários

        // Lista de Comentários que uma Review tem
        public ICollection<Comentarios> ListaComs { get; set;}
    }
}
