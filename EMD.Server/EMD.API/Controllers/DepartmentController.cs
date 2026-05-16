using EMD.API.Authorization;
using EMD.BLL;
using EMD.EF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EMD.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class DepartmentController : ControllerBase
    {
        private readonly DepartmentBusiness _departmentBusiness;
        private readonly IAuthorizationService _authorizationService;

        public DepartmentController(DepartmentBusiness departmentBusiness, IAuthorizationService authorizationService)
        {
            _departmentBusiness = departmentBusiness;
            _authorizationService = authorizationService;
        }

        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("PostLimiter")]
        [HttpPost("Add")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddDepartment([FromBody] Department department)
        {
            var result = await _departmentBusiness.AddDepartmentAsync(department);

            if (!result.Success)
                return BadRequest(result.Message);

            return CreatedAtRoute("GetDepartmentById", new { id = result.NewId }, department);
        }

        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("DeleteLimiter")]
        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var result = await _departmentBusiness.DeleteDepartmentAsync(id);

            return result.Success
                ? Ok(result.Message)
                : BadRequest(result.Message);
        }

        [AllowAnonymous]
        [EnableRateLimiting("GetLimiter")]
        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllDepartments()
        {
            var departments = await _departmentBusiness.GetAllDepartmentsAsync();

            return departments.Any()
                ? Ok(departments)
                : NotFound("No departments found.");
        }

        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("GetLimiter")]
        [HttpGet("{id}", Name = "GetDepartmentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, id, new EmployeeDepartmentOrAdminRequirement());

            if (!authResult.Succeeded)
                return Forbid();

            var result = await _departmentBusiness.GetDepartmentByIdAsync(id);

            return result.Success
                ? Ok(result.Data)
                : NotFound(result.Message);
        }

        [EnableRateLimiting("GetLimiter")]
        [HttpGet("ActiveCount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActiveDepartmentsCount()
        {
            var count = await _departmentBusiness.GetActiveDepartmentsCountAsync();
            return Ok(count);
        }

        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("PutLimiter")]
        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateDepartment([FromBody] Department department)
        {
            var result = await _departmentBusiness.UpdateDepartmentAsync(department);

            return result.Success
                ? Ok(result.Message)
                : BadRequest(result.Message);
        }
    }
}
