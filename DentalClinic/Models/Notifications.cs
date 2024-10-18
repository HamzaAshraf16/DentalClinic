using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DentalClinic.Models
{
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int NotificationId { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        [MaxLength]
        public string Message { get; set; }
        public bool IsRead { get; set; } = false; 
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [ForeignKey("Patient")]
        public int PatientId { get; set; } 
        public virtual Patient Patient { get; set; }
    }
}
