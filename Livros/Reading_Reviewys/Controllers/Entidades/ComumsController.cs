using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reading_Reviewys.Data;
using Reading_Reviewys.Models;

namespace Reading_Reviewys.Controllers
{
    [Authorize]
    public class ComumsController : Controller
    {
        /// <summary>
        /// Objeto representativo da BD
        /// </summary>
        private readonly ApplicationDbContext _context;

        public ComumsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Comums
        [Authorize(Roles = "Comum,Administrador")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Comum.ToListAsync());
        }

        // GET: Comums/Details/5
        [Authorize(Roles = "Comum,Administrador")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comum = await _context.Comum
                .FirstOrDefaultAsync(m => m.IdUser == id);
            if (comum == null)
            {
                return NotFound();
            }

            // Caso em que o Comum existe, devolve-se a View com os seus dados
            return View(comum);
        }

        // GET: Comums/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Comums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("IdUser,Username,Role,Data_Entrada,Imagem_Perfil")] Comum comum)
        {
            if (ModelState.IsValid)
            {
                // Atribuição de valores
                comum.Data_Entrada = DateOnly.FromDateTime(DateTime.Now);
                comum.Role = "Comum";

                // Adição do Comum e salvaguarda dos seus dados na BD
                _context.Add(comum);
                await _context.SaveChangesAsync();

                // Regresso ao Index
                return RedirectToAction(nameof(Index));
            }

            // Caso o Model seja inválido, devolve-se a View com os dados inseridos
            return View(comum);
        }

        // GET: Comums/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comum = await _context.Comum.FindAsync(id);
            if (comum == null)
            {
                return NotFound();
            }

            return View(comum);
        }

        // POST: Comums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("IdUser,Username,Imagem_Perfil")] Comum comum)
        {
            if (id != comum.IdUser)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Tenta encontrar o Comum a editar através do seu ID
                    var atualComum = await _context.Comum.FindAsync(id);
                    if (atualComum == null)
                    {
                        return NotFound();
                    }

                    // Atualização dos atributos editáveis
                    atualComum.Username = comum.Username;
                    atualComum.Imagem_Perfil = comum.Imagem_Perfil;

                    // Update dos atributos editáveis e salvaguarda das alterações para a BD
                    _context.Update(atualComum);
                    await _context.SaveChangesAsync();

                    // Regresso ao Index
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Caso em que o Comum não existe e devolve-se a página de "NotFound"
                    if (!ComumExists(comum.IdUser))
                    {
                        return NotFound();
                    }
                    // Caso em que o Comum existe mas houve lançamento de uma exceção
                    else
                    {
                        throw;
                    }
                }
            }

            // Caso o Model seja inválido, devolve-se a View com os dados inseridos
            return View(comum);
        }

        // GET: Comums/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comum = await _context.Comum
                .FirstOrDefaultAsync(m => m.IdUser == id);
            if (comum == null)
            {
                return NotFound();
            }

            // Caso o Comum não seja encontrado, devolve-se a mesma View
            return View(comum);
        }

        // POST: Comums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comum = await _context.Comum.FindAsync(id);
            if (comum != null)
            {
                _context.Comum.Remove(comum);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se um Comum existe na BD
        /// </summary>
        /// <param name="id">ID do Comum</param>
        /// <returns>Verdadeiro se o Comum existir, Falso caso contrário</returns>
        private bool ComumExists(int id)
        {
            return _context.Comum.Any(e => e.IdUser == id);
        }
    }
}
