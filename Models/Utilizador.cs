using System.ComponentModel.DataAnnotations;

namespace Reading_Reviewys.Models
{
    public class Utilizador {
        [Key]
        public int IdUser { get; set;}

        public string Username { get; set;}

        public string Password { get; set;}

        public string Role { get; set;}

        public DateOnly Data_Entrada { get; set;}

        public byte[] Imagem_Perfil { get; set;}
    }
}
