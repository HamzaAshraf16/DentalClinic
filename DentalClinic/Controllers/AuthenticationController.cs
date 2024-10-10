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
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using DentalClinic.Models;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace DentalClinic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ClinicContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration _configuration;

        // In-memory storage for reset codes (you can replace this with a more permanent solution)
        private static readonly ConcurrentDictionary<string, ResetCodeEntry> ResetCodes = new ConcurrentDictionary<string, ResetCodeEntry>();


        public AuthenticationController(ClinicContext context, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            this.context = context;
            this.userManager = userManager;
            this._configuration = configuration;
        }
        [HttpPost("register-doctor")]
        //[Authorize(Roles = "Admin")] // Only accessible by Admin
        public async Task<IActionResult> RegisterDoctor(RegisterDoctorModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (model.Password != model.ConfirmPassword)
            {
                return BadRequest(new { Error = "Passwords do not match!" });
            }

            if (await userManager.FindByEmailAsync(model.Email) != null)
                return BadRequest(new { Error = "Email is already registered!" });

            if (await userManager.FindByNameAsync(model.Name) != null)
                return BadRequest(new { Error = "Username is already registered!" });

            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email, PhoneNumber = model.PhoneNumber };
                    var result = await userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        var doctor = new Doctor
                        {
                            UserId = user.Id,
                            Name = model.Name,
                            PhoneNumber = model.PhoneNumber,
                            Email=model.Email
                        };
                        await context.Doctors.AddAsync(doctor);
                        await context.SaveChangesAsync();

                        await userManager.AddToRoleAsync(user, "Admin");
                        JwtSecurityToken token = await GenerateJwtTokenDashBoard(user, "Admin");

                        var authModel = new AuthenticatoinModel
                        {

                            Message = "Doctor registered successfully",
                            IsAuthenticated = true,
                            Username = user.UserName,
                            Email = user.Email,
                            Roles = new List<string> { "Admin" },
                            Token = new JwtSecurityTokenHandler().WriteToken(token),
                            ExpiresOn = token.ValidTo
                        };

                        await transaction.CommitAsync();
                        return Ok(authModel);
                    }
                    return BadRequest(result.Errors);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, new { Error = "An error occurred during registration.", Details = ex.Message });
                }
            }
        }
        [HttpPost("register-branch")]
        //[Authorize(Roles = "Secretary")] // Only accessible by Secretary
        public async Task<IActionResult> RegisterBranch(RegisterBranchModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (model.Password != model.ConfirmPassword)
            {
                return BadRequest(new { Error = "Passwords do not match!" });
            }

            if (await userManager.FindByEmailAsync(model.Email) != null)
                return BadRequest(new { Error = "Email is already registered!" });
            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email, PhoneNumber = model.PhoneNumber };
                    var result = await userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        var branch = new Branch
                        {
                            UserId = user.Id,
                            Name = model.Name,
                            Location = model.Location
                        };

                        await context.Branchs.AddAsync(branch);
                        await context.SaveChangesAsync();

                        await userManager.AddToRoleAsync(user, "Secretary");
                        JwtSecurityToken token = await GenerateJwtTokenDashBoard(user, "Admin");
                        var authModel = new AuthenticatoinModel
                        {
                            Message = "Secretary registered successfully",
                            IsAuthenticated = true,
                            Username = user.UserName,
                            Email = user.Email,
                            Roles = new List<string> { "Secretary" },
                            Token = new JwtSecurityTokenHandler().WriteToken(token),
                            ExpiresOn = token.ValidTo
                        };
                        await transaction.CommitAsync();
                        return Ok(authModel);
                    }
                    return BadRequest(result.Errors);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, new { Error = "An error occurred during registration.", Details = ex.Message });
                }
            }
        }


        // Login for both Doctor and Secretary with dashboard access
        [HttpPost("login-dashboard")]
        public async Task<IActionResult> LoginDashboard(LoginModel login)
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

            var roles = await userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                JwtSecurityToken doctorToken = await GenerateJwtTokenDashBoard(user, "Admin");
                return Ok(new AuthenticatoinModel
                {
                    Message = "Login successful",
                    IsAuthenticated = true,
                    Username = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToList(),
                    Token = new JwtSecurityTokenHandler().WriteToken(doctorToken),
                    ExpiresOn = doctorToken.ValidTo
                });
            }

            if (roles.Contains("Secretary"))
            {
                JwtSecurityToken secretaryToken = await GenerateJwtTokenDashBoard(user, "Secretary");
                return Ok(new AuthenticatoinModel
                {
                    Message = "Login successful",
                    IsAuthenticated = true,
                    Username = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToList(),
                    Token = new JwtSecurityTokenHandler().WriteToken(secretaryToken),
                    ExpiresOn = secretaryToken.ValidTo
                });
            }

            return Unauthorized(new { Error = "Unauthorized user role." });
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

            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                try
                {
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email, PhoneNumber = model.PhoneNumber };
                    var result = await userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        var patientHistory = new PatientHistory
                        {
                            Hypertension = model.Hypertension,
                            Diabetes = model.Diabetes,
                            StomachAche = model.StomachAche,
                            PeriodontalDisease = model.PeriodontalDisease,
                            IsPregnant = model.IsPregnant,
                            IsBreastfeeding = model.IsBreastfeeding,
                            IsSmoking = model.IsSmoking,
                            KidneyDiseases = model.KidneyDiseases,
                            HeartDiseases = model.HeartDiseases
                        };
                        await context.PatientsHistory.AddAsync(patientHistory);
                        await context.SaveChangesAsync();

                        var patient = new Patient
                        {
                            UserId = user.Id,
                            Name = model.Name,
                            Gender = model.Gender,
                            PhoneNumber = model.PhoneNumber,
                            Address = model.Address,
                            Age=model.Age,
                            PatientHistoryId = patientHistory.PatientHistoryID
                            
                        };
                        await context.Patients.AddAsync(patient);
                        await context.SaveChangesAsync();


                        await userManager.AddToRoleAsync(user, "User");
                        JwtSecurityToken token = await GenerateJwtToken(user);
                        AuthenticatoinModel authModel = new AuthenticatoinModel
                        {
                            Message = "User, patient record, and patient history created successfully",
                            IsAuthenticated = true,
                            Username = user.UserName,
                            Email = user.Email,
                            Roles = new List<string> { "User" },
                            Token = new JwtSecurityTokenHandler().WriteToken(token),
                            ExpiresOn = token.ValidTo
                        };

                        await transaction.CommitAsync();
                        return Ok(authModel);
                    }
                    return BadRequest(result.Errors);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, new { Error = "An error occurred during registration.", Details = ex.Message });
                }
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the user exists in the system
            var userManagerUser = await userManager.FindByEmailAsync(login.Email);
            if (userManagerUser == null || !await userManager.CheckPasswordAsync(userManagerUser, login.Password))
            {
                return BadRequest(new { Error = "Invalid email or password." });
            }

            // Ensure the user has the User role
            if (!await userManager.IsInRoleAsync(userManagerUser, "User"))
            {
                await userManager.AddToRoleAsync(userManagerUser, "User");
            }

            // Generate JWT token for regular User
            JwtSecurityToken defaultToken = await GenerateJwtToken(userManagerUser);

            // Return successful login with User role
            return Ok(new AuthenticatoinModel
            {
                Message = "Login successful",
                IsAuthenticated = true,
                Username = userManagerUser.UserName,
                Email = userManagerUser.Email,
                Roles = new List<string> { "User" },
                Token = new JwtSecurityTokenHandler().WriteToken(defaultToken),
                ExpiresOn = defaultToken.ValidTo
            });
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

            // Update user properties
            user.Email = model.Email;
            user.UserName = model.Email; // If you want to allow username change
            user.PhoneNumber = model.PhoneNumber;

            // Update in the UserManager
            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Optionally, update other related data (e.g., Patient record if applicable)
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
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"])),
                ValidateIssuer = true,
                ValidIssuer = _configuration["JWT:issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["JWT:audience"],
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
         [HttpPost("change-Branch-password")]
 public async Task<IActionResult> ChangePassword([FromBody] ChangeBranchPass request)
 {
    
     if (request.NewPassword != request.ConfirmPassword)
     {
         return BadRequest("كلمات السر لا تتطابق.");
     }
     var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); 
     var userRole = await userManager.GetRolesAsync(await userManager.FindByIdAsync(userId));

     
     if (!userRole.Contains("Admin"))
     {
         return Forbid("ليس لديك الصلاحيات لتغيير كلمة السر."); 
     }


    
     var user = await userManager.FindByEmailAsync(request.Email);
     if (user == null)
     {
         return NotFound("المستخدم غير موجود.");
     }

     
     var result = await userManager.RemovePasswordAsync(user);
     if (result.Succeeded)
     {
         result = await userManager.AddPasswordAsync(user, request.NewPassword);
         if (result.Succeeded)
         {
             return Ok(new { message = "تم تغيير كلمة السر بنجاح." });
         }
     }

     return BadRequest("حدث خطأ أثناء تغيير كلمة السر.");
 }


        private async Task<JwtSecurityToken> GenerateJwtTokenDashBoard(ApplicationUser user, string role)
        {
            List<Claim> userClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, role)
            };

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



        private string GenerateRandomCode(int length)
        {
            const string chars = "0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
