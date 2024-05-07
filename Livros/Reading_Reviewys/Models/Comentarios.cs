﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Security.Cryptography.X509Certificates;

namespace Reading_Reviewys.Models
{
    /// <summary>
    /// Classe representativa dos Comentários
    /// que poderão ser feitos às Reviews pelos
    /// Utilizadores.
    /// </summary>
    public class Comentarios
    {
        /// <summary>
        /// PK
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Data de submissão do Comentário
        /// que age como parte da PK da Classe Comentarios
        /// </summary>
        [Display(Name="Data de Publicação")]
        public DateTime Data { get; set; }

        /// <summary>
        /// Conteúdo escrito no Comentário
        /// </summary>
        [Display(Name="Descrição")]
        [StringLength(100)]
        [Required(ErrorMessage="Se quer realizar um {0}, escreva qualquer coisa!")]
        public string Descricao { get; set; }

        /* ****************************************
         * Construção dos Relacionamentos
         * ****************************************/

        // Relacionamento 1-N com Reviews

        // Chave Estrangeira vinda de Reviews
        [ForeignKey(nameof(Review))]
        public int ReviewFK { get; set; }
        public Reviews Review { get; set; }

        /// <summary>
        /// Criador do Comentrário
        /// </summary>
        [ForeignKey(nameof(CriadorComentario))]
        public int CriadorComentarioFK { get; set; }
        public Utilizador CriadorComentario { get; set; }
    }
}
