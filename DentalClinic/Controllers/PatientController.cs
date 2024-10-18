using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DentalClinic.Models;
using DentalClinic.DTO;
using Microsoft.CodeAnalysis.Scripting;
using System.Security.Claims;

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
                    Age = p.Age,
                    UserId = p.UserId,
                    Hypertension = p.PatientHistory.Hypertension,
                    Diabetes = p.PatientHistory.Diabetes,
                    StomachAche = p.PatientHistory.StomachAche,
                    PeriodontalDisease = p.PatientHistory.PeriodontalDisease,
                    IsPregnant = p.PatientHistory.IsPregnant,
                    IsBreastfeeding = p.PatientHistory.IsBreastfeeding,
                    IsSmoking = p.PatientHistory.IsSmoking,
                    KidneyDiseases = p.PatientHistory.KidneyDiseases,
                    HeartDiseases = p.PatientHistory.HeartDiseases,
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
                                            Age = p.Age,
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
                return NotFound(new { message = $"Patient with ID {id} not found." });
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
                                            Age = p.Age,
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
            existingPatient.Age = patientDto.Age;


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
                Age = newPatientDto.Age,
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
            var patienthistory = await _context.PatientsHistory.FindAsync(patient.PatientHistoryId);
            var user = await _context.Users.FindAsync(patient.UserId);

            if (patient == null || patienthistory == null || user == null)
            {
                return NotFound(new { message = "The patient not Founded" });
            }


            _context.Patients.Remove(patient);
            _context.PatientsHistory.Remove(patienthistory);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }








        [HttpPost("AddPatientWithHistory")]
        public async Task<IActionResult> AddPatientWithHistory([FromBody] PatientDto newPatientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Create the PatientHistory object from the DTO
            var patientHistory = new PatientHistory
            {
                Hypertension = newPatientDto.Hypertension,
                Diabetes = newPatientDto.Diabetes,
                StomachAche = newPatientDto.StomachAche,
                PeriodontalDisease = newPatientDto.PeriodontalDisease,
                IsPregnant = newPatientDto.IsPregnant,
                IsBreastfeeding = newPatientDto.IsBreastfeeding,
                IsSmoking = newPatientDto.IsSmoking,
                KidneyDiseases = newPatientDto.KidneyDiseases,
                HeartDiseases = newPatientDto.HeartDiseases
            };

            // Create the Patient object and associate the history
            var patient = new Patient
            {
                Name = newPatientDto.Name,
                Gender = newPatientDto.Gender,
                PhoneNumber = newPatientDto.PhoneNumber,
                Address = newPatientDto.Address,
                Age = newPatientDto.Age,
                UserId = newPatientDto.UserId, // Assuming you want to store the UserId as well
                PatientHistory = patientHistory // Link the history to the patient
            };

            // Save both the patient and the history in a single transaction
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            // Create a PatientDto to return
            var patientDto = new PatientDto
            {
                PatientId = patient.PatientId,
                Name = patient.Name,
                Gender = patient.Gender,
                PhoneNumber = patient.PhoneNumber,
                Address = patient.Address,
                Age = patient.Age,
                UserId = patient.UserId,
                Hypertension = patientHistory.Hypertension,
                Diabetes = patientHistory.Diabetes,
                StomachAche = patientHistory.StomachAche,
                PeriodontalDisease = patientHistory.PeriodontalDisease,
                IsPregnant = patientHistory.IsPregnant,
                IsBreastfeeding = patientHistory.IsBreastfeeding,
                IsSmoking = patientHistory.IsSmoking,
                KidneyDiseases = patientHistory.KidneyDiseases,
                HeartDiseases = patientHistory.HeartDiseases
            };

            return CreatedAtAction(nameof(GetPatientById), new { id = patient.PatientId }, patientDto);
        }






        [HttpGet("GetLoggedInPatientProfile")]
        public async Task<ActionResult<PatientDto>> GetLoggedInPatientProfile()
        {
            var userId = User.FindFirst("nameidentifier")?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }
            var patient = await _context.Patients
                .Include(p => p.PatientHistory)
                .Where(p => p.UserId == userId)
                .Select(p => new PatientDto
                {
                    PatientId = p.PatientId,
                    Name = p.Name,
                    Gender = p.Gender,
                    PhoneNumber = p.PhoneNumber,
                    Address = p.Address,
                    Age = p.Age,
                    Hypertension = p.PatientHistory.Hypertension,
                    Diabetes = p.PatientHistory.Diabetes,
                    StomachAche = p.PatientHistory.StomachAche,
                    PeriodontalDisease = p.PatientHistory.PeriodontalDisease,
                    IsPregnant = p.PatientHistory.IsPregnant,
                    IsBreastfeeding = p.PatientHistory.IsBreastfeeding,
                    IsSmoking = p.PatientHistory.IsSmoking,
                    KidneyDiseases = p.PatientHistory.KidneyDiseases,
                    HeartDiseases = p.PatientHistory.HeartDiseases
                })
                .FirstOrDefaultAsync();
            if (patient == null)
            {
                return NotFound();
            }
            return Ok(patient);
        }
    }
}