using DentalClinic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Concurrent;

namespace DentalClinic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticatoinController : ControllerBase
    {
        private readonly ClinicContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration _configuration;

        // In-memory storage for reset codes (you can replace this with a more permanent solution)
        private static readonly ConcurrentDictionary<string, ResetCodeEntry> ResetCodes = new ConcurrentDictionary<string, ResetCodeEntry>();


        public AuthenticatoinController(ClinicContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            this.context = context;
            this.userManager = userManager;
            this._configuration = configuration;
        }

        // Other methods...
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (await userManager.FindByEmailAsync(model.Email) != null)
                return BadRequest(new { Error = "Email is already registered!" });

            if (await userManager.FindByNameAsync(model.Name) != null)
                return BadRequest(new { Error = "Username is already registered!" });

            var user = new ApplicationUser { UserName = model.Email, Email = model.Email, PhoneNumber = model.PhoneNumber };
            var result = await userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var patient = new Patient
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
                AuthenticatoinModel authModel = new AuthenticatoinModel
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
            if (user == null || !await userManager.CheckPasswordAsync(user, login.Password))
            {
                return BadRequest(new { Error = "Invalid email or password." });
            }

            JwtSecurityToken token = await GenerateJwtToken(user);
            var rolelist = await userManager.GetRolesAsync(user);
            AuthenticatoinModel authModel = new AuthenticatoinModel
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
            AuthenticatoinModel authModel = new AuthenticatoinModel
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
<<<<<<< HEAD
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new { Error = "User not found." });
            }

            // Generate a random 6-character code
            var resetCode = GenerateRandomCode(8);
            // Store the reset code temporarily with the current time
            ResetCodes[model.Email] = new ResetCodeEntry { Code = resetCode, CreatedAt = DateTime.UtcNow };

            // Store the reset code temporarily
           // ResetCodes[model.Email] = resetCode;

            // Send the code to the user's email
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Dental Clinic", "hamzaashraf2001141@gmail.com"));
            emailMessage.To.Add(new MailboxAddress(model.Email, model.Email));
            emailMessage.Subject = "Reset your password";
            emailMessage.Body = new TextPart("plain")
            {
                Text = $"Your password reset code is: {resetCode}"
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync("hamzaashraf2001141@gmail.com", "lomy mdew atek arpo");
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }

            return Ok(new { Message = "Password reset code has been sent to your email." });
        }

        //[HttpPost("ResetPassword")]
        //public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var user = await userManager.FindByEmailAsync(model.Email);
        //    if (user == null)
        //    {
        //        return BadRequest(new { Error = "User not found." });
        //    }

        //    Validate the reset code
        //    if (!ResetCodes.TryGetValue(model.Email, out var storedCode) || model.Code != storedCode)
        //    {
        //        return BadRequest(new { Error = "Invalid reset code." });
        //    }

        //    Generate a password reset token
        //   var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);

        //    Proceed to reset the password using the token
        //    var resetPassResult = await userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);
        //    if (!resetPassResult.Succeeded)
        //    {
        //        return BadRequest(resetPassResult.Errors);
        //    }

        //    Remove the reset code after successful reset
        //    ResetCodes.TryRemove(model.Email, out _);

        //    return Ok(new { Message = "Password has been reset successfully." });
        //}
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(new { Error = "User not found." });
            }

            // Validate the reset code and check its expiration time
            if (!ResetCodes.TryGetValue(model.Email, out var resetCodeEntry) || model.Code != resetCodeEntry.Code)
            {
                return BadRequest(new { Error = "Invalid reset code." });
            }

            // Check if the code is still valid (for example, valid for 1 hour)
            if ((DateTime.UtcNow - resetCodeEntry.CreatedAt).TotalHours > 1)
            {
                return BadRequest(new { Error = "Reset code has expired." });
            }

            // Generate a password reset token
            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);

            // Proceed to reset the password using the token
            var resetPassResult = await userManager.ResetPasswordAsync(user, resetToken, model.NewPassword);
            if (!resetPassResult.Succeeded)
            {
                return BadRequest(resetPassResult.Errors);
            }

            // Remove the reset code after successful reset
            ResetCodes.TryRemove(model.Email, out _);

            return Ok(new { Message = "Password has been reset successfully." });
        }





        private string GenerateRandomCode(int length)
        {
            const string chars = "0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // Other methods...

=======

        [HttpGet("GetAllUsers")]
        [Authorize(Roles = "Admin")]  
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await userManager.Users.Select(user => new
            {
                user.Id,
                user.UserName,
                user.Email,
                Roles = userManager.GetRolesAsync(user).Result
            }).ToListAsync();

            return Ok(users);
        }
>>>>>>> 07606cd8d038919b85e8b6925f1b9c59ba56000d
        private async Task<JwtSecurityToken> GenerateJwtToken(ApplicationUser user)
        {
            List<Claim> userClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                userClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _configuration["JWT:issuer"],
                audience: _configuration["JWT:audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(15),
                signingCredentials: creds
            );

            return token;
        }
    }
}
