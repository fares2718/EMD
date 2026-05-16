using EMD.EF;
using EMD.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace EMD.DAL.DA
{
    public class DepartmentDataAccess
    {
        private readonly EMDDbContext _context;

        public DepartmentDataAccess(EMDDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddDepartmentAsync(Department department)
        {
            await _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return department.DepartmentId;
        }

        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department == null)
                return false;
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Department>> GetAllDepartmentsAsync()
        {
            return await _context.Departments
            .AsNoTracking()
            .ToListAsync();
        }

        public async Task<Department?> GetDepartmentByIdAsync(int id)
        {
            return await _context.Departments
            .AsNoTracking()
            .FindAsync(id);
        }

        public async Task<bool> UpdateDepartmentAsync(Department department)
        {
            var existingDepartment = await _context.Departments.FindAsync(department.DepartmentId);
            if (existingDepartment == null)
                return false;
            existingDepartment.DepartmentName = department.DepartmentName;
            existingDepartment.IsActive = department.IsActive;
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
