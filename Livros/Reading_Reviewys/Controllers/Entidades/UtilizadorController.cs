using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Reading_Reviewys.Data;
using Reading_Reviewys.Models;
using System.Data;


namespace Reading_Reviewys.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UtilizadorController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// objecto que contém os dados do Servidor
        /// </summary>
        private readonly IWebHostEnvironment _webHostEnvironment;

        public UtilizadorController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Utilizador
        public async Task<IActionResult> Index()
        {
            return View(await _context.Utilizador.ToListAsync());
        }

        // GET: Utilizador/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(m => m.IdUser == id);
            if (utilizador == null)
            {
                return NotFound();
            }

            return View(utilizador);
        }

        // GET: Utilizador/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Utilizador/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdUser,Username,Role")] Utilizador utilizador, IFormFile Imagem_Perfil)
        {
            if (ModelState.IsValid)
            {
                // Submissão da Imagem

                // Vars Auxiliares
                string nomeImagem = "";
                bool haImagem = false;

                if (Imagem_Perfil != null)
                {
                    // Validação do Formato do Documento submetido
                    if (Imagem_Perfil.ContentType == "image/png" || Imagem_Perfil.ContentType == "image/jpeg")
                    {
                        // Geração de um nome único
                        Guid g = Guid.NewGuid();
                        nomeImagem = g.ToString();
                        string extensaoImagem = Path.GetExtension(Imagem_Perfil.FileName).ToLowerInvariant();
                        nomeImagem += extensaoImagem;

                        // Salvar a Imagem para o Servidor
                        string localizacaoImagem = Path.Combine(_webHostEnvironment.WebRootPath, "Imagens");
                        if (!Directory.Exists(localizacaoImagem))
                        {
                            Directory.CreateDirectory(localizacaoImagem);
                        }
                        localizacaoImagem = Path.Combine(localizacaoImagem, nomeImagem);
                        using (var stream = new FileStream(localizacaoImagem, FileMode.Create))
                        {
                            await Imagem_Perfil.CopyToAsync(stream);
                        }

                        // Indicação que existe imagem
                        haImagem = true;
                    }
                    else
                    {
                        // Caso de Formato Inválido onde se retorna a View com os dados do Utilizador 
                        ModelState.AddModelError(string.Empty, "O formato da imagem não é suportado. Por favor, escolha um formato válido");
                        return View(utilizador);
                    }
                }
                else
                {
                    // Caso de Falta de Imagem onde se define a imagem default
                    utilizador.Imagem_Perfil = "/default.jpg";
                }

                // Criação do Identity User
                var utilIdentity = new IdentityUser
                {
                    UserName = utilizador.Username,
                    Email = "useradmin@hotmail.com",
                    EmailConfirmed = true
                };

                // Hash da Password Default
                var passHasher = new PasswordHasher<IdentityUser>();
                utilIdentity.PasswordHash = passHasher.HashPassword(utilIdentity, "Abcd1_");

                // Tentativa da Submissão do Utilizador na tabela "AspNetUsers"
                var tentAsp = await _userManager.CreateAsync(utilIdentity);
                if (tentAsp.Succeeded)
                {
                    // Link entre os Utilizadores de tabelas diferentes
                    utilizador.UserID = utilIdentity.Id;

                    // Criação do Utilizador na tabela "Utilizadores" segundo o Role escolhido e Atribuição de Permissões
                    switch (utilizador.Role)
                    {
                        case "Comum":
                            await _userManager.AddToRoleAsync(utilIdentity, "Comum");
                            var comum = new Comum
                            {
                                IdUser = utilizador.IdUser,
                                UserID = utilIdentity.Id,
                                Username = utilizador.Username,
                                Role = "Comum",
                                Imagem_Perfil = haImagem ? nomeImagem : "/default.jpg",
                                Data_Entrada = DateOnly.FromDateTime(DateTime.Now)
                            };
                            _context.Add(comum);
                            break;
                        case "Priveligiado":
                            await _userManager.AddToRoleAsync(utilIdentity, "Priveligiado");
                            var priveligiado = new Priveligiado
                            {
                                IdUser = utilizador.IdUser,
                                UserID = utilIdentity.Id,
                                Username = utilizador.Username,
                                Role = "Priveligiado",
                                Imagem_Perfil = haImagem ? nomeImagem : "/default.jpg",
                                Data_Entrada = DateOnly.FromDateTime(DateTime.Now),
                                Data_Subscricao = DateOnly.FromDateTime(DateTime.Now)
                            };
                            _context.Add(priveligiado);
                            break;
                        case "Autor":
                            await _userManager.AddToRoleAsync(utilIdentity, "Autor");
                            var autor = new Autor
                            {
                                IdUser = utilizador.IdUser,
                                UserID = utilIdentity.Id,
                                Username = utilizador.Username,
                                Nome = "Autor_Default",
                                Role = "Autor",
                                Imagem_Perfil = haImagem ? nomeImagem : "/default.jpg",
                                Data_Entrada = DateOnly.FromDateTime(DateTime.Now)
                            };
                            _context.Add(autor);
                            break;
                        case "Admin":
                            await _userManager.AddToRoleAsync(utilIdentity, "Administrador");
                            var admin = new Admin
                            {
                                IdUser = utilizador.IdUser,
                                UserID = utilIdentity.Id,
                                Username = utilizador.Username,
                                Email = "admindefault@hotmail.com",
                                Role = "Admin",
                                Imagem_Perfil = haImagem ? nomeImagem : "/default.jpg",
                                Data_Entrada = DateOnly.FromDateTime(DateTime.Now)
                            };
                            _context.Add(admin);
                            break;
                        default:
                            await _userManager.AddToRoleAsync(utilIdentity, "Comum");
                            var utilErro = new Comum
                            {
                                IdUser = utilizador.IdUser,
                                UserID = utilIdentity.Id,
                                Username = utilizador.Username,
                                Role = "Comum",
                                Imagem_Perfil = haImagem ? nomeImagem : "/default.jpg",
                                Data_Entrada = DateOnly.FromDateTime(DateTime.Now)
                            };
                            _context.Add(utilErro);
                            break;
                    }

                    // Guardar as alterações realizadas
                    await _context.SaveChangesAsync();

                    // Regressar ao Index
                    return RedirectToAction(nameof(Index));
                }
            }
            // Caso o Modelo seja inválido, retorna-se a View com os dados do Utilizador 
            return View(utilizador);
        }


        // GET: Utilizador/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizador.FindAsync(id);
            if (utilizador == null)
            {
                return NotFound();
            }
            return View(utilizador);
        }

        // POST: Utilizador/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdUser,Username,Role,Imagem_Perfil")] Utilizador utilizador, IFormFile Imagem_Perfil)
        {
            if (id != utilizador.IdUser)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Tenta encontrar o utilizador na tabela "Utilizador"
                    var existeUtilizador = await _context.Utilizador.FindAsync(utilizador.IdUser);

                    // Se o mesmo não existir retorna-se a página de NotFound
                    if (existeUtilizador == null)
                    {
                        return NotFound();
                    }

                    // Identity User correspondente ao Utilizador com o mesmo UserID
                    var identAtual = await _userManager.FindByIdAsync(existeUtilizador.UserID);

                    // Verificação se o Role foi alterado ou não
                    if (existeUtilizador.Role != utilizador.Role)
                    {
                        switch (utilizador.Role)
                        {
                            case "Comum":
                                // Atualização do Role e Username na tabela "AspNetUsers"
                                await _userManager.AddToRoleAsync(identAtual, "Comum");
                                identAtual.UserName = utilizador.Username;
                                identAtual.NormalizedUserName = utilizador.Username.ToUpper();

                                var comum = new Comum
                                {
                                    IdUser = existeUtilizador.IdUser,
                                    UserID = existeUtilizador.UserID,
                                    Username = utilizador.Username,
                                    Role = utilizador.Role,
                                    Imagem_Perfil = existeUtilizador.Imagem_Perfil,
                                    Data_Entrada = existeUtilizador.Data_Entrada
                                };

                                // Remoção do Utilizador Antigo na tabela "Utilizadores"
                                _context.Remove(existeUtilizador);

                                _context.Add(comum);
                                break;
                            case "Priveligiado":
                                // Atualização do Role e Username na tabela "AspNetUsers"
                                await _userManager.AddToRoleAsync(identAtual, "Priveligiado");
                                identAtual.UserName = utilizador.Username;
                                identAtual.NormalizedUserName = utilizador.Username.ToUpper();

                                var priveligiado = new Priveligiado
                                {
                                    IdUser = existeUtilizador.IdUser,
                                    UserID = existeUtilizador.UserID,
                                    Username = utilizador.Username,
                                    Role = utilizador.Role,
                                    Imagem_Perfil = existeUtilizador.Imagem_Perfil,
                                    Data_Entrada = existeUtilizador.Data_Entrada,
                                    Data_Subscricao = DateOnly.FromDateTime(DateTime.Now)
                                };
                                // Remoção do Utilizador Antigo na tabela "Utilizadores"
                                _context.Remove(existeUtilizador);

                                _context.Add(priveligiado);
                                break;
                            case "Autor":
                                // Atualização do Role e Username na tabela "AspNetUsers"
                                await _userManager.AddToRoleAsync(identAtual, "Autor");
                                identAtual.UserName = utilizador.Username;
                                identAtual.NormalizedUserName = utilizador.Username.ToUpper();

                                var autor = new Autor
                                {
                                    IdUser = existeUtilizador.IdUser,
                                    UserID = existeUtilizador.UserID,
                                    Username = utilizador.Username,
                                    Nome = "Autor_Default",
                                    Role = utilizador.Role,
                                    Imagem_Perfil = existeUtilizador.Imagem_Perfil,
                                    Data_Entrada = existeUtilizador.Data_Entrada
                                };
                                // Remoção do Utilizador Antigo na tabela "Utilizadores"
                                _context.Remove(existeUtilizador);

                                _context.Add(autor);
                                break;
                            case "Admin":
                                // Atualização do Role e Username na tabela "AspNetUsers"
                                await _userManager.AddToRoleAsync(identAtual, "Administrador");
                                identAtual.UserName = utilizador.Username;
                                identAtual.NormalizedUserName = utilizador.Username.ToUpper();

                                var admin = new Admin
                                {
                                    IdUser = existeUtilizador.IdUser,
                                    UserID = existeUtilizador.UserID,
                                    Username = utilizador.Username,
                                    Email = "useradmin@hotmail.com",
                                    Role = utilizador.Role,
                                    Imagem_Perfil = existeUtilizador.Imagem_Perfil,
                                    Data_Entrada = existeUtilizador.Data_Entrada
                                };
                                _context.Add(admin);

                                // Remoção do Utilizador Antigo na tabela "Utilizadores"
                                _context.Remove(existeUtilizador);
                                break;
                            default:
                                // Não se faz nada
                                break;
                        }
                    }
                    else
                    {
                        // Atualizar o Username na tabela "Utilizadores" e "AspNetUsers"
                        existeUtilizador.Username = utilizador.Username;
                        identAtual.UserName = utilizador.Username;
                        identAtual.NormalizedUserName = utilizador.Username.ToUpper();

                        // Update dos dados na tabela "Utilizadores"
                        _context.Update(existeUtilizador);
                    }

                    // Atualizar a imagem 
                    if (Imagem_Perfil != null)
                    {
                        if (Imagem_Perfil.ContentType == "image/png" || Imagem_Perfil.ContentType == "image/jpeg")
                        {
                            // Originar um nome único para a Imagem
                            Guid g = Guid.NewGuid();
                            string nomeImagem = g.ToString() + Path.GetExtension(Imagem_Perfil.FileName).ToLowerInvariant();

                            // Salvar o Caminho da Imagem
                            string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "Imagens", nomeImagem);

                            using (var stream = new FileStream(imagePath, FileMode.Create))
                            {
                                await Imagem_Perfil.CopyToAsync(stream);
                            }

                            // Update do Caminho da Imagem
                            utilizador.Imagem_Perfil = nomeImagem;
                            existeUtilizador.Imagem_Perfil = utilizador.Imagem_Perfil;

                            // Update do Utilizador na BD
                            _context.Update(existeUtilizador);
                        }
                        else
                        {
                            // Mantem se a imagem original
                            return RedirectToAction(nameof(Index));
                        }
                    }

                    // Guardar as alterações realizadas para a BD
                    await _context.SaveChangesAsync();

                    // Transferir as Reviews e Comentários para o novo utilizador
                    if (existeUtilizador.Role != utilizador.Role)
                    {
                        // Buscar todos os comentários do Utilizador Antigo
                        var comentsAntigos = await _context.Comentarios.Where(c => c.CriadorComentarioFK == existeUtilizador.IdUser).ToListAsync();

                        // Atualizar o IdUser dos Comentários para o novo utilizador
                        foreach (var comentario in comentsAntigos)
                        {
                            comentario.CriadorComentarioFK = utilizador.IdUser;
                            _context.Update(comentario);
                        }

                        // Guardar as alterações realizadas
                        await _context.SaveChangesAsync();

                        // Buscar todas as reviews do Utilizador Antigo
                        var revwsAntigas = await _context.Reviews.Where(r => r.UtilizadorFK == existeUtilizador.IdUser).ToListAsync();

                        // Atualizar o IdUser das Reviews para o novo utilizador
                        foreach (var review in revwsAntigas)
                        {
                            review.UtilizadorFK = utilizador.IdUser;
                            _context.Update(review);
                        }

                        // Guardar as alterações realizadas
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UtilizadorExists(utilizador.IdUser))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                // Regressa-se ao Index
                return RedirectToAction(nameof(Index));
            }

            // Se o Model não for válido, retorna-se a mesma View com os dados preenchidos
            return View(utilizador);
        }

        // GET: Utilizador/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var utilizador = await _context.Utilizador
                .FirstOrDefaultAsync(m => m.IdUser == id);
            if (utilizador == null)
            {
                return NotFound();
            }

            return View(utilizador);
        }

        // POST: Utilizador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Captura do utilizador segundo o seu id
            var utilizador = await _context.Utilizador.FindAsync(id);

            // Caso não seja encontrado nenhum utilizador retorna-se a página de NotFound
            if (utilizador == null)
            {
                return NotFound();
            }

            // Tentativa de apagar o utilizador das tabelas "Utilizador" e "AspNetUsers"
            try
            {
                // Apagar os comentários do utilizador
                var comentarios = await _context.Comentarios
                                            .Where(c => c.CriadorComentarioFK == utilizador.IdUser)
                                            .ToListAsync();
                _context.Comentarios.RemoveRange(comentarios);

                // Apagar reviews do utilizador
                var reviews = await _context.Reviews
                                            .Where(r => r.UtilizadorFK == utilizador.IdUser)
                                            .ToListAsync();
                _context.Reviews.RemoveRange(reviews);

                // Salvar as alterações realizadas (Apagar comentários e reviews do utilizador)
                await _context.SaveChangesAsync();

                // Apagar o utilizador da tabela "Utilizador"
                _context.Utilizador.Remove(utilizador);
                await _context.SaveChangesAsync();

                // Apagar o utilizador da tabela "AspNetUsers"
                var utilizadorIdentity = await _userManager.FindByIdAsync(utilizador.UserID);

                // Caso não seja encontrado nenhum utilizador com o dado "UserID" retorna-se a página de NotFound
                if (utilizadorIdentity == null)
                {
                    return NotFound();
                }

                // Tentativa de apagar o utilizador da tabela "AspNetUsers"
                var apagaIdent = await _userManager.DeleteAsync(utilizadorIdentity);

                // Caso não tenha sido possível apagar o utilizador da tabela "AspNetUsers" anuncia-se falhanço
                if (!apagaIdent.Succeeded)
                {
                    return BadRequest("Falhanço ao tentar apagar o utilizador da tabela AspNetUsers.");
                }

                // Caso tenha havido sucesso, volta-se ao Index
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Anunciar que existiu falhanço
                return BadRequest($"Falhanço ao apagar o user: {ex.Message}");
            }
        }

        private bool UtilizadorExists(int id)
        {
            return _context.Utilizador.Any(e => e.IdUser == id);
        }
    }
}
