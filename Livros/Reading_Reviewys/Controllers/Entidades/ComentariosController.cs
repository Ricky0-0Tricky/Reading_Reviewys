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
        // <summary>
        /// Objeto representativo da BD
        /// </summary>
        private readonly ApplicationDbContext _context;

        // <summary>
        /// Objeto para interagir com os dados da pessoa autenticada
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

            // Obter ID da pessoa autenticada
            ViewData["UserID"] = _userManager.GetUserId(User);
            return View(await comentarios.ToListAsync());
        }

        // GET: Comentarios/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            // Obter ID da pessoa autenticada
            ViewData["UserID"] = _userManager.GetUserId(User);

            if (id == null)
            {
                return NotFound();
            }

            // Lista de Comentários
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
            // Estabelecer a data do comentário como a atual
            comentario.Data = DateTime.Now;

            // Obter ID da pessoa autenticada
            var userId = _userManager.GetUserId(User);

            // Preenchimento de Atributos relacionados com o Criador do Comentário
            comentario.CriadorComentario = await _context.Utilizador
                .Where(r => r.UserID == userId)
                .FirstOrDefaultAsync();
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

            // Caso o Model seja inválido, mostram-se alguns ViewDatas com info dos Utilizadores e Reviews
            ViewData["CriadorComentarioFK"] = new SelectList(_context.Utilizador, "IdUser", "Username", comentario.CriadorComentarioFK);
            ViewData["ReviewFK"] = new SelectList(_context.Reviews, "IdReview", "DescricaoReview", comentario.ReviewFK);
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

            // Obter ID da pessoa autenticada
            var userId = _userManager.GetUserId(User);

            // Obter os comentários do Utilizador
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
                    // Obter ID da pessoa autenticada
                    var userId = _userManager.GetUserId(User);

                    // Obter o comentário a ser editado
                    var atualComentario = await _context.Comentarios
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.Id == id && c.CriadorComentario.UserID == userId);

                    if (atualComentario == null)
                    {
                        return NotFound();
                    }

                    // Alterar alguns atributos do comentário
                    comentarios.ReviewFK = atualComentario.ReviewFK;
                    comentarios.CriadorComentarioFK = atualComentario.CriadorComentarioFK;
                    comentarios.Data = DateTime.Now;

                    _context.Update(comentarios);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Caso em que o comentário não existe
                    if (!ComentariosExists(comentarios.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        // Caso em que o comentário existe, mas houve uma exceção
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CriadorComentarioFK"] = new SelectList(_context.Utilizador, "IdUser", "Username", comentarios.CriadorComentarioFK);
            ViewData["ReviewFK"] = new SelectList(_context.Reviews, "IdReview", "DescricaoReview", comentarios.ReviewFK);
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

            // Obter ID da pessoa autenticada
            var userId = _userManager.GetUserId(User);

            // Obter o comentário a ser apagado
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
            // Obter ID da pessoa autenticada
            var userId = _userManager.GetUserId(User);

            // Encontrar o comentario a ser apagado e pertença ao Utilizador
            var comentario = await _context.Comentarios
                .Where(c => c.Id == id && c.CriadorComentario.UserID == userId)
                .FirstOrDefaultAsync();

            if (comentario == null)
            {
                return NotFound();
            }

            // Remoção completa do comentário da BD
            _context.Comentarios.Remove(comentario);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ComentariosExists(int id)
        {
            return _context.Comentarios.Any(e => e.Id == id);
        }

        // POST: AdicionarComentario
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AdicionarComentario(int IdReview, string Descricao)
        {
            // Obter ID da pessoa autenticada
            var userId = _userManager.GetUserId(User);

            // Caso em que o Utilizador não escreveu nada
            if (string.IsNullOrEmpty(Descricao))
            {
                ModelState.AddModelError("Descricao", "O comentário não pode estar vazio.");
                return RedirectToAction("Details", "Reviews", new { id = IdReview });
            }

            // Fetch do Utilizador na BD
            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(u => u.UserID == userId);

            if (utilizador == null)
            {
                // Caso em que o Utilizador não existe
                return NotFound("Utilizador Desconhecido.");
            }

            // Criação do comentário com valores adequadados
            var comentario = new Comentarios
            {
                ReviewFK = IdReview,
                CriadorComentarioFK = utilizador.IdUser,
                Descricao = Descricao,
                Data = DateTime.Now
            };

            // Adição do mesmo à BD
            _context.Comentarios.Add(comentario);
            await _context.SaveChangesAsync();

            // Redirect para a página onde o mesmo foi feito
            return RedirectToAction("Details", "Reviews", new { id = IdReview });
        }
    }
}
