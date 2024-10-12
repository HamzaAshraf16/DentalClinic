using System.ComponentModel.DataAnnotations;

namespace DentalClinic.Models
{
    public class ChangeBranchPass
    {
        public string Email { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
