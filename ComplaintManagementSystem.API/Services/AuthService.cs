using ComplaintManagementSystem.Shared.DTOs;
using ComplaintManagementSystem.Shared.Entities;
using ComplaintManagementSystem.API.Repositories;
using System.Threading.Tasks;
using System;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;

namespace ComplaintManagementSystem.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
        }

        public async Task<int> RegisterAsync(RegisterRequest request)
        {
            var existing = await _authRepository.GetByUsernameOrEmailAsync(request.Email ?? string.Empty);
            if (existing != null) throw new InvalidOperationException("User already exists.");

            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password ?? string.Empty),
                RoleId = request.RoleId,
                CreatedDate = DateTime.UtcNow,
                IsActive = true
            };

            return await _authRepository.CreateAsync(user);
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _authRepository.GetByUsernameOrEmailAsync(request.UsernameOrEmail ?? string.Empty);
            if (user == null || !VerifyPassword(request.Password ?? string.Empty, user.PasswordHash ?? string.Empty))
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            var jwtSection = _configuration.GetSection("Jwt");
            var key = jwtSection["Key"];
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var expires = int.Parse(jwtSection["ExpiresMinutes"] ?? "60");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FirstName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.LastName ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("RoleId", user.RoleId.ToString()),
                new Claim(ClaimTypes.Role, user.RoleId == 1 ? "Admin" : "User")
            };

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expires),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return new LoginResponse
            {
                UserId = user.Id,
                Username = user.FirstName + " " + user.LastName,
                RoleId = user.RoleId,
                Token = jwt
            };
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private static bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
    }
}