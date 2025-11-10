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
        var ongsPendentes = await _db.Ongs.Where(o => !o.Aprovaçao).ToListAsync();
        ViewBag.UnreadFale = await _db.FaleConoscoModels.CountAsync(m => m.Visualizada == false);
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

    [HttpGet]
    public async Task<IActionResult> UnreadMessagesCount()
    {

        var count = await _db.FaleConoscoModels
    .CountAsync(f => f.Visualizada == false);
        return Json(new { count });
    }
}
