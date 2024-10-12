using Microsoft.AspNetCore.Identity;

namespace DentalClinic.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual Patient Patient { get; set; }
        public virtual Doctor Doctor{ get; set; }
        public virtual Branch Branch { get; set; }
    }
}
