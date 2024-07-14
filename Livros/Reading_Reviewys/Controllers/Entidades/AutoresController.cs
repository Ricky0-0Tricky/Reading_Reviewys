using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reading_Reviewys.Data;
using Reading_Reviewys.Models;

namespace Reading_Reviewys.Controllers
{
    public class AutoresController : Controller
    {
        /// <summary>
        /// Objeto representativo da BD
        /// </summary>
        private readonly ApplicationDbContext _context;

        public AutoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Autores
        [Authorize(Roles = "Priveligiado, Autor, Administrador")]
        public async Task<IActionResult> Index()
        {
            // Retorna a lista de Autores
            return View(await _context.Autor.ToListAsync());
        }

        // GET: Autores/Details/5
        [Authorize(Roles = "Autor, Administrador")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var autor = await _context.Autor
                .FirstOrDefaultAsync(m => m.IdUser == id);
            if (autor == null)
            {
                return NotFound();
            }

            // Caso em que o Autor existe, devolve-se a View com os seus dados
            return View(autor);
        }

        // GET: Autores/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Autores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("Nome,IdUser,Username,Role,Data_Entrada,Imagem_Perfil")] Autor autor)
        {
            if (ModelState.IsValid)
            {
                // Atribuição de valores
                autor.Data_Entrada = DateOnly.FromDateTime(DateTime.Now);
                autor.Role = "Autor";

                // Adição do Autor e salvaguarda dos seus dados na BD
                _context.Add(autor);
                await _context.SaveChangesAsync();

                // Regresso ao Index
                return RedirectToAction(nameof(Index));
            }

            // Caso o Model seja inválido, devolve-se a View com os dados inseridos
            return View(autor);
        }

        // GET: Autores/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var autor = await _context.Autor.FindAsync(id);
            if (autor == null)
            {
                return NotFound();
            }

            return View(autor);
        }

        // POST: Autores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id, [Bind("IdUser,Nome,Username,Imagem_Perfil")] Autor autor)
        {
            if (id != autor.IdUser)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Tenta encontrar o Autor a editar através do seu ID
                    var atualAutor = await _context.Autor.FindAsync(id);
                    if (atualAutor == null)
                    {
                        return NotFound();
                    }

                    // Atualização dos atributos editáveis
                    atualAutor.Nome = autor.Nome;
                    atualAutor.Username = autor.Username;
                    atualAutor.Imagem_Perfil = autor.Imagem_Perfil;

                    // Update dos atributos editáveis e salvaguarda das alterações para a BD
                    _context.Update(atualAutor);
                    await _context.SaveChangesAsync();

                    // Regresso ao Index
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Caso em que o Autor não existe e devolve-se a página de "NotFound"
                    if (!AutorExists(autor.IdUser))
                    {
                        return NotFound();
                    }
                    // Caso em que o Autor existe mas houve lançamento de uma exceção
                    else
                    {
                        throw;
                    }
                }
            }

            // Caso o Model seja inválido, devolve-se a View com os dados inseridos
            return View(autor);
        }

        // GET: Autores/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var autor = await _context.Autor
                .FirstOrDefaultAsync(m => m.IdUser == id);
            if (autor == null)
            {
                return NotFound();
            }

            // Caso o Autor não seja encontrado, devolve-se a mesma View
            return View(autor);
        }

        // POST: Autores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var autor = await _context.Autor.FindAsync(id);
            if (autor != null)
            {
                _context.Autor.Remove(autor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Verifica se um Autor existe na BD
        /// </summary>
        /// <param name="id">ID do Autor</param>
        /// <returns>Verdadeiro se o Autor existir, Falso caso contrário</returns>
        private bool AutorExists(int id)
        {
            return _context.Autor.Any(e => e.IdUser == id);
        }
    }
}
