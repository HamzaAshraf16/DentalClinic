using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace DentalClinic.Models
{
    public class Doctor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DoctorId { get; set; } // Primary Key
        [StringLength(50)]
        public string? UserName { get; set; } 
        [StringLength(50)]
        public string? Password { get; set; } 
        [Required]
        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        public string Name { get; set; } 
 
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

        public virtual ICollection<Doctor_Work_Branch> DoctorWorkBranches { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }


    }

}

