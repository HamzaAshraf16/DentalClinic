using DentalClinic.DTO;
using DentalClinic.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentalClinic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorWorkBranchController : ControllerBase
    {
        private readonly ClinicContext _context;

        public DoctorWorkBranchController(ClinicContext Context)
        {
            _context=Context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<Doctor_Work_Branch> work = await _context.DoctorWorkBranchs.Include(d => d.Doctor)
                .Include(b => b.Branch).ToListAsync();
            List<BranchInfoandDoctor> branchDoctors = new List<BranchInfoandDoctor>();
            foreach (Doctor_Work_Branch item in work)
            {
                BranchInfoandDoctor WorkIn = new BranchInfoandDoctor();
                WorkIn.DoctorWorkBranchId = item.DoctorWorkBranchId;
                WorkIn.DoctorName = item.Doctor.Name;
                WorkIn.BranchName = item.Branch.Name;
                WorkIn.Day = item.Day;
                WorkIn.StartTime = item.StartTime;
                WorkIn.EndTime = item.EndTime;
                WorkIn.IsWork = item.IsWork;
                branchDoctors.Add(WorkIn);
            }
            return Ok(branchDoctors);
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult> GetDoctorWorkBranch(int id)
        {
            if (id == null) 
            {
                return BadRequest(new { message = "the id is required" });
            }
            try
            {
                var doctorWorkBranch = await _context.DoctorWorkBranchs
               .Include(dw => dw.Doctor)
               .Include(dw => dw.Branch)
               .Where(dw => dw.DoctorWorkBranchId == id)
               .Select(dw => new BranchInfoandDoctor
               {
                   DoctorWorkBranchId = dw.DoctorWorkBranchId,
                   Day = dw.Day,
                   IsWork = dw.IsWork,
                   StartTime = dw.StartTime,
                   EndTime = dw.EndTime,
                   BranchName = dw.Branch.Name,
                   DoctorName = dw.Doctor.Name,
               })
               .FirstOrDefaultAsync();

                if (doctorWorkBranch == null)
                {
                    return NotFound();
                }

                return Ok(doctorWorkBranch);
            }
            catch (Exception ex) 
            {
                return StatusCode(500, new { message = "An error occurred while retrieving data.", error = ex.Message });

            }

        }



        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateDoctorWorkBranch(int id, BranchInfoandDoctor dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }
            try
            {
                var doctorWorkBranch = await _context.DoctorWorkBranchs.FirstOrDefaultAsync(w => w.DoctorWorkBranchId == id);
                if (doctorWorkBranch == null)
                {
                    return NotFound(new { message = "The id Not Found" });
                }
                var Branch = _context.Branchs.FirstOrDefault(d => d.Name == dto.BranchName);
                if (Branch == null)
                {
                    return BadRequest("Branch not found");
                }
                var Doctor = _context.Doctors.FirstOrDefault(d => d.Name == dto.DoctorName);
                if (Doctor == null)
                {
                    return BadRequest("Doctor not found");
                }

                doctorWorkBranch.Day = dto.Day;
                doctorWorkBranch.IsWork = dto.IsWork;
                doctorWorkBranch.StartTime = dto.StartTime;
                doctorWorkBranch.EndTime = dto.EndTime;

                await _context.SaveChangesAsync();

                return Ok(new {message="The data has Updated Successfully"});

            } catch (DbUpdateException db)
            {
                return StatusCode(500, new { message = "An error occurred while Update the employee.", error = db.Message });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }


        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteDoctorWorkBranch(int id)
        {
            if (id == null)
            {
                return BadRequest(new { message = "the id is required" });
            }
            try
            {
                var doctorWorkBranch = await _context.DoctorWorkBranchs.FirstOrDefaultAsync(w => w.DoctorWorkBranchId == id);

                if (doctorWorkBranch == null)
                {
                    return NotFound(new {message="The data Not Found"});
                }

                _context.DoctorWorkBranchs.Remove(doctorWorkBranch);
                await _context.SaveChangesAsync();

                return Ok(new {message="The Data Deleted Successfully"});
            }
            catch (DbUpdateException db) 
            {
                return StatusCode(500, new { message = "An error occurred while deleting data.", error = db.Message });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", error = ex.Message });
            }

        }

    }
}
