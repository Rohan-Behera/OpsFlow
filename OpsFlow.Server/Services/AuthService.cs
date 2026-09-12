using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpsFlow.Server.Core;
using OpsFlow.Server.Models.DTOModels.AuthDTO;
using OpsFlow.Server.Models.EntityModels;
using OpsFlow.Server.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace OpsFlow.Server.Services
{
    public interface IAuthService
    {
        Task<ServiceResult<UserModel>> SignUpAsync(SignupRequest request);

        Task<ServiceResult<UserModel>> LoginAsync(LoginRequest request);
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

        public async Task<ServiceResult<UserModel>> SignUpAsync(SignupRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return ServiceResult<UserModel>.Fail("Email already exists");
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

            var response = await IssueTokens(entity);

            return ServiceResult<UserModel>.Ok(response);
        }

        public async Task<ServiceResult<UserModel>> LoginAsync(LoginRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !user.IsActive || !BCrypt.Net.BCrypt.Verify(request.Password, Encoding.UTF8.GetString(user.PasswordHash)))
            {
                return ServiceResult<UserModel>.Fail("Invalid email or password");
            }

            var response = await IssueTokens(user);

            return ServiceResult<UserModel>.Ok(response);
        }

        private async Task<UserModel> IssueTokens(User user)
        {
            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpiryDays"]));

            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.UserId,
                TokenHash = HashToken(refreshToken),
                ExpiresAt = refreshTokenExpiry,
                Revoked = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new UserModel
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private string GenerateAccessToken(User user)
        {
            var claims = new[]
           {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:AccessTokenExpiryMinutes"]));
            var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var cred = new SigningCredentials(jwtKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: accessTokenExpiry,
                signingCredentials: cred
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);

            return Convert.ToBase64String(bytes);
        }

        private string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));

            return Convert.ToBase64String(hashedBytes);
        }
        
    }
}
