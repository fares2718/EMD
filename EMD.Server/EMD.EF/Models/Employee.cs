using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMD.EF.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [RegularExpression(@"^[\p{L}\s'-]+$",
            ErrorMessage = "Name can only contain letters, spaces, apostrophes, and hyphens.")]
        public string EmployeeName { get; set; } = string.Empty;

        [RegularExpression(@"^[0-9]{10}$",
            ErrorMessage = "Phone number must be exactly 10 digits.")]
        public string PhoneNo { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        [RegularExpression(@"^[\p{L}\s'-]+$",
            ErrorMessage = "City name must contain only letters.")]
        public string City { get; set; } = string.Empty;

        [RegularExpression(@"^[\p{L}\s'-]+$",
            ErrorMessage = "State name must contain only letters.")]
        public string State { get; set; } = string.Empty;

        [RegularExpression(@"^[0-9]{6}$",
            ErrorMessage = "Pincode must be exactly 6 digits.")]
        public string Pincode { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        [RegularExpression(@"^[0-9]{10}$",
            ErrorMessage = "Alternate phone must be 10 digits.")]
        public string? AltPhoneNo { get; set; } = null;

        public int DesignationId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
        public Designation Designation { get; set; } = null!;
        public List<Session> Sessions { get; set; } = new List<Session>();
    }
}
