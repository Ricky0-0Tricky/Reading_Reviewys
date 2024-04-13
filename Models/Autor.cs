using System.ComponentModel.DataAnnotations;

namespace Reading_Reviewys.Models
{
    public class Autor : Utilizador {

        public Autor() {
            ListaLivros = new HashSet<Rel_4>();
        }
        public string Nome { get; set;}

        // relacionamento N-M, sem atributos no relacionamento
        public ICollection<Rel_4> ListaLivros { get; set; }
    }
}
