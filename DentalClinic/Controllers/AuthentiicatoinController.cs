using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DentalClinic.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpGet("verify-token")]
        [Authorize]
        public IActionResult VerifyToken()
        {
           
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Invalid token.");
            }

           
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("dji215ajiowdjqa7sdfadfqeffsdgsd427ak1579255")),
                ValidateIssuer = true,
                ValidIssuer = "http://localhost:5275/",
                ValidateAudience = true,
                ValidAudience = "http://localhost:4200/",
                ValidateLifetime = true
            };

            try
            {
                
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                return Ok(new { Message = "Token is valid.", User = principal.Identity.Name });
            }
            catch (SecurityTokenExpiredException)
            {
                return Unauthorized("Token has expired.");
            }
            catch (SecurityTokenException)
            {
                return Unauthorized("Invalid token.");
            }
        }
    }
}
