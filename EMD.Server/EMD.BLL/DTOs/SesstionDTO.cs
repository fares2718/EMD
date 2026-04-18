using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EMD.BLL.DTOs
{
    public class SesstionDTO
    {
        public int SessionId { get; set; }
        public int UserId { get; set; }
        public string RefreshTokenHash { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiresAt { get; set; }
        public DateTime? RefreshTokenRevokedAt { get; set; }
    }
}
