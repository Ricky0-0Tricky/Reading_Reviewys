using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Reading_Reviewys.Data;

namespace Reading_Reviewys.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Imagem_Perfil { get; set; }

        [BindProperty]
        public IFormFile ImagemPerfil { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("Utilizador Asp não encontrado");
            }

            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.UserID == user.Id);
            if (utilizador == null)
            {
                return NotFound("Utilizador não encontrado");
            }

            Username = utilizador.Username;
            Imagem_Perfil = utilizador.Imagem_Perfil;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound("User not found");
            }

            var utilizador = await _context.Utilizador.FirstOrDefaultAsync(u => u.UserID == user.Id);
            if (utilizador == null)
            {
                return NotFound("Utilizador not found");
            }

            // Verifica se existe uma imagem submetida
            if (ImagemPerfil != null)
            {
                // Validação do Formato do Documento submetido
                if (ImagemPerfil.ContentType == "image/png" || ImagemPerfil.ContentType == "image/jpeg")
                {
                    // Geração de um nome único para a imagem
                    string nomeImagem = Guid.NewGuid().ToString() + Path.GetExtension(ImagemPerfil.FileName).ToLowerInvariant();

                    // Caminho para salvar a imagem
                    string localizacaoImagem = Path.Combine(_webHostEnvironment.WebRootPath, "Imagens", nomeImagem);

                    // Salvar a imagem para o servidor
                    using (var stream = new FileStream(localizacaoImagem, FileMode.Create))
                    {
                        await ImagemPerfil.CopyToAsync(stream);
                    }

                    // Atualiza o caminho da imagem
                    utilizador.Imagem_Perfil = nomeImagem;
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "O formato da imagem não é suportado. Por favor, escolha um formato válido");
                    return Page();
                }
            }

            // Atualiza o username
            utilizador.Username = Username;
            _context.Update(utilizador);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
