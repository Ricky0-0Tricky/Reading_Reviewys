using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace Reading_Reviewys.Models
{
    public class Comentarios {

        [Key, Column(Order = 1)]
        public int  NomeUser { get; set;}

        [Key, Column(Order = 2)]
        public DateOnly Data { get; set;}

        public string Descricao { get; set;}
    }
}
