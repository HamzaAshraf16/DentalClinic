using DentalClinic.Migrations;
using DentalClinic.Models;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IConfiguration _configuration;

        public AuthenticatoinController(ClinicContext context,UserManager<ApplicationUser> userManager,IConfiguration configuration)
        {
            this.context = context;
            this.userManager = userManager;
            this._configuration = configuration;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }
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


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);

            }
            var user = await userManager.FindByEmailAsync(login.Email);
            if (user is null || !await userManager.CheckPasswordAsync(user, login.Password))
            {
                return BadRequest(new { Error = "Invalid email or password." });
            }
            JwtSecurityToken token = await GenerateJwtToken(user);
            var rolelist = await userManager.GetRolesAsync(user);
            AuthenticatoinModel authModel = new AuthenticatoinModel()
            {
                Message = "Login successful",
                IsAuthenticated = true,
                Username = user.UserName,
                Email = user.Email,
                Roles = rolelist.ToList(),
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresOn = token.ValidTo
            };
            return Ok(authModel);
        }


        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new { Error = "User not found." });
            }

            if (!await userManager.CheckPasswordAsync(user, model.CurrentPassword))
            {
                return BadRequest(new { Error = "Current password is incorrect." });
            }

            var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            JwtSecurityToken token = await GenerateJwtToken(user);
            var rolelist = await userManager.GetRolesAsync(user);

            AuthenticatoinModel authModel = new AuthenticatoinModel()
            {
                Message = "Password changed successfully",
                IsAuthenticated = true,
                Username = user.UserName,
                Email = user.Email,
                Roles = rolelist.ToList(),
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresOn = token.ValidTo
            };

            return Ok(authModel);
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser(UpdateUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new { Error = "User not found." });
            }

            user.Email = model.Email;
            user.UserName = model.Email; 
            user.PhoneNumber = model.PhoneNumber;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var patient = await context.Patients.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (patient != null)
            {
                patient.Name = model.Name;
                patient.Gender = model.Gender;
                patient.Address = model.Address;
                patient.PhoneNumber = model.PhoneNumber;

                context.Patients.Update(patient);
                await context.SaveChangesAsync();
            }

            return Ok(new { Message = "User data updated successfully." });
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

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuration["JWT:issuer"],
                audience: _configuration["JWT:audience"],
                claims: UserClaims,
                expires: DateTime.Now.AddDays(15),
                signingCredentials: creds
            );

            return token;
        }


    }
}

