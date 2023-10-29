using Microsoft.AspNetCore.Authorization;

namespace DotNetCoreAssignments.Models.Policies
{
    public class SameUserOrAdminRequirement : IAuthorizationRequirement
    {
        public SameUserOrAdminRequirement() 
        { 

        }
    }
}
