using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reading_Reviewys.Data;
using Reading_Reviewys.Models;

namespace Reading_Reviewys.Controllers
{
    public class LivrosController : Controller
    {
        /// <summary>
        /// Referência à BD do projeto
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Objecto que contém os dados do Servidor
        /// </summary>
        private readonly IWebHostEnvironment _webHostEnvironment;


        public LivrosController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Livros
        public async Task<IActionResult> Index()
        {
            return View(await _context.Livro.ToListAsync());
        }

        // GET: Livros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var livro = await _context.Livro
                .Include(l => l.ListaAutores)
                .Include(l => l.ListaPublicacao)
                .FirstOrDefaultAsync(m => m.IdLivro == id);
            if (livro == null)
            {
                return NotFound();
            }


            return View(livro);
        }

        // GET: Livros/Create
        [Authorize(Roles = "Comum,Priveligiado,Autor,Administrador")]
        public IActionResult Create()
        {
            // Obtenção dos Autores existentes na BD
            ViewData["ListaAutores"] = _context.Autor.OrderBy(p => p.Nome).ToList();

            return View();
        }

        // POST: Livros/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Comum,Priveligiado,Autor,Administrador")]
        public async Task<IActionResult> Create([Bind("IdLivro,Titulo,Genero,AnoPublicacao")] Livro livro, int[] listaIdsAutores, IFormFile ImagemCapa)
        {
            var listaAutores = new List<Autor>();
            foreach (var autId in listaIdsAutores)
            {
                var aut = _context.Autor.FirstOrDefault(p => p.IdUser == autId);

                if (aut != null)
                {
                    listaAutores.Add(aut);
                }
            }

            if (listaAutores != null)
            {
                livro.ListaAutores = listaAutores;
            }
            else
            {
                return View(livro);
            }

            if (ModelState.IsValid)
            {
                // Vars auxiliares
                string nomeImagem = "";
                bool haImagem = false;

                // Há ficheiro?
                if (ImagemCapa == null)
                {
                    // Não há, logo crio uma mensagem de erro
                    ModelState.AddModelError("", "Deve fornecer uma imagem para a capa!");

                    // Devolver controlo à View
                    return View(livro);
                }
                else
                {
                    // Há ficheiro, mas é uma imagem?
                    if (!(ImagemCapa.ContentType == "image/png" || ImagemCapa.ContentType == "image/jpeg"))
                    {
                        // Não, então vamos usar uma imagem pre-definida
                        livro.Capa = "CapaDefault.jpg";
                    }
                    else
                    {
                        // Há imagem
                        haImagem = true;

                        // Gerar nome imagem
                        Guid g = Guid.NewGuid();
                        nomeImagem = g.ToString();
                        string extensaoImagem = Path.GetExtension(ImagemCapa.FileName).ToLowerInvariant();
                        nomeImagem += extensaoImagem;

                        // Guardar o nome do ficheiro na BD
                        livro.Capa = nomeImagem;
                    }
                }

                _context.Add(livro);
                await _context.SaveChangesAsync();

                // Guardar a imagem do livro
                if (haImagem)
                {
                    // Determinar o local de armazenamento da imagem
                    string localizacaoImagem = _webHostEnvironment.WebRootPath;
                    // Adicionar à raiz da parte web, o nome da pasta onde queremos guardar as imagens
                    localizacaoImagem = Path.Combine(localizacaoImagem, "Imagens");

                    // Será que o local existe?
                    if (!Directory.Exists(localizacaoImagem))
                    {
                        Directory.CreateDirectory(localizacaoImagem);
                    }

                    // Atribuir ao caminho o nome da imagem
                    localizacaoImagem = Path.Combine(localizacaoImagem, nomeImagem);

                    // Guardar a imagem no Disco Rígido
                    using var stream = new FileStream(localizacaoImagem, FileMode.Create);
                    await ImagemCapa.CopyToAsync(stream);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(livro); 
        }

        // GET: Livros/Edit/5
        [Authorize(Roles = "Comum,Priveligiado,Autor,Administrador")]
            public async Task<IActionResult> Edit(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var livro = await _context.Livro.FindAsync(id);
                if (livro == null)
                {
                    return NotFound();
                }
                return View(livro);
            }

            // POST: Livros/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to.
            // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            [Authorize(Roles = "Comum,Priveligiado,Autor,Administrador")]
            public async Task<IActionResult> Edit(int id, [Bind("IdLivro,Capa,Titulo,Genero,AnoPublicacao")] Livro livro)
            {
                if (id != livro.IdLivro)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(livro);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!LivroExists(livro.IdLivro))
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
                return View(livro);
            }

            // GET: Livros/Delete/5
            [Authorize(Roles = "Comum,Priveligiado,Autor,Administrador")]
            public async Task<IActionResult> Delete(int? id)
            {
                if (id == null)
                {
                    return NotFound();
                }

                var livro = await _context.Livro
                    .FirstOrDefaultAsync(m => m.IdLivro == id);
                if (livro == null)
                {
                    return NotFound();
                }

                return View(livro);
            }

            // POST: Livros/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            [Authorize(Roles = "Comum,Priveligiado,Autor,Administrador")]
            public async Task<IActionResult> DeleteConfirmed(int id)
            {
                var livro = await _context.Livro.FindAsync(id);
                if (livro != null)
                {
                    _context.Livro.Remove(livro);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            private bool LivroExists(int id)
            {
                return _context.Livro.Any(e => e.IdLivro == id);
            }
        }
    }
