using DentalClinic.Migrations;
using DentalClinic.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DentalClinic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticatoinController : ControllerBase
    {
        private readonly ClinicContext context;
        private readonly UserManager<ApplicationUser> userManager;

        public AuthenticatoinController(ClinicContext context,UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (await userManager.FindByEmailAsync(model.Email) is not null)
                return BadRequest(new { Error = "Email is already registered!" });

            if (await userManager.FindByNameAsync(model.Name) is not null)
                return BadRequest(new { Error = "Username is already registered!" });

            ApplicationUser user = new ApplicationUser { UserName = model.Email, Email = model.Email,PhoneNumber = model.PhoneNumber };
            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                Patient patient = new Patient
                {
                    UserId = user.Id,
                    Name = model.Name,
                    Gender = model.Gender,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address
                };
                await context.Patients.AddAsync(patient);
                await context.SaveChangesAsync();

                await userManager.AddToRoleAsync(user, "User");

                JwtSecurityToken token = await GenerateJwtToken(user);

                AuthenticatoinModel authModel = new AuthenticatoinModel()
                {
                    Message = "User and patient record created successfully",
                    IsAuthenticated = true,
                    Username = user.UserName,
                    Email = user.Email,
                    Roles = new List<string> { "User" },
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    ExpiresOn = token.ValidTo
                };

                return Ok(authModel);
            }

            return BadRequest(result.Errors);
        }

        private async Task<JwtSecurityToken> GenerateJwtToken(ApplicationUser user)
        {
            List<Claim> UserClaims = new List<Claim>();

            UserClaims.Add(new Claim(JwtRegisteredClaimNames.Name, user.UserName));
            UserClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            UserClaims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                UserClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("dji215ajiowdjqa7sdfadfqeffsdgsd427ak1579255"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: "http://localhost:37439/",
                audience: "http://localhost:4200/",
                claims: UserClaims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return token;
        }
    }
}

