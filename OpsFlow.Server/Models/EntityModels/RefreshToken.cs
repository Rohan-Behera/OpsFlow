using System.ComponentModel.DataAnnotations.Schema;

namespace OpsFlow.Server.Models.EntityModels
{
    public class RefreshToken
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }   // navigation property

        public string TokenHash { get; set; }

        public DateTime ExpiresAt { get; set; }
        public bool Revoked { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Convenience property — not mapped to a column
        [NotMapped]
        public bool IsActive => !Revoked && ExpiresAt > DateTime.UtcNow;
    }
}
