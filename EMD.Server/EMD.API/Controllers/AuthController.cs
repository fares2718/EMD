using Azure.Core;
using EMD.BLL;
using EMD.BLL.DTOs;
using EMD.EF.DTOs.Auth;
using EMD.DAL.DA;
using EMD.EF.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EMD.EF.DTOs;

namespace EMD.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly EmployeeBusiness _employeeBusiness;
        private readonly SessionBusiness _sessionBusiness;
        private readonly IConfiguration _config;

        public AuthController(EmployeeBusiness employeeBusiness, SessionBusiness sessionBusiness, IConfiguration config)
        {
            _employeeBusiness = employeeBusiness;
            _sessionBusiness = sessionBusiness;
            _config = config;
        }

        [HttpPost("login")]
        //[EnableRateLimiting("AuthLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody]LoginRequest request)
        {
            var ValidationResult = await _employeeBusiness.AuthValidation(request.Email, request.Password);

            if(!ValidationResult.Valid)
                return Unauthorized( ValidationResult.Message);
            if (ValidationResult.Data == null)
                return Unauthorized(ValidationResult.Message);

            var employee = ValidationResult.Data!;
            var claims = new[]
            {   
                new Claim(ClaimTypes.NameIdentifier, employee.EmployeeId.ToString()),
                new Claim(ClaimTypes.Email, employee.Email),
                new Claim(ClaimTypes.Role, employee.Role),
                new Claim("DesignationId", employee.DesignationId.ToString()),
                new Claim("DepartmentId", employee.Designation.DepartmentId.ToString())
            };

            var secretKey = _config["JWT_SECRET_KEY"];

            if (string.IsNullOrEmpty(secretKey))
                return StatusCode(500, "JWT Secret Key is missing in environment variables.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "EMD",
                audience: "EMDUsers",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds
            );

            string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = GenerateRefreshToken();

            var sessionId = await _sessionBusiness.AddSessionAsync(employee.EmployeeId, refreshToken, DateTime.Now.AddDays(7));

            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(2),
            });

            Response.Cookies.Append("accessToken", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(30),
            });

            Response.Cookies.Append("sessionId", sessionId.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(2),
            });

            var empDTO = new EmployeeDTO
            {
                EmployeeId = employee.EmployeeId,
                EmployeeName = employee.EmployeeName,
                Email = employee.Email,
                Address = employee.Address,
                AltPhoneNo = employee.AltPhoneNo,
                PhoneNo = employee.PhoneNo,
                City = employee.City,
                Pincode = employee.Pincode,
                Role = employee.Role,
                State = employee.State,
                Designation = employee.Designation.DesignationName,
                Department = employee.Designation.Department.DepartmentName ?? "N/A"
            };

            return Ok(empDTO);
        }

        [HttpPost("refresh")]
        //[EnableRateLimiting("AuthLimiter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var sessionId = Request.Cookies["sessionId"];

            if (string.IsNullOrEmpty(refreshToken)|| string.IsNullOrEmpty(sessionId))
                return Unauthorized("Missing refresh token");

            var session = await _sessionBusiness.GetSessionByIdAsync(int.Parse(sessionId));


            if (session == null || session.RefreshTokenRevokedAt != null || session.RefreshTokenExpiresAt < DateTime.UtcNow)
                return Unauthorized("Invalid refresh request");

            if(!BCrypt.Net.BCrypt.Verify(refreshToken, session.RefreshTokenHash))
                return Unauthorized("Invalid refresh request");


            var userInfo = (await _employeeBusiness.GetEmployeeByIdAsync(session.UserId)).Data!;

            // Issue NEW access token (same claims & signing settings as login)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userInfo.EmployeeId.ToString()),
                new Claim(ClaimTypes.Email,userInfo.Email),
                new Claim(ClaimTypes.Role, userInfo.Role),
                new Claim("DesignationId", userInfo.DesignationId.ToString()),
                new Claim("DepartmentId", userInfo.Designation.DepartmentId.ToString())
            };

            var secretKey = _config["JWT_SECRET_KEY"]; ;

            if (string.IsNullOrEmpty(secretKey))
                return StatusCode(500, "JWT Secret Key is missing in environment variables.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: "EMD",
                audience: "EMDUsers",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            var newAccessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            // Rotation: replace refresh token
            var newRefreshToken = GenerateRefreshToken();
            if (!await _sessionBusiness.UpdateSessionAsync(session.SessionId,newRefreshToken, DateTime.UtcNow.AddDays(7)))
                return StatusCode(500, "Failed to rotate refresh token");

            Response.Cookies.Append("refreshToken", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(2),
            });

            Response.Cookies.Append("accessToken", newAccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(30),
            });

            Response.Cookies.Append("sessionId", sessionId.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(2),
            });

            var empDTO = new EmployeeDTO
            {
                EmployeeId = userInfo.EmployeeId,
                EmployeeName = userInfo.EmployeeName,
                Email = userInfo.Email,
                Address = userInfo.Address,
                AltPhoneNo = userInfo.AltPhoneNo,
                PhoneNo = userInfo.PhoneNo,
                City = userInfo.City,
                Pincode = userInfo.Pincode,
                Role = userInfo.Role,
                State = userInfo.State,
                Designation = userInfo.Designation.DesignationName,
                Department = userInfo.Designation.Department.DepartmentName
            };

            return Ok(empDTO);
        }

        [HttpPost("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var sessionId = Request.Cookies["sessionId"];

            if(string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(sessionId))
                return Ok(); // Do not reveal if user exists

            var session = await _sessionBusiness.GetSessionByIdAsync(int.Parse(sessionId));

            if (session == null)
                return Ok(); // Do not reveal if user exists

            if(!await _sessionBusiness.RevokeSessionAsync(session.SessionId))
                return StatusCode(500, "Failed to revoke session");
            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
            });

            Response.Cookies.Delete("sessionId", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
            });

            return Ok("Logged out successfully");
            
        }

        private static string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
