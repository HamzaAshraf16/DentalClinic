using System.ComponentModel.DataAnnotations;

namespace DentalClinic.Dtos
{
    public class AddPhoneNumberDto
    {
        [Required]
        [RegularExpression(@"^(?:\+20|0)?1[0125]\d{8}$", ErrorMessage = "عليك إدخال رقم هاتف مصري مكون من 11 رقم")]
        public string Phonenumber { get; set; }

        [Required]
        public int BranchID { get; set; }
    }
}

