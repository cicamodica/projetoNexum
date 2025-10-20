using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace nexumApp.Models
{
    public class OngIsOwnerRequirementHandler : AuthorizationHandler<OngIsOwnerRequirement>
    {
        private readonly IHttpContextAccessor _contextAccessor;
         public OngIsOwnerRequirementHandler(IHttpContextAccessor contextAccessor) 
        { 
            _contextAccessor = contextAccessor; 
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OngIsOwnerRequirement requirement)
        {
            var userIdFromUrl = _contextAccessor.HttpContext.Request.RouteValues["UserId"];
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //var resourceId = context.Resource

            if (userId == userIdFromUrl.ToString())
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
