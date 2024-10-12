using DentalClinic.DTO;
using DentalClinic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<ActionResult<IEnumerable<DoctorDTO>>> GetDoctors()
    {
        var doctors = await _context.Doctors
                                    .Include(d => d.DoctorWorkBranches)
                                    .ThenInclude(dwb => dwb.Branch)
                                    .ToListAsync();

        var doctorDtos = doctors.Select(doctor => new DoctorDTO
        {
            DoctorId = doctor.DoctorId,
            Name = doctor.Name,
            UserId = doctor.UserId,
            PhoneNumber = doctor.PhoneNumber,
            WorkBranches = doctor.DoctorWorkBranches.Select(dwb => new DoctorWorkBranchDTO
            {
                Day = dwb.Day,
                StartTime = dwb.StartTime,
                EndTime = dwb.EndTime,
                BranchName = dwb.Branch.Name
            }).ToList()
        }).ToList();

        return Ok(doctorDtos);
    }

    // GET: api/Doctor/5
    [HttpGet("{id}")]
    public async Task<ActionResult<DoctorDTO>> GetDoctorById(int id)
    {
        var doctor = await _context.Doctors
                                   .Include(d => d.DoctorWorkBranches)
                                   .ThenInclude(dwb => dwb.Branch)
                                   .FirstOrDefaultAsync(d => d.DoctorId == id);

        if (doctor == null)
        {
            return NotFound(new { message = "Doctor not found" });
        }

        var doctorDto = new DoctorDTO
        {
            DoctorId = doctor.DoctorId,
            Name = doctor.Name,
            PhoneNumber = doctor.PhoneNumber,
            WorkBranches = doctor.DoctorWorkBranches.Select(dwb => new DoctorWorkBranchDTO
            {
                Day = dwb.Day,
                StartTime = dwb.StartTime,
                EndTime = dwb.EndTime,
                BranchName = dwb.Branch.Name
            }).ToList()
        };

        return Ok(doctorDto);
    }

    // PUT: api/Doctor/5
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

        existingDoctor.PhoneNumber = doctorDto.PhoneNumber;

        // Mark properties as modified

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
            return NotFound(new { message = "Doctor not found." });
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