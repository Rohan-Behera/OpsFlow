using OpsFlow.Server.Models.DTOModels.AuthDTO;
using System.ComponentModel.DataAnnotations;

namespace OpsFlow.Server.Models.EntityModels
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string? Email { get; set; }
        public byte[] PasswordHash { get; set; } = null!;
        public byte[]? Salt { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; } = null!;

        // Navigation
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public Employee? Employee { get; set; }
        public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
