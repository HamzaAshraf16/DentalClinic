using System.ComponentModel.DataAnnotations;

namespace DentalClinic.Models
{
    public class ResetPasswordModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Code { get; set; } // Add this property for the reset code
        
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; }
    }
}
