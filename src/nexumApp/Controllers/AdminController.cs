/*using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nexumApp.Data;
using nexumApp.Models;

namespace nexumApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: AdminController
        //[Route("/admin/painel")]
        public async Task<IActionResult> Index()
        {
            var ongs = await _context.Ongs.Where(ong => ong.Aprovaçao == false).ToListAsync();
            return View(ongs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Approve(int id)
        {
            var ong = await _context.Ongs.SingleOrDefaultAsync(dbOng => dbOng.Id == id);
            try {
                ong.Aprovaçao = true;
                _context.Update(ong);
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateConcurrencyException){
                throw;
            }
            return RedirectToAction(nameof(Index));
            
            //return View(nameof(Index));
        }
    }
}
*/