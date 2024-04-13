﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Reading_Reviewys.Models {
    public class Rel_3 {
        public DateOnly Datas_Publicacao { get; set; }

        // Definição da PK e FKs
        [Key, Column(Order = 1)]
        [ForeignKey(nameof(Review))]
        public int? ReviewFK { get; set; }
        public Reviews Review { get; set; }

        [Key, Column(Order = 2)]
        [ForeignKey(nameof(Livro))]
        public int? LivroFK { get; set; }
        public Livro Livro { get; set; }
    }
}
