using EMD.API.Authorization;
using EMD.BLL;
using EMD.EF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EMD.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class DesignationController : ControllerBase
    {
        private readonly DesignationBusiness _designationBusiness;
        private readonly IAuthorizationService _authorizationService;

        public DesignationController(DesignationBusiness designationBusiness, IAuthorizationService authorizationService)
        {
            _designationBusiness = designationBusiness;
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
        public async Task<IActionResult> AddDesignation([FromBody] Designation designation)
        {
            var result = await _designationBusiness.AddDesignationAsync(designation);

            if (!result.Success)
                return BadRequest(result.Message);

            return CreatedAtRoute("GetDesignationById", new { id = result.NewId }, designation);
        }

        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("DeleteLimiter")]
        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteDesignation(int id)
        {
            var result = await _designationBusiness.DeleteDesignationAsync(id);

            return result.Success
                ? Ok(result.Message)
                : BadRequest(result.Message);
        }

        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("GetLimiter")]
        [HttpGet("Filter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FilterDesignations(
            [FromQuery] int? departmentId,
            [FromQuery] string? name)
        {
            var list = await _designationBusiness.GetAllDesignationsAsync();

            if (departmentId.HasValue && departmentId > 0)
                list = list.Where(d => d.DepartmentId == departmentId.Value).ToList();

            if (!string.IsNullOrWhiteSpace(name))
                list = list.Where(d => d.DesignationName.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();

            return list.Any()
                ? Ok(list)
                : NotFound("No designations match the filter criteria.");
        }

        [Authorize(Roles = "Admin")]
        [EnableRateLimiting("GetLimiter")]
        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllDesignations()
        {
            var list = await _designationBusiness.GetAllDesignationsAsync();

            return list.Any()
                ? Ok(list)
                : NotFound("No designations found.");
        }

        [EnableRateLimiting("GetLimiter")]
        [HttpGet("{id}", Name = "GetDesignationById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDesignationById(int id)
        {
            var authResult = await _authorizationService.AuthorizeAsync(User, id, new EmployeeDesignationOrAdminRequirement());
            if(!authResult.Succeeded)
                return Forbid();
            var result = await _designationBusiness.GetDesignationByIdAsync(id);

            return result.Success
                ? Ok(result.Data)
                : NotFound(result.Message);
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
        public async Task<IActionResult> UpdateDesignation([FromBody] Designation designation)
        {
            var result = await _designationBusiness.UpdateDesignationAsync(designation);

            return result.Success
                ? Ok(result.Message)
                : BadRequest(result.Message);
        }
    }
}
