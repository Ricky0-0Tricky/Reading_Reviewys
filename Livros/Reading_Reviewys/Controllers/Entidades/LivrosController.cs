using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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

        // <summary>
        /// Objeto para interagir com os dados da pessoa autenticada
        /// </summary>
        private readonly UserManager<IdentityUser> _userManager;


        public LivrosController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
        }

        // GET: Livros
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Livro.ToListAsync());
        }

        // GET: Livros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // Obter ID da pessoa autenticada
            ViewData["UserID"] = _userManager.GetUserId(User);

            if (id == null)
            {
                return NotFound();
            }

            // Obter atributos relativos ao Livro
            var livro = await _context.Livro
                .Include(l => l.ListaAutores)
                .Include(l => l.ListaPublicacao)
                .ThenInclude(l => l.Utilizador)
                .FirstOrDefaultAsync(m => m.IdLivro == id);
            if (livro == null)
            {
                return NotFound();
            }


            return View(livro);
        }

        // GET: Livros/Create
        [Authorize(Roles = "Autor,Administrador")]
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
        [Authorize(Roles = "Autor,Administrador")]
        public async Task<IActionResult> Create([Bind("IdLivro,Titulo,Genero,AnoPublicacao")] Livro livro, int[] listaIdsAutores, IFormFile ImagemCapa)
        {
            // Criação de uma lista de autores
            var listaAutores = new List<Autor>();
            
            // Iterar sobre todos os IDs na lista de IDs de Autores
            foreach (var autId in listaIdsAutores)
            {
                // Captura de um autor na BD
                var aut = _context.Autor.FirstOrDefault(p => p.IdUser == autId);

                // Caso um seja encontrado adiciona-se à lista
                if (aut != null)
                {
                    listaAutores.Add(aut);
                }
            }

            // Caso em que existem autores do livro
            if (listaAutores != null)
            {
                // Determinação dos Autores encontrados para os Autores do Livro
                livro.ListaAutores = listaAutores;
            }
            else
            {
                // Senão retorna-se a View do Livro
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

                // Adição do Livro à BD
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
        [Authorize(Roles = "Autor,Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Encontrar Livro a ser editado
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
        [Authorize(Roles = "Autor,Administrador")]
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
                    // Update do Livro na BD
                    _context.Update(livro);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Caso em que o Livro não existe
                    if (!LivroExists(livro.IdLivro))
                    {
                        return NotFound();
                    }
                    else
                    {
                        // Caso em que o Livro existe, mas algo correu mal
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(livro);
        }

        // GET: Livros/Delete/5
        [Authorize(Roles = "Autor,Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Encontrar o livro a ser apagado
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
        [Authorize(Roles = "Autor,Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Encontrar o Livro a ser apagado
            var livro = await _context.Livro.FindAsync(id);
            if (livro != null)
            {
                _context.Livro.Remove(livro);
            }

            // Salvar as alterações realizadas
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        /// <summary>
        /// Verifica se um Livro existe na BD
        /// </summary>
        /// <param name="id">ID do Livro</param>
        /// <returns>Verdadeiro se o Livro existir, Falso caso contrário</returns>
        private bool LivroExists(int id)
        {
            return _context.Livro.Any(e => e.IdLivro == id);
        }
    }
}
