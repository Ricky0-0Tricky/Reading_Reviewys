using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        // <summary>
        /// Objeto representativo da BD
        /// </summary>
        private readonly ApplicationDbContext _context;

        // <summary>
        /// Objeto para interagir com os dados da pessoa autenticada
        /// </summary>
        private readonly UserManager<IdentityUser> _userManager;

        public ReviewsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Reviews
        public async Task<IActionResult> Index()
        {
            // Receber as reviews existentes na BD
            var reviews = _context.Reviews.Include(r => r.Livro).Include(r => r.Utilizador);

            // Obter ID da pessoa autenticada
            ViewData["UserID"] = _userManager.GetUserId(User);

            // Retornar a View com a lista das reviews na BD
            return View(await reviews.ToListAsync());
        }

        // GET: Reviews/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            // Obter ID da pessoa autenticada
            ViewData["UserID"] = _userManager.GetUserId(User);

            if (id == null)
            {
                return NotFound();
            }

            // Obter as reviews realizadas a um dado livro
            var reviews = await _context.Reviews
                .Include(r => r.Livro)
                .Include(r => r.ListaComentarios)
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
        public async Task<IActionResult> Create([Bind("IdReview,DescricaoReview,DataAlteracao,LivroFK")] Reviews review)
        {
            // Prenchimento da Data de Alteração como o DataTime atual
            review.DataAlteracao = DateOnly.FromDateTime(DateTime.Now);

            // Obter ID da pessoa autenticada
            var userId = _userManager.GetUserId(User);

            // Identificação do User com o seu respetivo ID
            review.UtilizadorFK = await _context.Utilizador
                                        .Where(r => r.UserID == userId)
                                        .Select(r => r.IdUser)
                                        .FirstOrDefaultAsync();
            if (ModelState.IsValid)
            {
                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Livros", new { id = review.LivroFK });
            }
            ViewBag.LivroFK = new SelectList(_context.Livro, "IdLivro", "Titulo", review.LivroFK);
            ViewBag.UtilizadorFK = new SelectList(_context.Utilizador, "IdUser", "Username", review.UtilizadorFK);
            return View(review);
        }

        // GET: Reviews/Edit/5
        [Authorize(Roles = "Comum,Priveligiado,Autor,Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Obter ID da pessoa autenticada
            var userId = _userManager.GetUserId(User);

            // Obter Reviews do Utilizador
            var review = await _context.Reviews
                                       .Where(r => r.IdReview == id && r.Utilizador.UserID == userId)
                                       .FirstOrDefaultAsync();

            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Reviews/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Comum,Priveligiado,Autor,Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("IdReview,DescricaoReview,UtilizadorFK,LivroFK")] Reviews review)
        {
            if (id != review.IdReview)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Obter ID da pessoa autenticada
                    var userId = _userManager.GetUserId(User);

                    // Buscar a Review Existente
                    var atualReview = await _context.Reviews
                            .AsNoTracking()
                            .FirstOrDefaultAsync(r => r.IdReview == id && r.Utilizador.UserID == userId);

                    if (atualReview == null)
                    {
                        return NotFound();
                    }

                    // Preservação das chaves estrangeiras
                    review.UtilizadorFK = atualReview.UtilizadorFK;
                    review.LivroFK = atualReview.LivroFK;

                    // Atualização da data de alteração 
                    review.DataAlteracao = DateOnly.FromDateTime(DateTime.Now);

                    // Atualização da Review na BD
                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewsExists(review.IdReview))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Livros", new { id = review.LivroFK });
            }

            return View(review);
        }

        // GET: Reviews/Delete/5
        [Authorize(Roles = "Comum,Priveligiado,Autor,Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            // Obter ID da pessoa autenticada
            var userId = _userManager.GetUserId(User);

            if (id == null)
            {
                return NotFound();
            }

            // Obter Review a ser apagada
            var reviews = await _context.Reviews
                .Include(r => r.Livro)
                .Include(r => r.Utilizador)
                .FirstOrDefaultAsync(m => m.IdReview == id && m.Utilizador.UserID == userId);

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
            // Obter Review a ser apagada
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                // Captura da FK do Livro antes do apagamento da review
                var livroId = review.LivroFK;

                // Apagamento da Review e salvaguarda para a BD
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();

                // Redirect para a página do livro onde a mesma se enquadrava
                return RedirectToAction("Details", "Livros", new { id = livroId });
            }
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se uma Review existe na BD
        /// </summary>
        /// <param name="id">ID da Review</param>
        /// <returns>Verdadeiro se a Review existir, Falso caso contrário</returns>
        private bool ReviewsExists(int id)
        {
            return _context.Reviews.Any(e => e.IdReview == id);
        }
    }
}
