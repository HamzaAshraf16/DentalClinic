using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
namespace DentalClinic.DTO
{
    public class AddPatientDto
    {
      
        public string? UserName { get; set; }

        public string? Password { get; set; }  

        public string Name { get; set; }

        public byte Gender { get; set; }

        public string PhoneNumber { get; set; }
        public int Age { get; set; }


        public string Address { get; set; }
        public int PatientHistoryId { get; set; }
    }
}
