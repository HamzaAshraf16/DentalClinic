using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DentalClinic.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DentalClinic.DTO;
using Microsoft.CodeAnalysis.Operations;

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

        // GET: api/PhoneNumbers
        [HttpGet]
        public IActionResult GetPhoneNumbers()
        {
            var phoneNumbers = context.PhoneNumbers
                                      .Include(p => p.Branch)
                                      .Select(p => new PhoneNumberDto
                                      {
                                          Phonenumber = p.Phonenumber,
<<<<<<< HEAD
                                          BranchID = p.BranchID,
                                          location = p.Branch.Location,
                                          BranchName = p.Branch.Name
=======
                                          BranchID=p.BranchID,
                                          BranchName = p.Branch.Name,
                                          BranchLocation = p.Branch.Location,

>>>>>>> b8a636330d9b86b565486406fe7823144c752b09
                                      })
                                      .ToList();

            if (phoneNumbers == null || !phoneNumbers.Any())
            {
                return NotFound(new { message = "لا توجد أرقام هاتف" });
            }
            return Ok(phoneNumbers);
        }

        // GET: api/PhoneNumbers/5
        [HttpGet("{id}")]
        public IActionResult GetPhoneNumber(int id)
        {
            var phoneNumber = context.PhoneNumbers
                                     .Include(p => p.Branch)
                                     .Where(p => p.Id == id)
                                     .Select(p => new PhoneNumberDto
                                     {
                                         Phonenumber = p.Phonenumber,
                                         BranchID = p.BranchID,
                                         location = p.Branch.Location,
                                         BranchName = p.Branch.Name
                                     })
                                     .FirstOrDefault();

            if (phoneNumber == null)
            {
                return NotFound(new { message = "رقم الهاتف غير موجود" });
            }

<<<<<<< HEAD
            return Ok(phoneNumber);
        }

        // POST: api/PhoneNumbers
        [HttpPost]
        public IActionResult PostPhoneNumber([FromBody] PhoneNumberDto phoneNumberDto)
=======
            // Validate model state
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if the branch exists
            var branchExists = context.Branchs.Any(b => b.BranchId == dto.BranchID);
            if (!branchExists)
            {
                return BadRequest("الفرع غير موجود");
            }

            // Create and add the new phone number
            var phoneNumber = new PhoneNumber
            {
                Phonenumber = dto.Phonenumber,
                BranchID = dto.BranchID
            };

            context.PhoneNumbers.Add(phoneNumber);
            context.SaveChanges();

            // Return the created phone number
            return CreatedAtAction(nameof(GetAllPhoneNumbers), new { id = phoneNumber.Phonenumber }, new
            {
                phoneNumber.Phonenumber,
                BranchName = dto.BranchName
            });
        }




        [HttpPut("{id}")]
        public IActionResult UpdatePhoneNumber(int id, [FromBody] PhoneNumberDto dto)
>>>>>>> b8a636330d9b86b565486406fe7823144c752b09
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var branch = context.Branchs.Find(phoneNumberDto.BranchID);
            if (branch == null)
            {
                return BadRequest(new { message = "الفرع غير موجود" });
            }

            var phoneNumber = new PhoneNumber
            {
                Phonenumber = phoneNumberDto.Phonenumber,
                BranchID = phoneNumberDto.BranchID
            };

            context.PhoneNumbers.Add(phoneNumber);
            context.SaveChanges();

            return CreatedAtAction(nameof(GetPhoneNumber), new { id = phoneNumber.Id }, phoneNumberDto);
        }

        // PUT: api/PhoneNumbers/5
        [HttpPut("{id}")]
        public IActionResult PutPhoneNumber(int id, [FromBody] PhoneNumberDto phoneNumberDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var phoneNumber = context.PhoneNumbers.Find(id);
            if (phoneNumber == null)
            {
                return NotFound(new { message = "رقم الهاتف غير موجود" });
            }

            var branch = context.Branchs.Find(phoneNumberDto.BranchID);
            if (branch == null)
            {
                return BadRequest(new { message = "الفرع غير موجود" });
            }

            phoneNumber.Phonenumber = phoneNumberDto.Phonenumber;
            phoneNumber.BranchID = phoneNumberDto.BranchID;

            context.SaveChanges();

            return NoContent();
        }

        // DELETE: api/PhoneNumbers/5
        [HttpDelete("{id}")]
        public IActionResult DeletePhoneNumber(int id)
        {
            var phoneNumber = context.PhoneNumbers.Find(id);
            if (phoneNumber == null)
            {
                return NotFound(new { message = "رقم الهاتف غير موجود" });
            }

            context.PhoneNumbers.Remove(phoneNumber);
            context.SaveChanges();

            return Ok(new { message = "تم حذف رقم الهاتف بنجاح" });
        }


    }



}
























