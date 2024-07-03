using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reading_Reviewys.Data;
using Reading_Reviewys.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;


namespace Reading_Reviewys.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UtilizadorController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<IdentityUser> _userManager;

        public UtilizadorController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Utilizador
        public async Task<IActionResult> Index()
        {
            return View(await _context.Utilizador.ToListAsync());
        }

        // GET: Utilizador/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(m => m.IdUser == id);
            if (utilizador == null)
            {
                return NotFound();
            }

            return View(utilizador);
        }

        // GET: Utilizador/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Utilizador/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdUser,Username,Role,Data_Entrada,Imagem_Perfil")] Utilizador utilizador)
        {
            if (ModelState.IsValid)
            {
                // Regista a data de entrada do utilizador
                utilizador.Data_Entrada = DateOnly.FromDateTime(DateTime.Now);

                // Adição do Utilizador e salvaguarda dos seus dados na BD
                _context.Add(utilizador);
                await _context.SaveChangesAsync();

                // Regressa ao Index
                return RedirectToAction(nameof(Index));
            }
            return View(utilizador);
        }

        // GET: Utilizador/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizador.FindAsync(id);
            if (utilizador == null)
            {
                return NotFound();
            }
            return View(utilizador);
        }

        // POST: Utilizador/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdUser,Username,Role,Imagem_Perfil")] Utilizador utilizador)
        {
            if (id != utilizador.IdUser)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Tenta encontrar o utilizador na tabela "Utilizador"
                    var existeUtilizador = await _context.Utilizador.FindAsync(utilizador.IdUser);

                    // Se o mesmo não existir retorna-se a página de NotFound
                    if (existeUtilizador == null)
                    {
                        return NotFound();
                    }

                    // Atualização de Dados
                    existeUtilizador.Username = utilizador.Username;
                    existeUtilizador.Role = utilizador.Role;
                    existeUtilizador.Imagem_Perfil = utilizador.Imagem_Perfil;

                    _context.Update(existeUtilizador);

                    // Tenta encontrar o utilizador na tabela "AspNetUsers" pelo seu dado "UserID"
                    var utilizadorIdentity = await _userManager.FindByIdAsync(existeUtilizador.UserID);

                    // Se o mesmo não existir retorna-se a página de NotFound
                    if (utilizadorIdentity == null)
                    {
                        return NotFound();
                    }

                    // Caso o mesmo tenha sido encontrado, atualizam-se alguns dos seus dados e permissões
                    utilizadorIdentity.UserName = utilizador.Username;

                    switch(existeUtilizador.Role)
                    {
                        case "Comum":
                            await _userManager.AddToRoleAsync(utilizadorIdentity, "Comum");
                            break;
                        case "Priveligiado":
                            await _userManager.AddToRoleAsync(utilizadorIdentity, "Priveligiado");
                            break;
                        case "Autor":
                            await _userManager.AddToRoleAsync(utilizadorIdentity, "Autor");
                            break;
                        case "Admin":
                            await _userManager.AddToRoleAsync(utilizadorIdentity, "Administrador");
                            break;
                        default:
                            break;
                    }

                    // Tenta-se fazer update à tabela "AspNetUsers"
                    var updIdentity = await _userManager.UpdateAsync(utilizadorIdentity);
                    if (!updIdentity.Succeeded)
                    {
                        return View(utilizador);
                    }
                    
                    // Salvam-se as alterações
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtilizadorExists(utilizador.IdUser))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(utilizador);
        }

        // GET: Utilizador/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(m => m.IdUser == id);
            if (utilizador == null)
            {
                return NotFound();
            }

            return View(utilizador);
        }

        // POST: Utilizador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Captura do utilizador segundo o seu id
            var utilizador = await _context.Utilizador.FindAsync(id);

            // Caso não seja encontrado nenhum utilizador retorna-se a página de NotFound
            if (utilizador == null)
            {
                return NotFound();
            }

            // Tentativa de apagar o utilizador das tabelas "Utilizador" e "AspNetUsers"
            try
            {
                // Apagar os comentários do utilizador
                var comentarios = await _context.Comentarios
                                            .Where(c => c.CriadorComentarioFK == utilizador.IdUser)
                                            .ToListAsync();
                _context.Comentarios.RemoveRange(comentarios);

                // Apagar reviews do utilizador
                var reviews = await _context.Reviews
                                            .Where(r => r.UtilizadorFK == utilizador.IdUser)
                                            .ToListAsync();
                _context.Reviews.RemoveRange(reviews);

                // Salvar as alterações realizadas (Apagar comentários e reviews do utilizador)
                await _context.SaveChangesAsync();

                // Apagar o utilizador da tabela "Utilizador"
                _context.Utilizador.Remove(utilizador);
                await _context.SaveChangesAsync();

                // Apagar o utilizador da tabela "AspNetUsers"
                var utilizadorIdentity = await _userManager.FindByIdAsync(utilizador.UserID);

                // Caso não seja encontrado nenhum utilizador com o dado "UserID" retorna-se a página de NotFound
                if (utilizadorIdentity == null)
                {
                    return NotFound();
                }

                // Tentativa de apagar o utilizador da tabela "AspNetUsers"
                var apagaIdent = await _userManager.DeleteAsync(utilizadorIdentity);

                // Caso não tenha sido possível apagar o utilizador da tabela "AspNetUsers" anuncia-se falhanço
                if (!apagaIdent.Succeeded)
                {
                    return BadRequest("Falhanço ao tentar apagar o utilizador da tabela AspNetUsers.");
                }

                // Caso tenha havido sucesso, volta-se ao Index
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex) {
                // Anunciar que existiu falhanço
                return BadRequest($"Falhanço ao apagar o user: {ex.Message}");
            }
        }

            private bool UtilizadorExists(int id)
        {
            return _context.Utilizador.Any(e => e.IdUser == id);
        }
    }
}
