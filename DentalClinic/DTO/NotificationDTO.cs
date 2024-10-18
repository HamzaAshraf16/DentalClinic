using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DentalClinic.DTO
{
    public class NotificationDTO
    {
        public int NotificationId { get; set; }

        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedDate { get; set; } 

        public int PatientId { get; set; }
    }
}
