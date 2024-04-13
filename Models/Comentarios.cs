using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace Reading_Reviewys.Models {
    public class Comentarios {

        [Key, Column(Order = 1)]
        public int  NomeUser { get; set;}

        [Key, Column(Order = 2)]
        public DateOnly Data { get; set;}

        public string Descricao { get; set;}

        /*****************************
         * Construção de Relacionamentos  
         *****************************/

        // relacionamento N-1 com Reviews
        [ForeignKey(nameof(Review))]
        public int ReviewFK { get; set;}
        public Reviews Review { get; set;} 
    }
}
