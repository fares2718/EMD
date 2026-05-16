using System;
using System.Collections.Generic;
using System.Text;

namespace EMD.EF.DTOs.Auth
{
    public class RefreshRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
        public int SessionId { get; set; } 
    }
}
