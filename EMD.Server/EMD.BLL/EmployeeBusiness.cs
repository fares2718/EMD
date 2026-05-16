using AutoMapper;
using EMD.EF.DTOs;
using EMD.DAL.DA;
using EMD.EF.Models;

namespace EMD.BLL
{
    public class EmployeeBusiness
    {
        private readonly EmployeeDataAccess _employeeDataAccess;
        private readonly DepartmentDataAccess _departmentDataAccess;
        private IMapper _mapper;

        public EmployeeBusiness(EmployeeDataAccess employeeDataAccess, IMapper mapper, DepartmentDataAccess departmentDataAccess)
        {
            _employeeDataAccess = employeeDataAccess;
            _mapper = mapper;
            _departmentDataAccess = departmentDataAccess;
        }
        public async Task<(bool Success, string Message, int NewId)> AddEmployeeAsync(Employee employee)
        {
            if (employee == null)
                return (false, "Employee object is null.", 0);

            if (string.IsNullOrWhiteSpace(employee.EmployeeName))
                return (false, "Employee name is required.", 0);

            // Unique Email
            if (await _employeeDataAccess.EmailExistsAsync(employee.Email))
                return (false, "Email already exists.", 0);

            // Unique Phone
            if (await _employeeDataAccess.PhoneExistsAsync(employee.PhoneNo))
                return (false, "Phone number already exists.", 0);

            employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(employee.PasswordHash);

            int id = await _employeeDataAccess.AddEmployeeAsync(employee);
            return (true, "Employee added successfully.", id);
        }

        public async Task<(bool Valid, string Message,Employee? Data)> AuthValidation(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return (false, "Email and password are required.", null);
            var emp = await _employeeDataAccess.GetEmployeeByEmailAsync(email);
            if (emp == null)
                return (false, "Invalid email or password.", null);
            bool isValid = BCrypt.Net.BCrypt.Verify(password, emp.PasswordHash);
            return isValid
                ? (true, "Authentication successful.", emp)
                : (false, "Invalid email or password.", null);
        } 
        public async Task<(bool Success, string Message)> DeleteEmployeeAsync(int id)
        {
            if (id <= 0)
                return (false, "Invalid employee ID.");

            bool deleted = await _employeeDataAccess.DeleteEmployeeAsync(id);
            return deleted
                ? (true, "Employee deleted successfully.")
                : (false, "Employee not found.");
        }

        public async Task<List<EmployeeDTO>> FilterEmployeesAsync(
           string? name,
           string? email,
           string? phone,
           string? city,
           string? state,
           int? designationId,
           string? sortBy,
           bool isDescending,
           int pageNumber,
           int pageSize)
        {
             var listDTOs = (await _employeeDataAccess.FilterEmployeesAsync(
                name, email, phone, city, state, designationId,
                sortBy, isDescending, pageNumber, pageSize)).Select(e => _mapper.Map<EmployeeDTO>(e));
            return listDTOs.ToList();
        }

        public async Task<int> GetEmployeesCountAsync()
        {
            return await _employeeDataAccess.GetEmployeesCountAsync();
        }

        public async Task<List<EmployeeDTO>> GetAllEmployeesAsync()
        {
            var list = await _employeeDataAccess.GetAllEmployeesAsync();
            var dtos = new List<EmployeeDTO>();

            foreach (var item in list)
            {
                var dto = new EmployeeDTO
                {
                    EmployeeId = item.EmployeeId,
                    EmployeeName = item.EmployeeName,
                    Email = item.Email,
                    Address = item.Address,
                    AltPhoneNo = item.AltPhoneNo,
                    PhoneNo = item.PhoneNo,
                    City = item.City,
                    Pincode = item.Pincode,
                    Role = item.Role,
                    State = item.State,
                    Designation = item.Designation.DesignationName,
                    Department = item.Designation.Department.DepartmentName
                };
                dtos.Add(dto);
            }
            return dtos.ToList();
        }

        public async Task<(bool Success, string Message, Employee? Data)> GetEmployeeByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return (false, "Email is required.", null);
            var emp = await _employeeDataAccess.GetEmployeeByEmailAsync(email);
            return emp != null
                ? (true, "Employee found.", emp)
                : (false, "Employee not found.", null);
        }
        public async Task<(bool Success, string Message, Employee? Data)> GetEmployeeByIdAsync(int id)
        {
            if (id <= 0)
                return (false, "Invalid ID.", null);

            var emp = await _employeeDataAccess.GetEmployeeByIdAsync(id);

            return emp != null
                ? (true, "Employee found.", emp)
                : (false, "Employee not found.", null);
        }
        public async Task<(bool Success, string Message)> UpdateEmployeeAsync(Employee employee)
        {
            if (employee == null)
                return (false, "Employee object is null.");

            if (employee.EmployeeId <= 0)
                return (false, "Invalid employee ID.");

            // Unique Email (exclude current employee)
            if (await _employeeDataAccess.EmailExistsAsync(employee.Email, employee.EmployeeId))
                return (false, "Email already exists for another employee.");

            // Unique Phone (exclude current employee)
            if (await _employeeDataAccess.PhoneExistsAsync(employee.PhoneNo, employee.EmployeeId))
                return (false, "Phone number already exists for another employee.");

            employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(employee.PasswordHash);

            var updated = await _employeeDataAccess.UpdateEmployeeAsync(employee);
            return updated
                ? (true, "Employee updated successfully.")
                : (false, "Employee not found.");
        }

    }
}
