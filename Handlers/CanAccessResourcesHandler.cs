using DotNetCoreAssignments.Models.Policies;
using DotNetCoreAssignments.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using System.Text.RegularExpressions;

namespace DotNetCoreAssignments.Handlers
{
    public class CanAccessResourcesHandler : AuthorizationHandler<SameUserOrAdminRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserOrAdminRequirement requirement)
        {
            if (context.User.Identity != null && context.User.IsInRole(UserRoles.Admin))
                context.Succeed(requirement);
            else if (context.User.Identity != null && (context.User.IsInRole(UserRoles.User) || context.User.IsInRole(UserRoles.Admin)))
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
