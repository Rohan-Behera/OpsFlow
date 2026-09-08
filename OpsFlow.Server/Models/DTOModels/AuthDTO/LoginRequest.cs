namespace OpsFlow.Server.Models.DTOModels.AuthDTO
{
    public class LoginRequest
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
