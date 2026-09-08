namespace OpsFlow.Server.Models.DTOModels.AuthDTO
{
    public class LoginResponse
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiresAtUtc { get; set; }
        public IEnumerable<string> Roles { get; set; } = Array.Empty<string>();
    }
}
