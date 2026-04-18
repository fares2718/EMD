using EMD.BLL;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EMD.API.Authorization
{
    public class EmployeeDepartmentOrAdminHandler : AuthorizationHandler<EmployeeDepartmentOrAdminRequirement, int>
    {
        private readonly EmployeeBusiness _employeeBusiness;

        public EmployeeDepartmentOrAdminHandler(EmployeeBusiness employeeBusiness)
        {
            _employeeBusiness = employeeBusiness;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EmployeeDepartmentOrAdminRequirement requirement, int DepartmentId)
        {
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var deptId = context.User.FindFirstValue("DepartmentId");
            if (int.TryParse(deptId, out int empDeptId) && empDeptId == DepartmentId)
            {
                context.Succeed(requirement);  
            }
            return Task.CompletedTask;
        }
    }
}
