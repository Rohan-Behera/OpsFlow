namespace OpsFlow.Server.Models.DTOModels.AuthDTO
{
    public class LoginRequest
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
