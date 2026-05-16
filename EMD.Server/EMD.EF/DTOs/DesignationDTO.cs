using System;
using System.Collections.Generic;
using System.Text;

namespace EMD.EF.DTOs
{
    public class DesignationDTO
    {
        public int DesignationId { get; set; }

        public string DesignationName { get; set; } = string.Empty;

        public string DepartmentName { get; set; } = string.Empty;
    }
}