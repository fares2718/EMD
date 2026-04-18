using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EMD.API.Authorization
{
    public class EmployeeDesignationOrAdminHandler : AuthorizationHandler<EmployeeDesignationOrAdminRequirement, int>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EmployeeDesignationOrAdminRequirement requirement, int DesignationId)
        {
            if(context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            var desId = context.User.FindFirstValue("DesignationId");
            if (int.TryParse(desId, out int des) && des == DesignationId)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
