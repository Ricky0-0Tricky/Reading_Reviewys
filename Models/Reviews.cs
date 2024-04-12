using System.ComponentModel.DataAnnotations;

namespace Reading_Reviewys.Models
{
    public class Reviews {

        [Key]
        public int IdReview { get; set;}

        public string DescricaoReview { get; set;}

        public DateOnly DataAlteracao { get; set;} 
    }
}
