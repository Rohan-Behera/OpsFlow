using Azure.Core;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using OpsFlow.Server.Core;
using OpsFlow.Server.Models.DTOModels.AuthDTO;
using OpsFlow.Server.Models.EntityModels;
using System.Text;

namespace OpsFlow.Server.Services
{
    public interface IAuthService
    {
        Task GenerateTokens(UserModel user);
        Task GenerateAccessToken(string userName, DateTime accessTokenExpiry);
        Task GenerateRefreshToken();
        Task<User> SignUpAsync(SignupRequest request);
    }
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly OpsFlowContext _context;
        public AuthService(IConfiguration configuration, OpsFlowContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task GenerateTokens(UserModel user)
        {
            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:AccessTokenExpiryMinutes"]));

            var refreshTokenExpiry = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpiryDays"]));

            var accessToken = GenerateAccessToken(user.UserName, accessTokenExpiry);
        }

        public async Task GenerateAccessToken(string userName, DateTime accessTokenExpiry)
        {

        }

        public async Task GenerateRefreshToken()
        {

        }

        public async Task<User> SignUpAsync(SignupRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.UserName == request.UserName || u.Email == request.Email))
            {
                return null;
            }

            var hashPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var entity = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                PasswordHash = Encoding.UTF8.GetBytes(hashPassword),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
    }
}
