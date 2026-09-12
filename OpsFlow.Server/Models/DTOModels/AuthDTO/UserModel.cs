namespace OpsFlow.Server.Models.DTOModels.AuthDTO
{
    public class UserModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string? Email { get; set; }
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
