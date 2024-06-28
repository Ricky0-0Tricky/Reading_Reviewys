using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Reading_Reviewys.Data;
using Reading_Reviewys.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Reading_Reviewys.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reviews
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Reviews.Include(r => r.Livro).Include(r => r.Utilizador);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Reviews/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reviews = await _context.Reviews
                .Include(r => r.Livro)
                .Include(r => r.Utilizador)
                .FirstOrDefaultAsync(m => m.IdReview == id);
            if (reviews == null)
            {
                return NotFound();
            }

            return View(reviews);
        }

        // GET: Reviews/Create
        [Authorize(Roles = "Comum,Priveligiado,Autor,Administrador")]
        public IActionResult Create()
        {
            ViewBag.LivroFK = new SelectList(_context.Livro, "IdLivro", "Titulo");
            ViewBag.UtilizadorFK = new SelectList(_context.Utilizador, "IdUser", "Username");
            return View();
        }

        // POST: Reviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Comum,Priveligiado,Autor,Administrador")]
        public async Task<IActionResult> Create([Bind("IdReview,DescricaoReview,DataAlteracao,UtilizadorFK,LivroFK")] Reviews reviews)
        {
            // Prenchimento da Data de Alteração como o DataTime atual
            reviews.DataAlteracao = DateOnly.FromDateTime(DateTime.Now);

            if (ModelState.IsValid)
            {
                _context.Add(reviews);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.LivroFK = new SelectList(_context.Livro, "IdLivro", "Titulo", reviews.LivroFK);
            ViewBag.UtilizadorFK = new SelectList(_context.Utilizador, "IdUser", "Username", reviews.UtilizadorFK);
            return View(reviews);
        }

        // GET: Reviews/Edit/5
        [Authorize(Roles = "Comum,Priveligiado,Autor,Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reviews = await _context.Reviews.FindAsync(id);
            if (reviews == null)
            {
                return NotFound();
            }
            ViewBag.LivroFK = new SelectList(_context.Livro, "IdLivro", "Titulo", reviews.LivroFK);
            ViewBag.UtilizadorFK = new SelectList(_context.Utilizador, "IdUser", "Username", reviews.UtilizadorFK);
            return View(reviews);
        }

        // POST: Reviews/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Comum,Priveligiado,Autor,Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("IdReview,DescricaoReview")] Reviews reviews)
        {
            if (id != reviews.IdReview)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Buscar a Review Existente
                    var atualReview = await _context.Reviews
                        .AsNoTracking()
                        .FirstOrDefaultAsync(r => r.IdReview == id);

                    if (atualReview == null)
                    {
                        return NotFound();
                    }

                    // Preservação das chaves estrangeiras
                    reviews.UtilizadorFK = atualReview.UtilizadorFK;
                    reviews.LivroFK = atualReview.LivroFK;

                    // Atualização da data de alteração 
                    reviews.DataAlteracao = DateOnly.FromDateTime(DateTime.Now);

                    // Atualização da Review na BD
                    _context.Update(reviews);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewsExists(reviews.IdReview))
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

            ViewBag.LivroFK = new SelectList(_context.Livro, "IdLivro", "Titulo", reviews.LivroFK);
            ViewBag.UtilizadorFK = new SelectList(_context.Utilizador, "IdUser", "Username", reviews.UtilizadorFK);
            return View(reviews);
        }

        // GET: Reviews/Delete/5
        [Authorize(Roles = "Comum,Priveligiado,Autor,Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reviews = await _context.Reviews
                .Include(r => r.Livro)
                .Include(r => r.Utilizador)
                .FirstOrDefaultAsync(m => m.IdReview == id);
            if (reviews == null)
            {
                return NotFound();
            }

            return View(reviews);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Comum,Priveligiado,Autor,Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reviews = await _context.Reviews.FindAsync(id);
            if (reviews != null)
            {
                _context.Reviews.Remove(reviews);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewsExists(int id)
        {
            return _context.Reviews.Any(e => e.IdReview == id);
        }
    }
}
