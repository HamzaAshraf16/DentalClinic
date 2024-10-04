using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinic.Models
{
    public class Branch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BranchId { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [Column(TypeName = "nvarchar")]
        [StringLength(100)]
        public string Location { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [Column(TypeName = "nvarchar")]
        [StringLength(30)]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        public virtual ICollection<Doctor_Work_Branch> DoctorWorkBranches { get; set; }
    }
}
