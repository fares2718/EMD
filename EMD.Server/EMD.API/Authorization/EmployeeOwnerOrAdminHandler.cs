using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EMD.API.Authorization
{
    public class EmployeeOwnerOrAdminHandler : AuthorizationHandler<EmployeeOwnerOrAdminRequirement, int>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EmployeeOwnerOrAdminRequirement requirement, int employeeId)
        {
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Ownership check
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (int.TryParse(userId, out int authenticatedEmployee) &&
                authenticatedEmployee == employeeId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;

        }
    }
}
