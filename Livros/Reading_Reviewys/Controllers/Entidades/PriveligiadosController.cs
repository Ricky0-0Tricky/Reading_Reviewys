using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reading_Reviewys.Data;
using Reading_Reviewys.Models;

namespace Reading_Reviewys.Controllers
{
    [Authorize]
    public class PriveligiadosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PriveligiadosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Priveligiados
        [Authorize(Roles = "Priveligiado,Administrador")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Priveligiado.ToListAsync());
        }

        // GET: Priveligiados/Details/5
        [Authorize(Roles = "Priveligiado,Administrador")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priveligiado = await _context.Priveligiado
                .FirstOrDefaultAsync(m => m.IdUser == id);
            if (priveligiado == null)
            {
                return NotFound();
            }

            return View(priveligiado);
        }

        // GET: Priveligiados/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Priveligiados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("Data_Subscricao,IdUser,Username,Role,Data_Entrada,Imagem_Perfil")] Priveligiado priveligiado)
        {
            if (ModelState.IsValid)
            {
                // Atribuição de valores 
                priveligiado.Data_Entrada = DateOnly.FromDateTime(DateTime.Now); 
                priveligiado.Data_Subscricao = DateOnly.FromDateTime(DateTime.Now);
                priveligiado.Role = "Priveligiado";

                // Adição do Admin e salvaguarda dos seus dados na BD
                _context.Add(priveligiado);
                await _context.SaveChangesAsync();

                // Regresso ao Index
                return RedirectToAction(nameof(Index));
            }
            return View(priveligiado);
        }

        // GET: Priveligiados/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priveligiado = await _context.Priveligiado.FindAsync(id);
            if (priveligiado == null)
            {
                return NotFound();
            }
            return View(priveligiado);
        }

        // POST: Priveligiados/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("Data_Subscricao,IdUser,Username,Imagem_Perfil")] Priveligiado priveligiado)
        {
            if (id != priveligiado.IdUser)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Tenta encontrar o Priveligiado a editar através do seu ID
                    var atualPriveligiado = await _context.Priveligiado.FindAsync(id);
                    if (atualPriveligiado == null)
                    {
                        return NotFound();
                    }
                    // Atualização dos atributos editáveis
                    atualPriveligiado.Username = priveligiado.Username;
                    atualPriveligiado.Imagem_Perfil = priveligiado.Imagem_Perfil;

                    // Update dos atributos editáveis e salvaguarda das alterações para a BD
                    _context.Update(atualPriveligiado);
                    await _context.SaveChangesAsync();

                    // Regresso ao Index
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PriveligiadoExists(priveligiado.IdUser))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(priveligiado);
        }


        // GET: Priveligiados/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priveligiado = await _context.Priveligiado
                .FirstOrDefaultAsync(m => m.IdUser == id);
            if (priveligiado == null)
            {
                return NotFound();
            }

            return View(priveligiado);
        }

        // POST: Priveligiados/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var priveligiado = await _context.Priveligiado.FindAsync(id);
            if (priveligiado != null)
            {
                _context.Priveligiado.Remove(priveligiado);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PriveligiadoExists(int id)
        {
            return _context.Priveligiado.Any(e => e.IdUser == id);
        }
    }
}
