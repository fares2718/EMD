using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMD.EF.Models
{
    [Table("Designations")]
    public class Designation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DesignationId { get; set; }

        [Required, MaxLength(50)]
        [RegularExpression(@"^[\p{L}\s'-]+$",
            ErrorMessage = "Designation name must contain only letters, spaces, apostrophes, and hyphens.")]
        public string DesignationName { get; set; } = string.Empty;

        [Required]
        public int DepartmentId { get; set; }

        public Department Department { get; set; } = new Department();
    }
}
