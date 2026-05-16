using EMD.DAL.DA;
using EMD.EF.Models;

namespace EMD.BLL
{
    public class DepartmentBusiness
    {
        private readonly DepartmentDataAccess _departmentDataAccess;

        public DepartmentBusiness(DepartmentDataAccess departmentDataAccess)
        {
            _departmentDataAccess = departmentDataAccess;
        }

        public async Task<(bool Success, string Message, int NewId)> AddDepartmentAsync(Department department)
        {
            if (department == null)
                return (false, "Department object is null.", 0);

            if (string.IsNullOrWhiteSpace(department.DepartmentName))
                return (false, "Department name is required.", 0);

            var allDepartments = await _departmentDataAccess.GetAllDepartmentsAsync();
            if (allDepartments.Any(d => d.DepartmentName == department.DepartmentName))
                return (false, "Department name already exists.", 0);

            int id = await _departmentDataAccess.AddDepartmentAsync(department);
            return (true, "Department added successfully.", id);
        }

        public async Task<(bool Success, string Message)> DeleteDepartmentAsync(int id)
        {
            if (id <= 0)
                return (false, "Invalid department ID.");

            bool deleted = await _departmentDataAccess.DeleteDepartmentAsync(id);
            return deleted
                ? (true, "Department deleted successfully.")
                : (false, "Department not found.");
        }

        public async Task<int> GetActiveDepartmentsCountAsync()
        {
            return await _departmentDataAccess.GetActiveDepartmentsCountAsync();
        }
        public async Task<List<Department>> GetAllDepartmentsAsync()
        {
            var list = await _departmentDataAccess.GetAllDepartmentsAsync();
            return list.OrderBy(d => d.DepartmentName).ToList();
        }

        public async Task<(bool Success, string Message, Department? Data)> GetDepartmentByIdAsync(int id)
        {
            if (id <= 0)
                return (false, "Invalid ID.", null);

            var department = await _departmentDataAccess.GetDepartmentByIdAsync(id);
            return department != null
                ? (true, "Department found.", department)
                : (false, "Department not found.", null);
        }

        public async Task<(bool Success, string Message)> UpdateDepartmentAsync(Department department)
        {
            if (department == null)
                return (false, "Department object is null.");

            if (department.DepartmentId <= 0)
                return (false, "Invalid department ID.");

            if (string.IsNullOrWhiteSpace(department.DepartmentName))
                return (false, "Department name is required.");

            var updated = await _departmentDataAccess.UpdateDepartmentAsync(department);
            return updated
                ? (true, "Department updated successfully.")
                : (false, "Department not found.");
        }
    }
}
