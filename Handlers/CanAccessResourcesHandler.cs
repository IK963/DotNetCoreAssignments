using DotNetCoreAssignments.Enums;
using DotNetCoreAssignments.Models.Policies;
using Microsoft.AspNetCore.Authorization;

namespace DotNetCoreAssignments.Handlers
{
    public class CanAccessResourcesHandler : AuthorizationHandler<SameUserOrAdminRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserOrAdminRequirement requirement)
        {
            var roles = Enum.GetNames(typeof(UserRoles));

            if (context.User.Identity != null && context.User.IsInRole(UserRoles.Admin.ToString()))
                context.Succeed(requirement);
            else if (context.User.Identity != null && roles.Any(role => context.User.IsInRole(role)))
            {
                // Access the HttpContext to retrieve the route data
                var httpContext = (context.Resource as DefaultHttpContext)?.HttpContext;
                if (httpContext != null)
                {
                    // Access route data to get the ID from the URL
                    var routeData = httpContext.Request.RouteValues;
                    if (routeData.TryGetValue("id", out var idValue))
                    {
                        if (Guid.TryParse(idValue?.ToString(), out var id))
                        {
                            var dbContext = httpContext.RequestServices.GetService<ApplicationDbContext>(); 
                            var item =  dbContext.ToDo.Find(id);
                            if (item != null && item.UserName != null && item.UserName.Equals(context.User.Identity.Name))
                            {
                                context.Succeed(requirement);
                            }
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
