using System.ComponentModel.DataAnnotations;

namespace DentalClinic.Models
{
    public class UpdateUserModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }


        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"^(?:\+20|0)?1[0125]\d{8}$", ErrorMessage = "عليك إدخال رقم هاتف مصري مكون من 11 رقم ")]
        public string PhoneNumber { get; set; }

        [Required]
        [Range(0, 1)]
        public byte Gender { get; set; }

        [Required]
        [StringLength(150)]
        public string Address { get; set; }

    }
}
