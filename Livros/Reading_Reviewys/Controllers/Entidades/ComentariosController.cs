using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        // <summary>
        /// objeto para interagir com os dados da pessoa autenticada
        /// </summary>
        private readonly UserManager<IdentityUser> _userManager;

        public ComentariosController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Comentarios
        public async Task<IActionResult> Index()
        {
            var comentarios = _context.Comentarios.Include(c => c.CriadorComentario).Include(c => c.Review);
            // obter ID da pessoa autenticada
            ViewData["UserID"] = _userManager.GetUserId(User);
            return View(await comentarios.ToListAsync());
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
        public async Task<IActionResult> Create([Bind("Id,Data,Descricao,ReviewFK")] Comentarios comentario)
        {
            // Preenchimento da data de publicacao com a data atual 
            comentario.Data = DateTime.Now;
            // obter ID da pessoa autenticada
            var userId = _userManager.GetUserId(User);

            // Identificação do User com o seu respetivo ID
            comentario.CriadorComentarioFK = await _context.Utilizador
                                                    .Where(r => r.UserID == userId)
                                                    .Select(r => r.IdUser)
                                                    .FirstOrDefaultAsync();

            if (ModelState.IsValid)
            {
                _context.Add(comentario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Caso o Modelo não seja válido apresenta-se as chaves estrangeiras disponíveis
            ViewData["CriadorComentarioFK"] = new SelectList(_context.Utilizador, "IdUser", "Username", comentario.CriadorComentarioFK);
            ViewData["ReviewFK"] = new SelectList(_context.Reviews, "IdReview", "DescricaoReview", comentario.ReviewFK);

            // Fica-se na mesma View
            return View(comentario);
        }

        // GET: Comentarios/Edit/5
        [Authorize(Roles = "Comum,Priveligiado,Autor,Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // obter ID da pessoa autenticada
            var userId = _userManager.GetUserId(User);

            var comentario = await _context.Comentarios
                                           .Include(c => c.CriadorComentario)
                                           .Where(r => r.Id == id && r.CriadorComentario.UserID == userId)
                                           .FirstOrDefaultAsync();

            if (comentario == null)
            {
                return NotFound();
            }

            ViewData["CriadorComentarioFK"] = new SelectList(_context.Utilizador, "IdUser", "Username", comentario.CriadorComentarioFK);
            ViewData["ReviewFK"] = new SelectList(_context.Reviews, "IdReview", "DescricaoReview", comentario.ReviewFK);
            return View(comentario);
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
                    // obter ID da pessoa autenticada
                    var userId = _userManager.GetUserId(User);

                    // Pesquisa do comentário na BD
                    var atualComentario = await _context.Comentarios
                                                        .AsNoTracking()
                                                        .FirstOrDefaultAsync(c => c.Id == id && c.CriadorComentario.UserID == userId);

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
            // obter ID da pessoa autenticada
            var userId = _userManager.GetUserId(User);

            var comentario = await _context.Comentarios
                                           .Include(c => c.CriadorComentario)
                                           .Where(r => r.Id == id && r.CriadorComentario.UserID == userId)
                                           .FirstOrDefaultAsync();
            if (comentario == null)
            {
                return NotFound();
            }
            return View(comentario);
        }

        // POST: Comentarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Comum,Priveligiado,Autor,Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // obter ID da pessoa autenticada
            var userId = _userManager.GetUserId(User);

            var comentario = await _context.Comentarios
                                           .Where(c => c.Id == id && c.CriadorComentario.UserID == userId)
                                           .FirstOrDefaultAsync();

            if (comentario == null)
            {
                return NotFound();
            }

            _context.Comentarios.Remove(comentario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ComentariosExists(int id)
        {
            return _context.Comentarios.Any(e => e.Id == id);
        }
    }
}
