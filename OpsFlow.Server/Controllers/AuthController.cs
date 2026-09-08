using Microsoft.AspNetCore.Mvc;
using OpsFlow.Server.Core;
using OpsFlow.Server.Models.DTOModels.AuthDTO;
using OpsFlow.Server.Models.EntityModels;
using OpsFlow.Server.Services;
using System.Text;

namespace OpsFlow.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly OpsFlowContext _context;
        private readonly IAuthService _authService; 

        public AuthController(IConfiguration configuration, OpsFlowContext context, IAuthService authService)
        {
            _configuration = configuration;
            _context = context;
            _authService = authService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(SignupRequest request)
        {
            var res = await _authService.SignUpAsync(request);
            
            return Ok(new { message = "Signup successful" });

        }
    }
}