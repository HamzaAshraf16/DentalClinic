using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DentalClinic.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DentalClinic.DTO;
using Microsoft.CodeAnalysis.Operations;
using Humanizer;

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
                                          BranchName = p.Branch.Name,
                                          BranchID = p.BranchID,
                                          BranchLocation = p.Branch.Location,
                                      })
                                      .ToList();

            if (phoneNumbers == null || !phoneNumbers.Any())
            {
                return NotFound(new { message = "لا توجد أرقام هاتف" });
            }
            return Ok(phoneNumbers);
        }


        [HttpPost]
        public IActionResult AddPhoneNumber([FromBody] PhoneNumberDto dto)
        {
            if (dto == null)
            {
                return BadRequest("بيانات الهاتف غير صالحة");
            }

            if (dto.BranchID <= 0)
                // Validate model state
                if (!ModelState.IsValid)
                {
                    return BadRequest(" رقم id  خطأ ");
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

            return CreatedAtAction(nameof(GetAllPhoneNumbers), new { id = phoneNumber.Phonenumber }, phoneNumber);
            // Return the created phone number
            return CreatedAtAction(nameof(GetAllPhoneNumbers), new { id = phoneNumber.Phonenumber }, new
            {
                phoneNumber.Phonenumber,
                BranchName = dto.BranchName
            });
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePhoneNumber(int id, [FromBody] PhoneNumberDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var phoneNumber = context.PhoneNumbers
                .Include(p => p.Branch)
                .FirstOrDefault(p => p.Id == id);

            if (phoneNumber == null)
            {
                return NotFound("رقم الهاتف غير موجود.");
            }


            phoneNumber.Phonenumber = dto.Phonenumber;
            phoneNumber.BranchID = dto.BranchID;

            var response = new
            {
                phoneNumber.Id,
                phoneNumber.Phonenumber,
                phoneNumber.BranchID,
                branchName = phoneNumber.Branch?.Name
            };

            return Ok(response);
        }


        [HttpDelete("{phonenumber}")]
        public IActionResult DeletePhoneNumber(string phonenumber)
        {
            var phoneNumber = context.PhoneNumbers.FirstOrDefault(p => p.Phonenumber == phonenumber);
            if (phoneNumber == null)
            {
                return NotFound("رقم الهاتف غير موجود.");
            }

            context.PhoneNumbers.Remove(phoneNumber);
            context.SaveChanges();

            return Ok("تم حذف رقم الهاتف بنجاح.");
        }



    }
}

























