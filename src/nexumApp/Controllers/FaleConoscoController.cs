using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nexumApp.Data;
using nexumApp.Models;

namespace nexumApp.Controllers;

public class ContactController(ApplicationDbContext db, ILogger<ContactController> logger) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View(new FaleConoscoModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(FaleConoscoModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        db.FaleConoscoModels.Add(model);
        await db.SaveChangesAsync();

        TempData["ContactOk"] = "Mensagem enviada com sucesso! Em breve entraremos em contato.";
        return RedirectToAction(nameof(Sucesso));
    }

    public IActionResult Sucesso()
    {
        return View();
    }
}
