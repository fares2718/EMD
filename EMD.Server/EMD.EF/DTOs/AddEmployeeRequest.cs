using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EMD.EF.DTOs
{
    public class AddEmployeeRequest
    {

        [Required, MaxLength(50)]
        [RegularExpression(@"^[\p{L}\s'-]+$",
            ErrorMessage = "Name can only contain letters, spaces, apostrophes, and hyphens.")]
        public string EmployeeName { get; set; } = string.Empty;

        [Required, MaxLength(10), MinLength(10)]
        [RegularExpression(@"^[0-9]{10}$",
            ErrorMessage = "Phone number must be exactly 10 digits.")]
        public string PhoneNo { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Password { get; set; } = string.Empty;
        [Required, MaxLength(50)]
        public string Role { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        [RegularExpression(@"^[\p{L}\s'-]+$",
            ErrorMessage = "City name must contain only letters.")]
        public string City { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        [RegularExpression(@"^[\p{L}\s'-]+$",
            ErrorMessage = "State name must contain only letters.")]
        public string State { get; set; } = string.Empty;

        [Required, MaxLength(6), MinLength(6)]
        [RegularExpression(@"^[0-9]{6}$",
            ErrorMessage = "Pincode must be exactly 6 digits.")]
        public string Pincode { get; set; } = string.Empty;

        [Required, MaxLength(2000)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(10)]
        [RegularExpression(@"^[0-9]{10}$",
            ErrorMessage = "Alternate phone must be 10 digits.")]
        public string? AltPhoneNo { get; set; } = null;

        [Required]
        public int DesignationId { get; set; }
    }
}
