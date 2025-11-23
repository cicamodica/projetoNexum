using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nexumApp.Data;
using nexumApp.Models;
using nexumApp.Services;

namespace nexumApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class FaleConoscoController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IEmailSender _email;
    private readonly IEmailService _emailService;
    private readonly ILogger<DashboardController> _logger;

    public FaleConoscoController(ApplicationDbContext db, IEmailSender email, IEmailService emailService, ILogger<DashboardController> logger)
    {
        _db = db;
        _emailService = emailService;
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
        // Passa o próprio model para o partial
        return PartialView("_ReplyFaleConosco", item);
    }

    // POST: envia o e-mail de resposta (sem ViewModel)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reply(int id, string resposta)
    {
        var item = await _db.FaleConoscoModels.FindAsync(id);
        if (item == null) return NotFound();

        if (string.IsNullOrWhiteSpace(resposta))
        {
            ModelState.AddModelError("Resposta", "Você precisa informar uma resposta.");
            // Recarrega o partial com o próprio item
            return PartialView("_ReplyFaleConosco", item);
        }

        string corpoEmail = $@"
        <div style='font-family: Arial, sans-serif; color: #333;'>
            <h2 style='color: #16435D;'>Olá, {item.Nome}!</h2>

            <p>Você nos enviou a seguinte mensagem pela página <strong>Fale Conosco</strong>:</p>
            <blockquote style='border-left: 4px solid #16435D; padding-left: 8px; color: #555;'>
                {item.Mensagem}
            </blockquote>

            <p><strong>Resposta da nossa equipe:</strong></p>
            <p>{resposta}</p>

            <br />
            <p>Atenciosamente,<br /><strong>Equipe Nexum</strong></p>
        </div>";

        try
        {
            await _emailService.SendEmailAsync(
                item.Email,                      // e-mail do usuário do Fale Conosco
                "Resposta - Nexum",
                corpoEmail
            );

            item.Respondida = true;
            item.DataResposta = DateTime.UtcNow;
            item.Resposta = resposta;

            await _db.SaveChangesAsync();

            TempData["Success"] = "Resposta enviada com sucesso!";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar resposta de Fale Conosco {Id}", item.Id);
            TempData["Error"] = "Ocorreu um erro ao enviar o e-mail de resposta.";
        }

        return RedirectToAction(nameof(Index));
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
