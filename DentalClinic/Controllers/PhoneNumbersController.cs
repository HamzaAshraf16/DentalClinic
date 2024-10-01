using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DentalClinic.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DentalClinic.DTO;

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
                                          BranchID = p.BranchID,
                                          location = p.Branch.Location,
                                          BranchName = p.Branch.Name
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

            return Ok(phoneNumber);
        }

        // POST: api/PhoneNumbers
        [HttpPost]
        public IActionResult PostPhoneNumber([FromBody] PhoneNumberDto phoneNumberDto)
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
























