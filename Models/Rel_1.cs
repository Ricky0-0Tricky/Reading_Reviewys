using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Reading_Reviewys.Models {
    public class Rel_1 {
        // Definição da PK e FKs
        [Key ,ForeignKey(nameof("Review"))]
        public int? ReviewFK { get; set; }
        public Reviews Review { get; set; }

        [ForeignKey(nameof("Utilizador"))]
        public int? UtilizadorFK { get; set; }
        public Utilizador Utilizador { get; set; }
    }
}
