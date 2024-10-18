using System.ComponentModel.DataAnnotations.Schema;

namespace DentalClinic.DTO
{
    public class AppointmentDto
    {
        public int AppointmentId {  get; set; }
        public int Cost { get; set; }
        public string Time { get; set; }
        public DateTime Date { get; set; }
        [Column(TypeName = "nvarchar(MAX)")]
        public string Reports { get; set; }
        public int Type { get; set; }
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
        public int PatientId {  get; set; }
        public string PatientPhoneNumber { get; set; }
        public int? PatientAge { get; set; }
        public byte? PatientGender { get; set; }
        public int DoctorId { get; set; }
        public int? Status { get; set; }

    }
}
