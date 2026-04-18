using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EMD.EF.Models
{
    [Table("Sessions")]
    public class Session
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SessionId { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public string RefreshTokenHash { get; set; } = string.Empty;
        [Required]
        public DateTime RefreshTokenExpiresAt { get; set; }
        public DateTime? RefreshTokenRevokedAt { get; set; }
    }
}
