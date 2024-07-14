using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reading_Reviewys.Data;
using Reading_Reviewys.Models;
using System.Diagnostics;

namespace Reading_Reviewys.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            // Conceder ao Index todos os Livros na BD
            var livros = _context.Livro.ToList();
            return View(livros);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Evoluir()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Comum")]
        public async Task<IActionResult> Evoluir(string nomeCartao, string numCartao, string expCartao, string cvvCartao)
        {
            if (ModelState.IsValid)
            {
                // Encontrar o Utilizador Atual
                var utilIdentity = await _userManager.GetUserAsync(User);

                // Se o Utilizador não for nulo, então damos-lhe upgrade
                if (utilIdentity != null)
                {
                    // Remoção do role do Utilizador
                    var role = await _userManager.GetRolesAsync(utilIdentity);
                    await _userManager.RemoveFromRolesAsync(utilIdentity, role);

                    // Tentativa de atribuição das permissões de "Priveligiado"
                    var tentEvol = await _userManager.AddToRoleAsync(utilIdentity, "Priveligiado");

                    if (tentEvol.Succeeded)
                    {
                        // Atualização da info do Utilizador na tabela "Utilizadores"
                        var utilTab = await _context.Utilizador.FirstOrDefaultAsync(u => u.UserID == utilIdentity.Id);
                        if (utilTab != null)
                        {
                            // Atualização do Role na tabela "AspNetUsers"
                            await _userManager.AddToRoleAsync(utilIdentity, "Priveligiado");

                            var priveligiado = new Priveligiado
                            {
                                IdUser = utilTab.IdUser,
                                UserID = utilTab.UserID,
                                Username = utilTab.Username,
                                Role = "Priveligiado",
                                Imagem_Perfil = utilTab.Imagem_Perfil,
                                Data_Entrada = utilTab.Data_Entrada,
                                Data_Subscricao = DateOnly.FromDateTime(DateTime.Now)
                            };
                            // Remoção do Utilizador Antigo na tabela "Utilizadores"
                            _context.Remove(utilTab);

                            _context.Add(priveligiado);

                            // Buscar todos os comentários do Utilizador Antigo
                            var comentsAntigos = await _context.Comentarios.Where(c => c.CriadorComentarioFK == utilTab.IdUser).ToListAsync();

                            // Atualizar o IdUser dos Comentários para o novo utilizador
                            foreach (var comentario in comentsAntigos)
                            {
                                comentario.CriadorComentarioFK = priveligiado.IdUser;
                                _context.Update(comentario);
                            }

                            // Guardar as alterações realizadas
                            await _context.SaveChangesAsync();

                            // Buscar todas as reviews do Utilizador Antigo
                            var revwsAntigas = await _context.Reviews.Where(r => r.UtilizadorFK == utilTab.IdUser).ToListAsync();

                            // Atualizar o IdUser das Reviews para o novo utilizador
                            foreach (var review in revwsAntigas)
                            {
                                review.UtilizadorFK = priveligiado.IdUser;
                                _context.Update(review);
                            }

                            // Guardar as alterações realizadas
                            await _context.SaveChangesAsync();
                        }
                    }

                    // Sign-in automático com refresh
                    await _signInManager.RefreshSignInAsync(utilIdentity);

                    // Redirect para a HomePage
                    return RedirectToAction("Index", "Home");
                }
            }
            // Algo correu mal
            return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
}
