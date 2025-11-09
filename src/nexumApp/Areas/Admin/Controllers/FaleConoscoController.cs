using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nexumApp.Data;
using nexumApp.Models;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace nexumApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class FaleConoscoController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IEmailSender _email;

    public FaleConoscoController(ApplicationDbContext db, IEmailSender email)
    {
        _db = db;
        _email = email;
    }

    // GET
    [HttpGet]
    public async Task<IActionResult> Index(string status = "Todos")
    {
        IQueryable<FaleConoscoModel> q = _db.FaleConoscoModels
            .AsNoTracking()
            .OrderByDescending(x => x.CriadoEm);

        if (!string.Equals(status, "Todos", StringComparison.OrdinalIgnoreCase))
            q = q.Where(x => x.Status == status);

        ViewBag.Status = status;
        return View(await q.ToListAsync());
    }

    // GET (Partial)
    [HttpGet]
    public async Task<IActionResult> Reply(int id)
    {
        var item = await _db.FaleConoscoModels.FindAsync(id);
        if (item == null) return NotFound();
        return PartialView("_ReplyFaleConosco", item);
    }

    // POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reply(int id, string responseText)
    {
        if (string.IsNullOrWhiteSpace(responseText))
            return BadRequest("Resposta vazia.");

        var item = await _db.FaleConoscoModels.FindAsync(id);
        if (item == null) return NotFound();

        await _email.SendEmailAsync(item.Email, $"Re: {item.Assunto} - Nexum", responseText);

        item.Status = "Respondido";
        await _db.SaveChangesAsync();

        return Ok(new { ok = true });
    }

    // POST
    [HttpPost]
    public async Task<IActionResult> Archive(int id)
    {
        var item = await _db.FaleConoscoModels.FindAsync(id);
        if (item == null) return NotFound();

        item.Status = "Arquivado";
        await _db.SaveChangesAsync();

        return Ok(new { ok = true });
    }
}
