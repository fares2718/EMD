using EMD.EF;
using EMD.EF.Models;
using Microsoft.EntityFrameworkCore;

namespace EMD.DAL.DA
{
    public class DesignationDataAccess
    {
        private readonly EMDDbContext _context;

        public DesignationDataAccess(EMDDbContext context)
        {
            _context = context;
        }
        public async Task<int> AddDesignationAsync(Designation designation)
        {
            await _context.Designations.AddAsync(designation);
            await _context.SaveChangesAsync();
            return designation.DesignationId;
        }

        public async Task<bool> DeleteDesignationAsync(int id)
        {
            var designation = await _context.Designations.FindAsync(id);
            if (designation == null)
                return false;

            _context.Designations.Remove(designation);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get All
        public async Task<List<Designation>> GetAllDesignationsAsync()
        {
            return await _context.Designations
                .ToListAsync();
        }
        public async Task<Designation?> GetDesignationByIdAsync(int id)
        {
            return await _context.Designations
                .FindAsync(id);
        }

        public async Task<bool> UpdateDesignationAsync(Designation designation)
        {
            var existing = await _context.Designations.FindAsync(designation.DesignationId);
            if (existing == null)
                return false;

            existing.DesignationName = designation.DesignationName;
            existing.DepartmentId = designation.DepartmentId;

            _context.Designations.Update(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
