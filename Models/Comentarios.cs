using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace Reading_Reviewys.Models {
    /// <summary>
    /// Classe representativa dos Comentários
    /// que poderão ser feitos às Reviews pelos
    /// Útilizadores.
    /// </summary>
    public class Comentarios {
        /// <summary>
        /// Username do Utilizador que age 
        /// como parte da PK da Classe Comentarios
        /// </summary>
        [Key, Column(Order = 1)]
        public int NomeUser { get; set;}

        /// <summary>
        /// Data de submissão do Comentário
        /// que age como parte da PK da Classe Comentarios
        /// </summary>
        [Key, Column(Order = 2)]
        public DateOnly Data { get; set;}

        /// <summary>
        /// Conteúdo escrito no Comentário
        /// </summary>
        public string Descricao { get; set;}

        /* ****************************************
         * Construção dos Relacionamentos
         * ****************************************/

        // Relacionamento 1-N com Reviews

        // Chave Estrangeira vinda de Reviews
        [ForeignKey(nameof(Review))]
        public int ReviewFK { get; set;}
        public Reviews Review { get; set;} 
    }
}
