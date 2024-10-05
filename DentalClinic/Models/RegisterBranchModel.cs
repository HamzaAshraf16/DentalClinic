using System.ComponentModel.DataAnnotations;

namespace DentalClinic.Models
{
    public class RegisterBranchModel
    {

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"^(?:\+20|0)?1[0125]\d{8}$", ErrorMessage = "عليك إدخال رقم هاتف مصري مكون من 11 رقم ")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(150)]
        public string Location { get; set; }

        
    }
}

