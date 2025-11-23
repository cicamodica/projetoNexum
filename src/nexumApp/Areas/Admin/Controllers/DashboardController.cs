using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nexumApp.Controllers;
using nexumApp.Data;
using nexumApp.Models;
using nexumApp.Services;

namespace nexumApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "RequireAdmin")]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IEmailService _emailService;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(ApplicationDbContext db, IEmailService emailService, ILogger<DashboardController> logger, UserManager<User> userManager)
    {
        _db = db;
        _emailService = emailService;
        _userManager = userManager;
        _logger = logger;
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

        var user = await _userManager.FindByIdAsync(ong.UserId);

        // Envio do e-mail de aprovação:
        if (user != null && !string.IsNullOrWhiteSpace(user.Email))
        { 
            try
            {
                string corpoEmail = $@"
                <div style='font-family: Arial, sans-serif; color: #333;'>
                    <h2 style='color: #16435D;'>Olá, {ong.Nome}!</h2>
                    <p>Seu cadastro na plataforma <strong>Nexum</strong> foi <strong>aprovado</strong>! 🎉</p>
                    <hr>
                    <p>Agora sua ONG já pode acessar todas as funcionalidades da plataforma:
                    cadastrar metas, criar vagas de voluntariado e receber doações.</p>
                    <br>
                    <p>Atenciosamente,<br><strong>Equipe Nexum</strong></p>
                </div>";

                await _emailService.SendEmailAsync(
                    user.Email,
                    "Cadastro aprovado - Nexum",
                    corpoEmail
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar e-mail de aprovação para ONG {OngId}", ong.Id);

            }
        }

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


        var user = await _userManager.FindByIdAsync(ong.UserId);

        var motivosTexto = (motivos != null && motivos.Length > 0)
            ? string.Join("; ", motivos)
            : "Motivo não informado";

        // Envio do e-mail de reprovação:
        if (user != null && !string.IsNullOrWhiteSpace(user.Email))
        {
            try
            {
                string motivosHtml = (motivos != null && motivos.Length > 0)
                    ? "<ul>" + string.Join("", motivos.Select(m => $"<li>{m}</li>")) + "</ul>"
                    : "<p><em>Motivo não informado.</em></p>";

                string corpoEmail = $@"
                <div style='font-family: Arial, sans-serif; color: #333;'>
                    <h2 style='color: #16435D;'>Olá, {ong.Nome}!</h2>
                    <p>Seu cadastro na plataforma <strong>Nexum</strong> foi analisado, 
                    mas infelizmente <strong>não pôde ser aprovado</strong> neste momento.</p>
                    <hr>
                    <p><strong>Motivo(s) informado(s):</strong></p>
                    {motivosHtml}
                    <p>Você pode revisar suas informações e realizar um novo cadastro 
                    corrigindo os pontos indicados.</p>
                    <br>
                    <p>Atenciosamente,<br><strong>Equipe Nexum</strong></p>
                </div>";

                await _emailService.SendEmailAsync(
                    user.Email,
                    "Cadastro reprovado - Nexum",
                    corpoEmail
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar e-mail de reprovação para ONG {OngId}", ong.Id);
            }
        }



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
