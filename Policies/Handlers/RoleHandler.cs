using System.Security.Claims;
using System.Threading.Tasks;
using Identity.Policies.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace Identity.Policies.Handlers
{
    public class RoleHandler : AuthorizationHandler<RoleRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
        {
            var identity = context.User.Identity as ClaimsIdentity;
            string role = identity.FindFirst(ClaimTypes.Role)?.Value;
            if (!string.IsNullOrEmpty(role) && role == "Admin")
                context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}