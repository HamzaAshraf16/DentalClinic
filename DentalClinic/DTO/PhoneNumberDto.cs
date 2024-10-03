using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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