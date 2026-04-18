using EMD.DAL.DA;
using EMD.EF.Models;

namespace EMD.BLL
{
    public class DesignationBusiness
    {
        private readonly DesignationDataAccess _designationDataAccess;

        public DesignationBusiness(DesignationDataAccess designationDataAccess)
        {
            _designationDataAccess = designationDataAccess;
        }

        public async Task<(bool Success, string Message, int NewId)> AddDesignationAsync(Designation designation)
        {
            if (designation == null)
                return (false, "Designation object is null.", 0);

            if (string.IsNullOrWhiteSpace(designation.DesignationName))
                return (false, "Designation name is required.", 0);

            var all = await _designationDataAccess.GetAllDesignationsAsync();
            if (all.Any(d => d.DesignationName == designation.DesignationName && d.DepartmentId == designation.DepartmentId))
                return (false, "Designation name already exists.", 0);

            int id = await _designationDataAccess.AddDesignationAsync(designation);
            return (true, "Designation added successfully.", id);
        }
        public async Task<(bool Success, string Message)> DeleteDesignationAsync(int id)
        {
            if (id <= 0)
                return (false, "Invalid designation ID.");

            bool deleted = await _designationDataAccess.DeleteDesignationAsync(id);
            return deleted
                ? (true, "Designation deleted successfully.")
                : (false, "Designation not found.");
        }
        public async Task<List<Designation>> GetAllDesignationsAsync()
        {
            var list = await _designationDataAccess.GetAllDesignationsAsync();
            return list.OrderBy(d => d.DesignationName).ToList();
        }
        public async Task<(bool Success, string Message, Designation? Data)> GetDesignationByIdAsync(int id)
        {
            if (id <= 0)
                return (false, "Invalid ID.", null);

            var designation = await _designationDataAccess.GetDesignationByIdAsync(id);
            return designation != null
                ? (true, "Designation found.", designation)
                : (false, "Designation not found.", null);
        }

        public async Task<(bool Success, string Message)> UpdateDesignationAsync(Designation designation)
        {
            if (designation == null)
                return (false, "Designation object is null.");

            if (designation.DesignationId <= 0)
                return (false, "Invalid designation ID.");

            if (string.IsNullOrWhiteSpace(designation.DesignationName))
                return (false, "Designation name is required.");

            var updated = await _designationDataAccess.UpdateDesignationAsync(designation);
            return updated
                ? (true, "Designation updated successfully.")
                : (false, "Designation not found.");
        }
    }
}
