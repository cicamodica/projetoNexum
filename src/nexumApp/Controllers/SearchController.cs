using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nexumApp.Data;
using nexumApp.Models;

namespace nexumApp.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SearchController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string q)
        {
            q = q?.Trim();
            ViewBag.Query = q;

            // Se não tem termo, devolve listas vazias
            if (string.IsNullOrWhiteSpace(q))
            {
                ViewBag.Ongs = new List<Ong>();
                ViewBag.Vagas = new List<Vaga>();
                ViewBag.Metas = new List<Meta>();
                return View();
            }

            // ONGs aprovadas
            var ongs = await _db.Ongs
                .AsNoTracking()
                .Where(o => o.Aprovaçao &&
                            (o.Nome.Contains(q) ||
                             o.Descriçao.Contains(q) ||
                             o.Endereço.Contains(q) ||
                             o.CNPJ.Contains(q)))
                .OrderBy(o => o.Nome)
                .ToListAsync();

            // Vagas
            var vagas = await _db.Vagas
                .AsNoTracking()
                .Where(v => v.Titulo.Contains(q) ||
                            v.Descricao.Contains(q))
                .OrderBy(v => v.Titulo)
                .ToListAsync();

            // Metas
            var metas = await _db.Metas
                .AsNoTracking()
                .Where(m => m.Descricao.Contains(q) ||
                            m.Recurso.Contains(q))
                .OrderBy(m => m.Descricao)
                .ToListAsync();

            ViewBag.Ongs = ongs;
            ViewBag.Vagas = vagas;
            ViewBag.Metas = metas;

            return View();
        }
    }
}