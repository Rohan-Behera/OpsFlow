namespace OpsFlow.Server.Models.DTOModels.AuthDTO
{
    public class UserModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string? Email { get; set; }
        public byte[] PasswordHash { get; set; } = null!;
        public byte[]? Salt { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
}
