using Microsoft.AspNetCore.Mvc;
using OpsFlow.Server.Models.DTOModels.AuthDTO;
using OpsFlow.Server.Services;

namespace OpsFlow.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService; 

        public AuthController(IConfiguration configuration, IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(SignupRequest request)
        {
            var res = await _authService.SignUpAsync(request);

            if (!res.Success)
            {
                return BadRequest(new { message = res.Message });
            }

            return Ok(new { message = "Signup successful" });

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var res = await _authService.LoginAsync(request);
            if (!res.Success)
            {
                return BadRequest(new { message = res.Message });
            }
            return Ok(res.Data);
        }
    }
}