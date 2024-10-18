using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace DentalClinic.Models
{
    public class Patient
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatientId { get; set; } // Primary Key
         
        [Required]
        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        public string Name { get; set; } 
        [Required]
        [Range(0, 1)]  // Specify the allowed range for gender (0 or 1)
        public byte Gender { get; set; } // Gender (0: Male, 1: Female)

        private string _phoneNumber;
        [Required(ErrorMessage = "من فضلك ادخل رقم الهاتف")]
        [Column(TypeName = "nchar(11)")]
        [RegularExpression(@"^(?:\+20|0)?1[0125]\d{8}$", ErrorMessage = "عليك إدخال رقم هاتف مصري مكون من 11 رقم ")]
        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                // Validation logic
                if (!Regex.IsMatch(value, @"^(?:\+20|0)?1[0125]\d{8}$"))
                {
                    throw new ArgumentException("عليك إدخال رقم هاتف مصري مكون من 11 رقم");
                }
                _phoneNumber = value;
            }
        }
        [Required]
        [StringLength(150)]
        public string Address { get; set; }

        [Required]
        [Range(0, 100)] 
        public int? Age { get; set; }
        
        [ForeignKey("PatientHistory")]
        public int? PatientHistoryId { get; set; }
        public virtual PatientHistory PatientHistory { get; set; }

        [Column(TypeName = "nvarchar(450)")]
        [ForeignKey("User")]
        public string? UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
