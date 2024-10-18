using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinic.DTO
{
    public class BranchInfoandDoctor
    {
        public int DoctorWorkBranchId { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(50)]
        public string? DoctorName { get; set; }
        public int doctorId { get; set; }

        [Column(TypeName = "nvarchar")]
        [StringLength(30)]
        public string? BranchName { get; set; }
        public int branshId { get; set; }

        [RegularExpression(@"^(Monday|Tuesday|Wednesday|Thursday|Friday|Saturday|Sunday)$",
        ErrorMessage = "يجب ان يكون يوم عمل ")]
        public string? Day { get; set; }
        [DefaultValue(true)]
        public bool IsWork { get; set; }

        [Column(TypeName = "time")]
        public TimeSpan StartTime { get; set; }

        [Column(TypeName = "time")]
        public TimeSpan EndTime {  get; set; }


        



    }
}
