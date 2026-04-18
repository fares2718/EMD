using System;
using System.Collections.Generic;
using System.Text;

namespace EMD.BLL.DTOs.Auth
{
    public class LogoutRequest
    {
        public int SessionId { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }
}
