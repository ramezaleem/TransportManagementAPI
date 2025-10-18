using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TransportManagement.API.Services
{
    public class JwtTokenHelper
    {
        private readonly IConfiguration _configuration;
        public JwtTokenHelper ( IConfiguration configuration )
        {
            _configuration = configuration;
        }

        public string GenerateToken ( string userId, string userName, string email )
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            // اقرأ المفتاح السري من appsettings.json
            var secretKey = jwtSettings["SecretKey"];
            if (string.IsNullOrWhiteSpace(secretKey))
                throw new Exception("JWT SecretKey is not set in appsettings.json!");

            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiryMinutes = int.Parse(jwtSettings["ExpiryInMinutes"] ?? "15");

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.UniqueName, userName),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
