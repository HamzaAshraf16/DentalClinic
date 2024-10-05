using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class PhoneNumberDto
{
<<<<<<< HEAD
    public class PhoneNumberDto
    {
        [Required]
        [RegularExpression(@"^(?:\+20|0)?1[0125]\d{8}$", ErrorMessage = "عليك إدخال رقم هاتف مصري مكون من 11 رقم")]
        public string Phonenumber { get; set; }

        [Required]
        public int BranchID { get; set; }
        [Required(ErrorMessage = "Lacation is required")]
        [Column(TypeName = "nvarchar")]
        [StringLength(100)]
        public string BranchLocation { get; set; }
        [Required(ErrorMessage = "Name is required")]
        [Column(TypeName = "nvarchar")]
        [StringLength(30)]
        public string BranchName { get; set; }
    }
}
=======
    [Required]
    [RegularExpression(@"^(?:\+20|0)?1[0125]\d{8}$", ErrorMessage = "عليك إدخال رقم هاتف مصري مكون من 11 رقم")]
    public string Phonenumber { get; set; }
>>>>>>> f6a47c0f488a96134a8d779b231156da6638cc50

    [Required]
    public int BranchID { get; set; }
    [Required(ErrorMessage = "Lacation is required")]
    [Column(TypeName = "nvarchar")]
    [StringLength(100)]
    public string BranchLocation { get; set; }
    [Required(ErrorMessage = "Name is required")]
    [Column(TypeName = "nvarchar")]
    [StringLength(30)]
    public string BranchName { get; set; }
}