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
        ViewBag.Status = status ?? "Todos";

        var q = _db.FaleConoscoModels.AsNoTracking();

        switch ((status ?? "Todos").Trim().ToLowerInvariant())
        {
            case "novo":
                q = q.Where(x => !x.Visualizada && !x.Arquivada);
                break;

            case "respondido":
                q = q.Where(x => x.Respondida && !x.Arquivada);
                break;

            case "arquivado":
                q = q.Where(x => x.Arquivada);
                break;
        }

        var lista = await q.OrderByDescending(x => x.CriadoEm).ToListAsync();
        return View(lista);
    }

    // GET (Partial)
    [HttpGet]
    public async Task<IActionResult> Reply(int id)
    {
        var item = await _db.FaleConoscoModels.FindAsync(id);
        if (item == null) return NotFound();
        if (!item.Visualizada)
        {
            item.Visualizada = true;
            await _db.SaveChangesAsync();
        }

        return PartialView("_ReplyFaleConosco", item);
    }

    // GET
    [HttpGet]
    public async Task<IActionResult> UnreadCount()
    {
        var count = await _db.FaleConoscoModels
            .CountAsync(m => !m.Visualizada && !m.Arquivada);
        return Json(new { count });
    }

    // GET (Partial) – só para visualizar os detalhes da mensagem:
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var item = await _db.FaleConoscoModels.FindAsync(id);
        if (item == null) return NotFound();

        if (!item.Visualizada)
        {
            item.Visualizada = true;
            if (string.IsNullOrEmpty(item.Status) || item.Status == "Novo")
                item.Status = "Lido";
            await _db.SaveChangesAsync();
        }

        return PartialView("_FaleConoscoDetails", item);
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

        item.Respondida = true;
        item.Visualizada = true;
        item.Arquivada = false;
        item.Status = "Respondido";

        await _db.SaveChangesAsync();

        return Ok(new { ok = true });
    }

    // POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Archive(int id)
    {
        var item = await _db.FaleConoscoModels.FindAsync(id);
        if (item == null) return NotFound();

        item.Arquivada = true;
        item.Status = "Arquivado";
        await _db.SaveChangesAsync();

        return Ok(new { ok = true });
    }

    //POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var item = await _db.FaleConoscoModels.FindAsync(id);
        if (item == null) return NotFound();

        if (!item.Visualizada)
        {
            item.Visualizada = true;
            if (string.IsNullOrEmpty(item.Status) || item.Status == "Novo")
                item.Status = "Lido";
            await _db.SaveChangesAsync();
        }
        return Ok(new { ok = true });
    }

    // POST
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unarchive(int id)
    {
        var item = await _db.FaleConoscoModels.FindAsync(id);
        if (item == null) return NotFound();

        item.Arquivada = false;
        item.Status = item.Respondida ? "Respondido" : "Novo";
        await _db.SaveChangesAsync();

        return Ok(new { ok = true });
    }

}
