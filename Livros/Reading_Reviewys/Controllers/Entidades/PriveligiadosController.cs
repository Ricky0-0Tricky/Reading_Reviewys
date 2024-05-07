using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reading_Reviewys.Data;
using Reading_Reviewys.Models;

namespace Reading_Reviewys.Controllers
{
    public class PriveligiadosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PriveligiadosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Priveligiados
        public async Task<IActionResult> Index()
        {
            return View(await _context.Priveligiado.ToListAsync());
        }

        // GET: Priveligiados/Details/5
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Priveligiados/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Data_Subscricao,IdUser,Username,Role,Data_Entrada,Imagem_Perfil")] Priveligiado priveligiado)
        {
            if (ModelState.IsValid)
            {
                _context.Add(priveligiado);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(priveligiado);
        }

        // GET: Priveligiados/Edit/5
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
        public async Task<IActionResult> Edit(int id, [Bind("Data_Subscricao,IdUser,Username,Role,Data_Entrada,Imagem_Perfil")] Priveligiado priveligiado)
        {
            if (id != priveligiado.IdUser)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(priveligiado);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
            }
            return View(priveligiado);
        }

        // GET: Priveligiados/Delete/5
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
