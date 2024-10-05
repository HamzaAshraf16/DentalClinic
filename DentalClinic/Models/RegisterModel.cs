 using System.ComponentModel.DataAnnotations;

namespace DentalClinic.Models
{
    public class RegisterModel
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
        [Range(0, 1)]
        public byte Gender { get; set; }

        [Required]
        [Range(0, 100)]
        public int? Age { get; set; }

        [Required]
        [RegularExpression(@"^(?:\+20|0)?1[0125]\d{8}$", ErrorMessage = "عليك إدخال رقم هاتف مصري مكون من 11 رقم ")]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(150)]
        public string Address { get; set; }

        public bool Hypertension { get; set; }
        public bool Diabetes { get; set; }
        public bool StomachAche { get; set; }
        public bool PeriodontalDisease { get; set; }
        public bool IsPregnant { get; set; }
        public bool IsBreastfeeding { get; set; }
        public bool IsSmoking { get; set; }
        public bool KidneyDiseases { get; set; }
        public bool HeartDiseases { get; set; }
    }
}
