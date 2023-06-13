using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SocialMix.Api.ViewModels;
using SocialMix.BusinessLayer.Managers;
using SocialMix.DataLayer;
using SocialMix.Models.Models;

namespace SocialMix.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly UserManager userManager;
        private readonly PersonRepository userRepository;

        public AuthController(UserManager userManager, PersonRepository userRepository = null)
        {
            this.userManager = userManager;
            this.userRepository = userRepository;
        }



        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            string username = request.Username;
            string password = request.Password;

            Person user = this.userManager.AuthenticateUser(username, password);
            
            // Authenticate user and generate token
            var token = GenerateToken(username);

            // Return token
            return Ok(new { token });
        }

        private string GenerateToken(string username)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                // Add additional claims as needed
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("TenTwentyOne1021TenTwentyOne1021"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "cmwales",
                audience: "groupies",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

      
        [HttpPost("negotiate")]
        public IActionResult Negotiate(int negotiateVersion = 1)
        {
            // Perform negotiation logic here

            // For demonstration purposes, returning a sample response
            var response = new
            {
                Version = negotiateVersion,
                ConnectionToken = "ABC123",
                ConnectionId = "123456",
                AvailableTransports = new[] { "WebSockets", "ServerSentEvents", "LongPolling" }
                // Include any other properties required by your client-side negotiation process
            };

            return Ok(response);
        }
    }
}