using AutoMapper;
using EMD.BLL;
using EMD.BLL.DTOs;
using EMD.EF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace EMD.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class EmployeeController : ControllerBase
    {
        private readonly EmployeeBusiness _employeeBusiness;
        private readonly DepartmentBusiness _departmentBusiness;
        private readonly IAuthorizationService _authorizationService;
        private readonly IMapper _mapper;

        public EmployeeController(EmployeeBusiness employeeBusiness, IAuthorizationService authorizationService, DepartmentBusiness departmentBusiness, IMapper mapper)
        {
            _employeeBusiness = employeeBusiness;
            _authorizationService = authorizationService;
            _departmentBusiness = departmentBusiness;
            _mapper = mapper;
        }
        [Authorize(Roles = "Admin")]
        //[EnableRateLimiting("PostLimiter")]
        [HttpPost("Add")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddEmployee([FromBody] AddEmployeeRequest employee)
        {
            var emp = _mapper.Map<Employee>(employee);
            emp.PasswordHash = employee.Password;
            var result = await _employeeBusiness.AddEmployeeAsync(emp);

            if (!result.Success)
                return BadRequest(result.Message);

            return CreatedAtRoute("GetEmployeeById", new { id = result.NewId }, employee);
        }
        [Authorize(Roles = "Admin")]
        //[EnableRateLimiting("DeleteLimiter")]
        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var result = await _employeeBusiness.DeleteEmployeeAsync(id);

            return result.Success
                ? Ok(result.Message)
                : BadRequest(result.Message);
        }
        [Authorize(Roles = "Admin")]
        //[EnableRateLimiting("GetLimiter")]
        [HttpGet("Filter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> FilterEmployees(
            [FromQuery] string? name,
            [FromQuery] string? email,
            [FromQuery] string? phone,
            [FromQuery] string? city,
            [FromQuery] string? state,
            [FromQuery] int? designationId,
            [FromQuery] string? sortBy = "name",
            [FromQuery] bool isDescending = false,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var list = await _employeeBusiness.FilterEmployeesAsync(
                name, email, phone, city, state, designationId,
                sortBy, isDescending, pageNumber, pageSize);

            return list.Any()
                ? Ok(list)
                : NotFound("No employees match the filter criteria.");
        }

        [Authorize(Roles = "Admin")]
        //[EnableRateLimiting("GetLimiter")]
        [HttpGet("All")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllEmployees()
        {
            var list = await _employeeBusiness.GetAllEmployeesAsync();

            return list.Any()
                ? Ok(list)
                : NotFound("No employees found.");
        }

        [AllowAnonymous]
        [HttpGet("Count")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetEmployeesCount()
        {
            var count = await _employeeBusiness.GetEmployeesCountAsync();
            if(count == 0)
                return NotFound("No employees.");

            return Ok(count);
        }

        //[EnableRateLimiting("GetLimiter")]
        [HttpGet("{id}", Name = "GetEmployeeById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetEmployeeById(int id)
        {
            var authResult = await _authorizationService.AuthorizeAsync(
                                    User,
                                    id,
                                    "EmployeeOwnerOrAdmin");

            if (!authResult.Succeeded)
                return Forbid();
            var result = await _employeeBusiness.GetEmployeeByIdAsync(id);

            if(!result.Success)
                return NotFound(result.Message);
            var dto = new EmployeeDTO
            {
                EmployeeId = result.Data!.EmployeeId,
                EmployeeName = result.Data.EmployeeName,
                Address = result.Data.Address,
                City = result.Data.City,
                AltPhoneNo = result.Data.AltPhoneNo,
                Email = result.Data.Email,
                PhoneNo = result.Data.PhoneNo,
                Pincode = result.Data.Pincode,
                Role  = result.Data.Role,
                State   = result.Data.State,
                Department = (await _departmentBusiness.GetDepartmentByIdAsync(result.Data.Designation.DepartmentId)).Data!.DepartmentName,
                Designation = result.Data.Designation.DesignationName
            };

            return Ok(dto);
        }

        //[EnableRateLimiting("PutLimiter")]
        [HttpPut("Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateEmployee([FromBody] Employee employee)
        {
            var authResult = await _authorizationService.AuthorizeAsync(
                                    User,
                                    employee.EmployeeId,
                                    "EmployeeOwnerOrAdmin");

            if (!authResult.Succeeded)
                return Forbid();
            var result = await _employeeBusiness.UpdateEmployeeAsync(employee);

            return result.Success
                ? Ok(result.Message)
                : BadRequest(result.Message);
        }
    }
}
