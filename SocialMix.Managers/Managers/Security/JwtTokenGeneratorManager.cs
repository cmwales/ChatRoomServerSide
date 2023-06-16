using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using SocialMix.Models.Models;

namespace SocialMix.BusinessLayer.Managers.Security
{
    public class JwtTokenGeneratorManager
    {
        private string secretKey;
        private string issuer;
        private string audience;

        public JwtTokenGeneratorManager(string secretKey, string issuer, string audience)
        {
            this.secretKey = secretKey;
            this.issuer = issuer;
            this.audience = audience;
        }

        public string GenerateToken(User user, DateTime expirationDate, string[] roles = null)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            // Include user roles if applicable
            // Example:
            // new Claim(ClaimTypes.Role, "admin"),
            // new Claim(ClaimTypes.Role, "user"),
        };

            if (roles != null && roles.Length > 0)
            {
                foreach (var role in roles)
                {
                    claims.Append(new Claim(ClaimTypes.Role, role));
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expirationDate,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateSessionId()
        {
            Guid sessionIdGuid = Guid.NewGuid();
            string sessionId = sessionIdGuid.ToString();
            return sessionId;
        }
    }

}

