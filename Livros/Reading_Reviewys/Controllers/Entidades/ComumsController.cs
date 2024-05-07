using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reading_Reviewys.Data;
using Reading_Reviewys.Models;

namespace Reading_Reviewys.Controllers
{
    public class ComumsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ComumsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Comums
        public async Task<IActionResult> Index()
        {
            return View(await _context.Comum.ToListAsync());
        }

        // GET: Comums/Details/5
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

            return View(comum);
        }

        // GET: Comums/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Comums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdUser,Username,Role,Data_Entrada,Imagem_Perfil")] Comum comum)
        {
            if (ModelState.IsValid)
            {
                _context.Add(comum);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(comum);
        }

        // GET: Comums/Edit/5
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
        public async Task<IActionResult> Edit(int id, [Bind("IdUser,Username,Role,Data_Entrada,Imagem_Perfil")] Comum comum)
        {
            if (id != comum.IdUser)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComumExists(comum.IdUser))
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
            return View(comum);
        }

        // GET: Comums/Delete/5
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

            return View(comum);
        }

        // POST: Comums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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

        private bool ComumExists(int id)
        {
            return _context.Comum.Any(e => e.IdUser == id);
        }
    }
}
