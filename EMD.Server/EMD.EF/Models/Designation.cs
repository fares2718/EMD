using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMD.EF.Models
{
    public class Designation
    {
        public int DesignationId { get; set; }

        [RegularExpression(@"^[\p{L}\s'-]+$",
            ErrorMessage = "Designation name must contain only letters, spaces, apostrophes, and hyphens.")]
        public string DesignationName { get; set; } = string.Empty;
        public int DepartmentId { get; set; }

        public Department Department { get; set; } = null!;
        public List<Employee> Employees { get; set; } = new List<Employee>();
    }
}
