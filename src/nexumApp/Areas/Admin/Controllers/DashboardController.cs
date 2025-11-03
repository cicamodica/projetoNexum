using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nexumApp.Data;

namespace nexumApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "RequireAdmin")]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _db;
    public DashboardController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {

        var ongsPendentes = await _db.Ongs
            .Where(o => !o.Aprovaçao)
            .ToListAsync();

        return View(ongsPendentes);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var ong = await _db.Ongs.SingleOrDefaultAsync(x => x.Id == id);
        if (ong == null) return NotFound();

        ong.Aprovaçao = true;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
