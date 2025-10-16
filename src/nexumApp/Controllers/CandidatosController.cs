using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nexumApp.Data;

namespace nexumApp.Controllers
{
    public class CandidatosController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CandidatosController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var candidatos = await _context.Candidatos.ToListAsync();
            return View(candidatos);
        }
    }
}
