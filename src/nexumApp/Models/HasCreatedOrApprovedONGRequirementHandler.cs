using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using nexumApp.Data;
using System.Security.Claims;

namespace nexumApp.Models
{
    public class HasCreatedOrApprovedONGRequirement : IAuthorizationRequirement {}
    public class HasCreatedOrApprovedONGRequirementHandler(IServiceScopeFactory scopeFactory, IHttpContextAccessor contextAccessor) : AuthorizationHandler<HasCreatedOrApprovedONGRequirement>
    {
        protected override async Task<Task> HandleRequirementAsync(AuthorizationHandlerContext context, HasCreatedOrApprovedONGRequirement requirement)
        {
            using (var scope = scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userONGs = await dbContext.Ongs.Where(ong => ong.UserId == userId).ToListAsync();
                var hasCreatedONG = userONGs.Count() > 0;
                var hasApprovedONG = userONGs.Where(ong => ong.Aprovaçao == true).ToList().Count() > 0;


                if (hasApprovedONG || !hasCreatedONG)
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
                if((hasCreatedONG && !hasApprovedONG))
                {
                    context.Succeed(requirement);
                    contextAccessor.HttpContext.Response.Redirect("/Ongs/Wait");
                    return Task.CompletedTask;
                }
                else
                {
                    context.Succeed(requirement);
                    contextAccessor.HttpContext.Response.Redirect("/Ongs/Create");
                    return Task.CompletedTask;
                }
            }
        }
    }
}
