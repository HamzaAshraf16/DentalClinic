using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DentalClinic.Models;
using DentalClinic.DTO;
using Microsoft.CodeAnalysis.Scripting;

namespace DentalClinic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly ClinicContext _context;

        public PatientController(ClinicContext context)
        {
            _context = context;
        }
       

        [HttpGet("GetAllPatientsWithHistory")]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetAllPatientsWithHistory()
        {
            var patients = await _context.Patients
                .Include(p => p.PatientHistory)
                .Select(p => new PatientDto
                {
                    PatientId = p.PatientId,                    
                    Name = p.Name,
                    Gender = p.Gender,
                    PhoneNumber = p.PhoneNumber,
                    Address = p.Address,
                    Hypertension = p.PatientHistory.Hypertension,
                    Diabetes = p.PatientHistory.Diabetes,
                    StomachAche =  p.PatientHistory.StomachAche,
                    PeriodontalDisease = p.PatientHistory.PeriodontalDisease,
                    IsPregnant=p.PatientHistory.IsPregnant,
                    IsBreastfeeding=p.PatientHistory.IsBreastfeeding,
                    IsSmoking=p.PatientHistory.IsSmoking,
                    KidneyDiseases=p.PatientHistory.KidneyDiseases,
                    HeartDiseases=p.PatientHistory.HeartDiseases
                })
                .ToListAsync();

            return Ok(patients);
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDto>> GetPatientById(int id)
        {
            var patient = await _context.Patients
                                        .Include(p => p.PatientHistory)
                                        .Where(p => p.PatientId == id)
                                        .Select(p => new PatientDto
                                        {
                                            PatientId = p.PatientId,
                                            Name = p.Name,
                                            Gender = p.Gender,
                                            PhoneNumber = p.PhoneNumber,
                                            Address = p.Address,
                                            Hypertension = p.PatientHistory.Hypertension,
                                            Diabetes = p.PatientHistory.Diabetes,
                                            StomachAche = p.PatientHistory.StomachAche,
                                            PeriodontalDisease = p.PatientHistory.PeriodontalDisease,
                                            IsPregnant = p.PatientHistory.IsPregnant,
                                            IsBreastfeeding = p.PatientHistory.IsBreastfeeding,
                                            IsSmoking = p.PatientHistory.IsSmoking,
                                            KidneyDiseases = p.PatientHistory.KidneyDiseases,
                                            HeartDiseases = p.PatientHistory.HeartDiseases
                                        }).FirstOrDefaultAsync();

            if (patient == null)
            {
                return NotFound(new {message= $"Patient with ID {id} not found." });
            }

            return Ok(patient);
        }

        
        [HttpGet("GetByName/{name}")]
        public async Task<ActionResult<PatientDto>> GetPatientByName(string name)
        {
            var patient = await _context.Patients
                                        .Include(p => p.PatientHistory)
                                        .Where(p => p.Name.ToLower() == name.ToLower())
                                        .Select(p => new PatientDto
                                        {
                                            PatientId = p.PatientId,
                                            Name = p.Name,
                                            Gender = p.Gender,
                                            PhoneNumber = p.PhoneNumber,
                                            Address = p.Address,
                                            Hypertension = p.PatientHistory.Hypertension,
                                            Diabetes = p.PatientHistory.Diabetes,
                                            StomachAche = p.PatientHistory.StomachAche,
                                            PeriodontalDisease = p.PatientHistory.PeriodontalDisease,
                                            IsPregnant = p.PatientHistory.IsPregnant,
                                            IsBreastfeeding = p.PatientHistory.IsBreastfeeding,
                                            IsSmoking = p.PatientHistory.IsSmoking,
                                            KidneyDiseases = p.PatientHistory.KidneyDiseases,
                                            HeartDiseases = p.PatientHistory.HeartDiseases
                                        }).FirstOrDefaultAsync();

            if (patient == null)
            {
                return NotFound($"Patient with name {name} not found.");
            }

            return Ok(patient);
        }


       
        [HttpPut("EditPatient/{id}")]
        public async Task<IActionResult> EditPatient(int id, [FromBody] PatientDto patientDto)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); 
            }

            
            var existingPatient = await _context.Patients
                                                .Include(p => p.PatientHistory)
                                                .FirstOrDefaultAsync(p => p.PatientId == id);

            if (existingPatient == null)
            {
                return NotFound("Patient not found.");
            }

           
            existingPatient.Name = patientDto.Name;
    
            
            existingPatient.Gender = patientDto.Gender;
            existingPatient.PhoneNumber = patientDto.PhoneNumber;
            existingPatient.Address = patientDto.Address;

          
            if (existingPatient.PatientHistory != null)
            {
                existingPatient.PatientHistory.Hypertension = patientDto.Hypertension;
                existingPatient.PatientHistory.Diabetes = patientDto.Diabetes;
                existingPatient.PatientHistory.StomachAche = patientDto.StomachAche;
                existingPatient.PatientHistory.PeriodontalDisease = patientDto.PeriodontalDisease;
                existingPatient.PatientHistory.IsPregnant = patientDto.IsPregnant;
                existingPatient.PatientHistory.IsBreastfeeding = patientDto.IsBreastfeeding;
                existingPatient.PatientHistory.IsSmoking = patientDto.IsSmoking;
                existingPatient.PatientHistory.KidneyDiseases = patientDto.KidneyDiseases;
                existingPatient.PatientHistory.HeartDiseases = patientDto.HeartDiseases;
            }
            try
            {
                
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientExists(id))
                {
                    return NotFound("Patient not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

       
        private bool PatientExists(int id)
        {
            return _context.Patients.Any(p => p.PatientId == id);
        }

        
        [HttpPost("AddPatient")]
        public async Task<IActionResult> AddPatient([FromBody] AddPatientDto newPatientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            
            var patient = new Patient
            {
                
                Name = newPatientDto.Name,
                Gender = newPatientDto.Gender,
                PhoneNumber = newPatientDto.PhoneNumber,
                Address = newPatientDto.Address,
                PatientHistoryId = newPatientDto.PatientHistoryId
            };

           
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPatientById), new { id = patient.PatientId }, patient);
        }

        
        [HttpDelete("DeletePatient/{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            
            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
            {
                return NotFound(new {message="The patient not Founded"}); 
            }


            _context.Patients.Remove(patient); 
            await _context.SaveChangesAsync(); 

            return NoContent(); 
        }

    }
}
