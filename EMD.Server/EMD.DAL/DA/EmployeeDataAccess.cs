using EMD.EF;
using EMD.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace EMD.DAL.DA
{
    public class EmployeeDataAccess
    {
        private readonly EMDDbContext _context;

        public EmployeeDataAccess(EMDDbContext context)
        {
            _context = context;
        }
        public async Task<int> AddEmployeeAsync(Employee employee)
        {
            await _context.Employees.AddAsync(employee);
            await _context.SaveChangesAsync();
            return employee.EmployeeId;
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null)
                return false;

            _context.Employees.Remove(emp);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetEmployeesCountAsync()
        {
            return await _context.Employees.CountAsync();
        }

        public async Task<List<Employee>> FilterEmployeesAsync(
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
            var query = _context.Employees
                .AsNoTracking()
                .AsQueryable();

            // Filtering
            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(e => e.EmployeeName.Contains(name));

            if (!string.IsNullOrWhiteSpace(email))
                query = query.Where(e => e.Email.Contains(email));

            if (!string.IsNullOrWhiteSpace(phone))
                query = query.Where(e => e.PhoneNo.Contains(phone));

            if (!string.IsNullOrWhiteSpace(city))
                query = query.Where(e => e.City.Contains(city));

            if (!string.IsNullOrWhiteSpace(state))
                query = query.Where(e => e.State.Contains(state));

            if (designationId.HasValue && designationId > 0)
                query = query.Where(e => e.DesignationId == designationId.Value);

            // Sorting
            query = sortBy?.ToLower() switch
            {
                "name" => isDescending ? query.OrderByDescending(e => e.EmployeeName) : query.OrderBy(e => e.EmployeeName),
                "email" => isDescending ? query.OrderByDescending(e => e.Email) : query.OrderBy(e => e.Email),
                "city" => isDescending ? query.OrderByDescending(e => e.City) : query.OrderBy(e => e.City),
                "state" => isDescending ? query.OrderByDescending(e => e.State) : query.OrderBy(e => e.State),
                "designation" => isDescending ? query.OrderByDescending(e => e.DesignationId) : query.OrderBy(e => e.DesignationId),
                _ => query.OrderBy(e => e.EmployeeId)
            };

            // Pagination
            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees
                .AsNoTracking()
                .Include(e => e.Designation)
                .ToListAsync();
        }

        public async Task<Employee?> GetEmployeeByEmailAsync(string email)
        {
            return await _context.Employees
                .AsNoTracking()
                .Include(e => e.Designation)
                .ThenInclude(d => d.Department)
                .SingleOrDefaultAsync(e => e.Email == email);
        }
        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees
                .AsNoTracking()
                .Include(e => e.Designation)
                .ThenInclude(d => d.Department)
                .SingleOrDefaultAsync(e => e.EmployeeId == id);
        }
        public async Task<bool> UpdateEmployeeAsync(Employee employee)
        {
            var existing = await _context.Employees.FindAsync(employee.EmployeeId);
            if (existing == null)
                return false;

            existing.EmployeeName = employee.EmployeeName;
            existing.PhoneNo = employee.PhoneNo;
            existing.Email = employee.Email;
            existing.City = employee.City;
            existing.State = employee.State;
            existing.Pincode = employee.Pincode;
            existing.Address = employee.Address;
            existing.AltPhoneNo = employee.AltPhoneNo;
            existing.DesignationId = employee.DesignationId;

            _context.Employees.Update(existing);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            return await _context.Employees
                .AnyAsync(e => e.Email == email && (excludeId == null || e.EmployeeId != excludeId));
        }

        public async Task<bool> PhoneExistsAsync(string phone, int? excludeId = null)
        {
            return await _context.Employees
                .AnyAsync(e => e.PhoneNo == phone && (excludeId == null || e.EmployeeId != excludeId));
        }

    }
}
