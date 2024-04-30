using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Reading_Reviewys.Data;
using Reading_Reviewys.Models;

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
        public IActionResult Create()
        {
            ViewData["LivroFK"] = new SelectList(_context.Livro, "IdLivro", "IdLivro");
            ViewData["UtilizadorFK"] = new SelectList(_context.Utilizador, "IdUser", "Discriminator");
            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdReview,DescricaoReview,DataAlteracao,UtilizadorFK,LivroFK")] Reviews reviews)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reviews);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LivroFK"] = new SelectList(_context.Livro, "IdLivro", "IdLivro", reviews.LivroFK);
            ViewData["UtilizadorFK"] = new SelectList(_context.Utilizador, "IdUser", "Discriminator", reviews.UtilizadorFK);
            return View(reviews);
        }

        // GET: Reviews/Edit/5
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
            ViewData["LivroFK"] = new SelectList(_context.Livro, "IdLivro", "IdLivro", reviews.LivroFK);
            ViewData["UtilizadorFK"] = new SelectList(_context.Utilizador, "IdUser", "Discriminator", reviews.UtilizadorFK);
            return View(reviews);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdReview,DescricaoReview,DataAlteracao,UtilizadorFK,LivroFK")] Reviews reviews)
        {
            if (id != reviews.IdReview)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
            ViewData["LivroFK"] = new SelectList(_context.Livro, "IdLivro", "IdLivro", reviews.LivroFK);
            ViewData["UtilizadorFK"] = new SelectList(_context.Utilizador, "IdUser", "Discriminator", reviews.UtilizadorFK);
            return View(reviews);
        }

        // GET: Reviews/Delete/5
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
