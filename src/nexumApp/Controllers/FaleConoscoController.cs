using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nexumApp.Data;
using nexumApp.Models;

namespace nexumApp.Controllers;

public class FaleConoscoController(ApplicationDbContext db, ILogger<FaleConoscoController> logger) : Controller
{
    [HttpGet]
    public IActionResult Index() => View(new FaleConoscoModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(FaleConoscoModel model)
    {
        if (!ModelState.IsValid) return View(model);

        db.FaleConoscoModels.Add(model);
        await db.SaveChangesAsync();

        TempData["MensagemSucesso"] = "Sua mensagem foi enviada com sucesso!";
        return RedirectToAction(nameof(Sucesso));
    }

    public IActionResult Sucesso() => View();
}

