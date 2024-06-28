using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Reading_Reviewys.Data;
using Reading_Reviewys.Models;

namespace Reading_Reviewys.Controllers
{
    public class ComentariosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ComentariosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Comentarios
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Comentarios.Include(c => c.CriadorComentario).Include(c => c.Review);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Comentarios/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comentarios = await _context.Comentarios
                .Include(c => c.CriadorComentario)
                .Include(c => c.Review)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comentarios == null)
            {
                return NotFound();
            }

            return View(comentarios);
        }

        // GET: Comentarios/Create
        [Authorize(Roles = "Comum,Priveligiado,Autor,Administrador")]
        public IActionResult Create()
        {
            ViewData["CriadorComentarioFK"] = new SelectList(_context.Utilizador, "IdUser", "Username");
            ViewData["ReviewFK"] = new SelectList(_context.Reviews, "IdReview", "DescricaoReview");
            return View();
        }

        // POST: Comentarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Comum,Priveligiado,Autor,Administrador")]
        public async Task<IActionResult> Create([Bind("Id,Data,Descricao,ReviewFK,CriadorComentarioFK")] Comentarios comentarios)
        {
            // Preenchimento da data de publicacao com a data atual 
            comentarios.Data = DateTime.Now;

            if (ModelState.IsValid)
            {
                _context.Add(comentarios);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Caso o Modelo não seja válido apresenta-se as chaves estrangeiras disponíveis
            ViewData["CriadorComentarioFK"] = new SelectList(_context.Utilizador, "IdUser", "Username", comentarios.CriadorComentarioFK);
            ViewData["ReviewFK"] = new SelectList(_context.Reviews, "IdReview", "DescricaoReview", comentarios.ReviewFK);
            
            // Fica-se na mesma View
            return View(comentarios);
        }

        // GET: Comentarios/Edit/5
        [Authorize(Roles = "Comum,Priveligiado,Autor,Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comentarios = await _context.Comentarios.FindAsync(id);
            if (comentarios == null)
            {
                return NotFound();
            }
            ViewData["CriadorComentarioFK"] = new SelectList(_context.Utilizador, "IdUser", "Username", comentarios.CriadorComentarioFK);
            ViewData["ReviewFK"] = new SelectList(_context.Reviews, "IdReview", "DescricaoReview", comentarios.ReviewFK);
            return View(comentarios);
        }

        // POST: Comentarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Comum,Priveligiado,Autor,Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Data,Descricao")] Comentarios comentarios)
        {
            if (id != comentarios.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Pesquisa do comentário na BD
                    var atualComentario = await _context.Comentarios
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == id);

                    if (atualComentario == null)
                    {
                        return NotFound();
                    }

                    // Preservação das Chaves Estrangeiras
                    comentarios.ReviewFK = atualComentario.ReviewFK;
                    comentarios.CriadorComentarioFK = atualComentario.CriadorComentarioFK;

                    // Atualização da data
                    comentarios.Data = DateTime.Now;

                    // Atualização do Comentário na BD
                    _context.Update(comentarios);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComentariosExists(comentarios.Id))
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

            // Caso o Modelo não seja válido apresenta-se as chaves estrangeiras disponíveis
            ViewData["CriadorComentarioFK"] = new SelectList(_context.Utilizador, "IdUser", "Username", comentarios.CriadorComentarioFK);
            ViewData["ReviewFK"] = new SelectList(_context.Reviews, "IdReview", "DescricaoReview", comentarios.ReviewFK);

            // Fica-se na mesma View
            return View(comentarios);
        }


        // GET: Comentarios/Delete/5
        [Authorize(Roles = "Comum,Priveligiado,Autor,Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comentarios = await _context.Comentarios
                .Include(c => c.CriadorComentario)
                .Include(c => c.Review)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comentarios == null)
            {
                return NotFound();
            }

            return View(comentarios);
        }

        // POST: Comentarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Comum,Priveligiado,Autor,Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comentarios = await _context.Comentarios.FindAsync(id);
            if (comentarios != null)
            {
                _context.Comentarios.Remove(comentarios);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ComentariosExists(int id)
        {
            return _context.Comentarios.Any(e => e.Id == id);
        }
    }
}
