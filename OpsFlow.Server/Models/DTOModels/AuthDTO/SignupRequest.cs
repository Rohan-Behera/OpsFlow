using System.ComponentModel.DataAnnotations;

namespace OpsFlow.Server.Models.DTOModels.AuthDTO
{
    public class SignupRequest
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }
}
