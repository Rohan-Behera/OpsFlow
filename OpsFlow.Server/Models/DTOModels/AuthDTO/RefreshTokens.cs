using OpsFlow.Server.Models.EntityModels;

namespace OpsFlow.Server.Models.DTOModels.AuthDTO
{
    public class RefreshTokens
    {
        public int Id { get; set; }
        public int UserId { get; set; }        
        public string Token{ get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime RevokedAt { get; set; }


        public User User { get; set; } = null;
    }
}
