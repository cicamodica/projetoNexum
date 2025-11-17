using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nexumApp.Data;
using nexumApp.Models;

namespace nexumApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "RequireAdmin")]
public class ConviteAdminController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public ConviteAdminController(
        ApplicationDbContext db,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // Apenas o ADMIN pode criar convite:
    [Authorize(Roles = "Admin")]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateInvite(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            TempData["Error"] = "Informe um e-mail válido.";
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        var existing = await _userManager.FindByEmailAsync(email);
        if (existing is not null)
        {
            TempData["Error"] = "Já existe um usuário com esse e-mail.";
            return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
        }

        var invite = new ConviteAdministrador { Email = email.Trim() };
        _db.ConviteAdministradors.Add(invite);
        await _db.SaveChangesAsync();

        var link = Url.Action(
        action: nameof(Accept),           
        controller: "ConviteAdmin",        
        values: new { area = "Admin", token = invite.Token },
        protocol: Request.Scheme)!;

        TempData["Success"] = $"Convite criado! Envie este link para a pessoa: {link}";
        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
    }

    // Qualquer pessoa com o link pode abrir o formulário:
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Accept(Guid token)
    {
        var invite = await _db.ConviteAdministradors
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Token == token);

        if (invite is null || invite.Used || invite.ExpiresAt < DateTime.UtcNow)
            return NotFound("Convite inválido ou expirado.");

        return View(new ConviteAdminViewModel
        {
            Email = invite.Email,
            Token = token
        });
    }

    [AllowAnonymous]
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Accept(ConviteAdminViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var invite = await _db.ConviteAdministradors.FirstOrDefaultAsync(i => i.Token == model.Token);
        if (invite is null || invite.Used || invite.ExpiresAt < DateTime.UtcNow || !string.Equals(invite.Email, model.Email, StringComparison.OrdinalIgnoreCase))
            return NotFound("Convite inválido ou expirado.");

        var user = new User { UserName = model.Email, Email = model.Email, EmailConfirmed = true };

        var create = await _userManager.CreateAsync(user, model.Password);
        if (!create.Succeeded)
        {
            foreach (var err in create.Errors) ModelState.AddModelError("", err.Description);
            return View(model);
        }

        // RoleExist garante que a role Admin existe:
        if (!await _roleManager.RoleExistsAsync("Admin"))
            await _roleManager.CreateAsync(new IdentityRole("Admin"));

        await _userManager.AddToRoleAsync(user, "Admin");

        invite.Used = true;
        await _db.SaveChangesAsync();

        TempData["Success"] = "Conta de administrador criada com sucesso! Bem-vindo!";
        return RedirectToAction("Login", "Account", new { area = "Identity" });
    }
}