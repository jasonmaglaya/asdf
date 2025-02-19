using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Remy.Gambit.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Remy.Gambit.Api.Helpers
{
    public static class TokenHelper
    {
        public static string GenerateToken(User user, IConfiguration configuration)
        {
            var claims = new List<Claim> {
                new(ClaimTypes.Name, user.Id.ToString()),
                new(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                configuration["Jwt:IssuerSigningKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(configuration.GetValue("Jwt:TokenExpiryHours", 12)),
                    signingCredentials: creds,
                    issuer: configuration["Jwt:Issuer"]!
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public static string GenerateToken(int length)
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(length));
        }
    }
}
