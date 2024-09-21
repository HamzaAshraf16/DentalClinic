using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DentalClinic.Models; 
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DentalClinic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhoneNumbersController : ControllerBase
    {
        private readonly ClinicContext context;

        public PhoneNumbersController(ClinicContext _context)
        {
            context = _context;
        }

        [HttpGet]
        public IActionResult GetAllPhoneNumbers()
        {
            var phoneNumbers = context.PhoneNumbers
                                      .Include(p => p.Branch) 
                                      .Select(p => new
                                      {
                                          Phonenumber = p.Phonenumber,
                                          BranchName = p.Branch.Name 
                                      })
                                      .ToList(); 

            if (phoneNumbers == null || !phoneNumbers.Any())
            {
                return NotFound(new { message = "لا توجد أرقام هاتف" });
            }
            return Ok(phoneNumbers);
        }







    }


}
