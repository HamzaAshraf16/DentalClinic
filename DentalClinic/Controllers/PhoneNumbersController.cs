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
        private readonly ClinicContext _context;

        public PhoneNumbersController(ClinicContext context)
        {
            _context = context;
        }





    }


}
