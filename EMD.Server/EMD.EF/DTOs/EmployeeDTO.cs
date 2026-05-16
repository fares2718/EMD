using System;
using System.Collections.Generic;
using System.Text;

namespace EMD.EF.DTOs
{
    public class EmployeeDTO
    {
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; } = string.Empty;

        public string PhoneNo { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string Pincode { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string? AltPhoneNo { get; set; }

        public string Designation { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
