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
                return NotFound("لا توجد أرقام هاتف");
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
            {
                return BadRequest(" رقم id  خطأ ");
            }


            var branchExists = context.Branchs.Any(b => b.BranchId == dto.BranchID);
            if (!branchExists)
            {
                return BadRequest("الفرع غير موجود");
            }

            var phoneNumber = new PhoneNumber
            {
                Phonenumber = dto.Phonenumber,
                BranchID = dto.BranchID
            };

            context.PhoneNumbers.Add(phoneNumber);
            context.SaveChanges();

            return CreatedAtAction(nameof(GetAllPhoneNumbers), new { id = phoneNumber.Phonenumber }, phoneNumber);
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


        ////[HttpDelete("{id}")]
        ////public IActionResult deletePhoneNumberById(int id)
        ////{
        ////   
        ////    var phoneNumber = context.PhoneNumbers.FirstOrDefault(p => p.Id == id);

        ////   
        ////    if (phoneNumber == null)
        ////    {
        ////        return NotFound("رقم الهاتف غير موجود.");
        ////    }

        ////    
        ////    context.PhoneNumbers.Remove(phoneNumber);
        ////    context.SaveChanges();

        ////    
        ////    return Ok(new { message = "تم حذف رقم الهاتف بنجاح." });
        ////}

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
























