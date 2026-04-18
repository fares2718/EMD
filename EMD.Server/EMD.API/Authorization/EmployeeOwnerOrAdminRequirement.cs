using Microsoft.AspNetCore.Authorization;

namespace EMD.API.Authorization
{
    public class EmployeeOwnerOrAdminRequirement : IAuthorizationRequirement
    {
    }
}
