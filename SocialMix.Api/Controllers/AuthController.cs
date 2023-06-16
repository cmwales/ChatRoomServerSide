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
using SocialMix.BusinessLayer.Managers.Security;
using SocialMix.DataLayer;
using SocialMix.Models.Models;

namespace SocialMix.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly UserManager userManager;
        private readonly UserLoginActivityManager userLoginActivityManager;
        private readonly JwtTokenGeneratorManager jwtTokenGeneratorManager;

        public AuthController(UserManager userManager, JwtTokenGeneratorManager jwtTokenGeneratorManager, UserLoginActivityManager userLoginActivityManager)
        {
            this.userManager = userManager;
            this.jwtTokenGeneratorManager = jwtTokenGeneratorManager;
            this.userLoginActivityManager = userLoginActivityManager;
        }



        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {

            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            string username = request.Username;
            string password = request.Password;

            try
            {
                if (this.userLoginActivityManager.IsAccountLocked(username))
                {
                    throw new Exception("Account is locked out. Please wait 15 minutes and try again");
                }
                else
                {

                    // Check if the maximum number of login attempts has been reached

                    User user = this.userManager.AuthenticateUser(username, password);

                    var sessionId = this.jwtTokenGeneratorManager.GenerateSessionId();

                    var token = this.jwtTokenGeneratorManager.GenerateToken(user, DateTime.Now.AddMinutes(30));
                    this.userLoginActivityManager.RecordLoginSuccess(user.UserId, sessionId, ipAddress);

                    // Set session ID in a cookie
                    Response.Cookies.Append("sessionId", sessionId, new CookieOptions
                    {
                        HttpOnly = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTimeOffset.UtcNow.AddMinutes(30)
                    });

                    // Return token
                    return Ok(new { token });
                }

            }
            catch (Exception ex)
            {
                // Record failed login attempt
                this.userLoginActivityManager.RecordFailedLoginAttempt(username, ipAddress, ex.Message);

                // Get the number of failed login attempts for the user
                int failedAttempts = this.userLoginActivityManager.GetFailedLoginAttempts(username);

                // Check if the maximum number of login attempts has been reached
                if (failedAttempts >= 5)
                {
                    // Set the status to indicate lockout
                    this.userLoginActivityManager.LockoutUser(username, ipAddress);

                    return BadRequest(new { message = "Too many failed login attempts. Your account has been locked out. Please try again later." });
                }

                throw;
            }
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