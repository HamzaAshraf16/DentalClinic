using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DentalClinic.Models;
using DentalClinic.DTO;

namespace DentalClinic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly ClinicContext _context;

        public DoctorController(ClinicContext context)
        {
            _context = context;
        }

        // GET: api/Doctor
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Doctor>>> GetDoctors()
        {
            return await _context.Doctors.Include(d => d.DoctorWorkBranches)
                                         .Include(d => d.Appointments)
                                         .ToListAsync();
        }


        // GET: api/Doctor/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Doctor>> GetDoctorbyId(int id)
        {
            var doctor = await _context.Doctors.Include(d => d.DoctorWorkBranches)
                                               .Include(d => d.Appointments)
                                               .FirstOrDefaultAsync(d => d.DoctorId == id);

            if (doctor == null)
            {
                return NotFound(new {message="The Doctor not Found"});
            }

            return Ok(doctor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutDoctor(int id, [FromBody] UpdateDoctorDto doctorDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Fetch the doctor entity from the database
            var existingDoctor = await _context.Doctors.FindAsync(id);
            if (existingDoctor == null)
            {
                return NotFound(new { message = "Doctor not found." });
            }

            // Update only the Password and PhoneNumber fields from the DTO
            existingDoctor.Password = doctorDto.Password;
            existingDoctor.PhoneNumber = doctorDto.PhoneNumber;

            // Mark properties as modified
            _context.Entry(existingDoctor).Property(d => d.Password).IsModified = true;
            _context.Entry(existingDoctor).Property(d => d.PhoneNumber).IsModified = true;

            try
            {
                // Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorExists(id))
                {
                    return NotFound(new { message = "Doctor not found." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Doctor/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound(new { message = "The Doctor not Found" });
            }

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.DoctorId == id);
        }
    }
}
