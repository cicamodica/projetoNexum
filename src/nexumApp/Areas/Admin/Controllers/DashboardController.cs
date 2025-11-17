using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nexumApp.Data;
using nexumApp.Models;

namespace nexumApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "RequireAdmin")]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<User> _userManager;

    public DashboardController(ApplicationDbContext db, UserManager<User> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var ongsPendentes = await _db.Ongs.Where(o => !o.Aprovaçao).ToListAsync();
        ViewBag.UnreadFale = await _db.FaleConoscoModels.CountAsync(m => m.Visualizada == false);
        return View(ongsPendentes);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var ong = await _db.Ongs.SingleOrDefaultAsync(x => x.Id == id);
        if (ong == null) return NotFound();

        ong.Aprovaçao = true;
        await _db.SaveChangesAsync();

        TempData["Success"] = "ONG aprovada com sucesso!";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id, string[] motivos)
    {
        var ong = await _db.Ongs.SingleOrDefaultAsync(x => x.Id == id);
        if (ong == null)
            return NotFound();

        //Busca o usuário dono da ONG:
        var user = await _userManager.FindByIdAsync(ong.UserId);

        // Monta um texto com os motivos da reprovação marcados:
        var motivosTexto = (motivos != null && motivos.Length > 0)
            ? string.Join("; ", motivos)
            : "Motivo não informado";

        //Salva o texto em TempData:
        TempData["Info"] = $"ONG '{ong.Nome}' reprovada. Motivo(s): {motivosTexto}";

        // Remove a ONG do banco:
        _db.Ongs.Remove(ong);
        await _db.SaveChangesAsync();

        //Remove o usuário da Identity:
        if (user != null)
            await _userManager.DeleteAsync(user);

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
