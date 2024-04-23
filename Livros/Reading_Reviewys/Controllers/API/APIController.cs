using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reading_Reviewys.Data;
using Reading_Reviewys.Models;

namespace Reading_Reviewys.Controllers.HtmlController {
    [Route("api/[controller]")]
    [ApiController]
    public class APIController : ControllerBase {

        public ApplicationDbContext _context;

        public APIController(ApplicationDbContext context)
        { 
            _context = context;
        }

        // Retorna a info de uma dada tabela
        [HttpGet]
        [Route("")]
        public ActionResult Index()
        {
            //.(nome de uma tabela existente na BD
            var lista = _context.Comentarios;
            return Ok(lista);
        }
    }
}
