using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMD.EF.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }

        [Required, MaxLength(50)]
        [RegularExpression(@"^[\p{L}\s'-]+$",
            ErrorMessage = "Department name must contain only letters, spaces, apostrophes, and hyphens.")]
        public string DepartmentName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public List<Designation> Designations { get; set; } = new List<Designation>();
    }
}
