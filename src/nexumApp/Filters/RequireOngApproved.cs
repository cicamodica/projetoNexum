using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using nexumApp.Data;
using System.Security.Claims;

public class RequireOngApproved : IAsyncActionFilter
{
    private readonly ApplicationDbContext _db;
    public RequireOngApproved(ApplicationDbContext db) => _db = db;

    private static readonly (string controller, string action)[] PublicRoutes = new[]
    {
        ("Home", "Index"),
        ("Home", "Privacy"),
        ("Ongs", "Index"),
        ("Ongs", "Details"),
        ("Ongs", "Wait"),
        ("Account", "Logout"),
    };

    public async Task OnActionExecutionAsync(ActionExecutingContext ctx, ActionExecutionDelegate next)
    {
        var http = ctx.HttpContext;
        var user = http.User;

        // Se não autenticado, deixa os [Authorize] cuidarem; Login/Register continuam acessíveis
        if (!(user?.Identity?.IsAuthenticated ?? false))
        {
            await next();
            return;
        }

        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            ctx.Result = new RedirectToActionResult("Wait", "Ongs", null);
            return;
        }

        // ONG aprovada?
        var approved = await _db.Ongs.AnyAsync(o => o.UserId == userId && o.Aprovaçao);
        if (approved)
        {
            await next();
            return;
        }

        // NÃO aprovado: só pode rotas públicas
        var controller = (ctx.RouteData.Values["controller"]?.ToString() ?? "");
        var action = (ctx.RouteData.Values["action"]?.ToString() ?? "");
        var path = http.Request.Path.Value ?? "";       // cobre Razor Pages (Identity)

        // 1) Allowed por controller/action (MVC)
        bool isPublicMvc = PublicRoutes.Any(r =>
            r.controller.Equals(controller, StringComparison.OrdinalIgnoreCase) &&
            r.action.Equals(action, StringComparison.OrdinalIgnoreCase));

        // 2) Allowed por path (Razor Pages Identity e MVC genérico)
        bool isPublicPath =
            path.Equals("/", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/Home/Index", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/Home/Privacy", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/Ongs/Index", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/Ongs/Details", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/Ongs/Wait", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/Account/Logout", StringComparison.OrdinalIgnoreCase);

        // 3) Identity Login/Register — bloquear se JÁ autenticado e não aprovado
        bool isIdentityLoginOrRegister =
            path.StartsWith("/Identity/Account/Login", StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith("/Identity/Account/Register", StringComparison.OrdinalIgnoreCase) ||
            (controller.Equals("Account", StringComparison.OrdinalIgnoreCase) &&
             (action.Equals("Login", StringComparison.OrdinalIgnoreCase) ||
              action.Equals("Register", StringComparison.OrdinalIgnoreCase)));

        // 4) Perfil do Identity (Manage) — bloquear até aprovar
        bool isIdentityManage = path.StartsWith("/Identity/Account/Manage", StringComparison.OrdinalIgnoreCase);

        if (isIdentityLoginOrRegister || isIdentityManage || (!isPublicMvc && !isPublicPath))
        {
            ctx.Result = new RedirectToActionResult("Wait", "Ongs", null);
            return;
        }

        await next();
    }
}


