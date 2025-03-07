using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Remy.Gambit.Api.Constants;
using Remy.Gambit.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Remy.Gambit.Api.Helpers
{
    public static class TokenHelper
    {
        public static string GenerateToken(User user, string refreshToken, IConfiguration configuration)
        {
            var claims = new List<Claim> {
                new(ClaimTypes.Name, user.Id.ToString()),
                new(ClaimTypes.Role, user.Role),
                new(Config.SessionID, refreshToken)
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
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] password = new char[length];
            byte[] randomBytes = new byte[length];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            for (int i = 0; i < length; i++)
            {
                int index = randomBytes[i] % validChars.Length;
                password[i] = validChars[index];
            }

            return new string(password);
        }
    }
}
