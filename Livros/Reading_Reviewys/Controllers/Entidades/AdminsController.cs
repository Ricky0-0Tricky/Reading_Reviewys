using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reading_Reviewys.Data;
using Reading_Reviewys.Models;

namespace Reading_Reviewys.Controllers
{
    [Authorize]
    public class AdminsController : Controller
    {
        /// <summary>
        /// Objeto representativo da BD
        /// </summary>
        private readonly ApplicationDbContext _context;

        public AdminsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admins
        public async Task<IActionResult> Index()
        {
            // Retorna a lista de Admins
            return View(await _context.Admin.ToListAsync());
        }

        // GET: Admins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin
                .FirstOrDefaultAsync(m => m.IdUser == id);
            if (admin == null)
            {
                return NotFound();
            }

            // Caso em que o Admin existe, devolve-se a View com os seus dados
            return View(admin);
        }

        // GET: Admins/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("Email,IdUser,Username,Role,Data_Entrada,Imagem_Perfil")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                // Atribuição de valores ao Admin
                admin.Data_Entrada = DateOnly.FromDateTime(DateTime.Now);
                admin.Role = "Administrador";

                // Adição do Admin e salvaguarda dos seus dados na BD
                _context.Add(admin);
                await _context.SaveChangesAsync();

                // Regresso ao Index
                return RedirectToAction(nameof(Index));
            }

            // Caso o Model seja inválido, devolve-se a View com os dados inseridos
            return View(admin);
        }

        // GET: Admins/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // POST: Admins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("Email,IdUser,Username,Imagem_Perfil")] Admin admin)
        {
            if (id != admin.IdUser)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Tenta encontrar o Admin a editar através do seu id
                    var atualAdmin = await _context.Admin.FindAsync(id);
                    if (atualAdmin == null)
                    {
                        return NotFound();
                    }

                    // Atualização dos atributos desejados
                    atualAdmin.Email = admin.Email;
                    atualAdmin.Username = admin.Username;
                    atualAdmin.Imagem_Perfil = admin.Imagem_Perfil;

                    // Update dos atributos editáveis e salvaguarda das alterações para a BD
                    _context.Update(atualAdmin);
                    await _context.SaveChangesAsync();

                    // Regresso ao Index
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Caso em que o Admin não existe e devolve-se a página de "NotFound"
                    if (!AdminExists(admin.IdUser))
                    {
                        return NotFound();
                    }
                    // Caso em que o Admin existe mas houve lançamento de uma exceção
                    else
                    {
                        throw;
                    }
                }
            }

            // Caso o Model seja inválido, devolve-se a View com os dados inseridos
            return View(admin);
        }

        // GET: Admins/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin
                .FirstOrDefaultAsync(m => m.IdUser == id);
            if (admin == null)
            {
                return NotFound();
            }

            // Caso o Admin não seja encontrado, devolve-se a mesma View
            return View(admin);
        }

        // POST: Admins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var admin = await _context.Admin.FindAsync(id);
            if (admin != null)
            {
                _context.Admin.Remove(admin);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se um Admin existe na BD
        /// </summary>
        /// <param name="id">ID do Admin</param>
        /// <returns>Verdadeiro se o Admin existir, Falso caso contrário</returns>
        private bool AdminExists(int id)
        {
            return _context.Admin.Any(e => e.IdUser == id);
        }
    }
}
