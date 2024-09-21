using DentalClinic.DTO;
using DentalClinic.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace DentalClinic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientHistoryController : ControllerBase
    {
        private readonly ClinicContext context;

        public PatientHistoryController(ClinicContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var PatientHistories = await context.PatientsHistory
                                   .Include(ph => ph.Patient)
                                   .Select(ph => new PatientHistoryDto
                                   {
                                       PatientHistoryID = ph.PatientHistoryID,
                                       Hypertension = ph.Hypertension,
                                       Diabetes = ph.Diabetes,
                                       StomachAche = ph.StomachAche,
                                       PeriodontalDisease = ph.PeriodontalDisease,
                                       IsPregnant = ph.IsPregnant,
                                       IsBreastfeeding = ph.IsBreastfeeding,
                                       IsSmoking = ph.IsSmoking,
                                       KidneyDiseases = ph.KidneyDiseases,
                                       HeartDiseases = ph.HeartDiseases,
                                       Patient = new PatientForHistoryDto
                                       {
                                           PatientId = ph.Patient.PatientId,
                                           Name = ph.Patient.Name,
                                           Gender = ph.Patient.Gender,
                                           PhoneNumber = ph.Patient.PhoneNumber,
                                           Address = ph.Patient.Address
                                       }
                                   }).ToListAsync();
            return Ok(PatientHistories);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var PatientHistory = await context.PatientsHistory
                                 .Include(ph => ph.Patient)
                                 .Where(ph => ph.PatientHistoryID == id)
                                 .Select(ph => new PatientHistoryDto
                                 {
                                     PatientHistoryID = ph.PatientHistoryID,
                                     Hypertension = ph.Hypertension,
                                     Diabetes = ph.Diabetes,
                                     StomachAche = ph.StomachAche,
                                     PeriodontalDisease = ph.PeriodontalDisease,
                                     IsPregnant = ph.IsPregnant,
                                     IsBreastfeeding = ph.IsBreastfeeding,
                                     IsSmoking = ph.IsSmoking,
                                     KidneyDiseases = ph.KidneyDiseases,
                                     HeartDiseases = ph.HeartDiseases,
                                     Patient = new PatientForHistoryDto
                                     {
                                         PatientId = ph.Patient.PatientId,
                                         Name = ph.Patient.Name,
                                         Gender = ph.Patient.Gender,
                                         PhoneNumber = ph.Patient.PhoneNumber,
                                         Address = ph.Patient.Address
                                     }
                                 }).FirstOrDefaultAsync();

            if (PatientHistory == null)
            {
                return NotFound();
            }

            return Ok(PatientHistory);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult> GetPatientHistoryByPatientId(int patientId)
        {
            var patientHistory = await context.PatientsHistory
                .Include(ph => ph.Patient)
                .Where(ph => ph.Patient.PatientId == patientId)
                .Select(ph => new PatientHistoryDto
                {
                    PatientHistoryID = ph.PatientHistoryID,
                    Hypertension = ph.Hypertension,
                    Diabetes = ph.Diabetes,
                    StomachAche = ph.StomachAche,
                    PeriodontalDisease = ph.PeriodontalDisease,
                    IsPregnant = ph.IsPregnant,
                    IsBreastfeeding = ph.IsBreastfeeding,
                    IsSmoking = ph.IsSmoking,
                    KidneyDiseases = ph.KidneyDiseases,
                    HeartDiseases = ph.HeartDiseases,
                    Patient = new PatientForHistoryDto
                    {
                        PatientId = ph.Patient.PatientId,
                        Name = ph.Patient.Name,
                        Gender = ph.Patient.Gender,
                        PhoneNumber = ph.Patient.PhoneNumber,
                        Address = ph.Patient.Address
                    }
                })
                .FirstOrDefaultAsync();

            if (patientHistory == null)
            {
                return NotFound($"No patient history found for patient with ID {patientId}");
            }

            return Ok(patientHistory);
        }
        [HttpPost]
        public async Task<IActionResult> CreatePatientHistory(PatientHistoryCreateDto patientHistoryCreate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var patient = await context.Patients.FindAsync(patientHistoryCreate.PatientId);
            if (patient == null)
            {
                return BadRequest("Invalid PatientId");
            }
            var patientHistory = new PatientHistory
            {
                Hypertension = patientHistoryCreate.Hypertension,
                Diabetes = patientHistoryCreate.Diabetes,
                StomachAche = patientHistoryCreate.StomachAche,
                PeriodontalDisease = patientHistoryCreate.PeriodontalDisease,
                IsPregnant = patientHistoryCreate.IsPregnant,
                IsBreastfeeding = patientHistoryCreate.IsBreastfeeding,
                IsSmoking = patientHistoryCreate.IsSmoking,
                KidneyDiseases = patientHistoryCreate.KidneyDiseases,
                HeartDiseases = patientHistoryCreate.HeartDiseases,
                Patient = patient
            };

            context.PatientsHistory.Add(patientHistory);
            await context.SaveChangesAsync();

            var patientHistoryDto = new PatientHistoryDto
            {
                PatientHistoryID = patientHistory.PatientHistoryID,
                Hypertension = patientHistory.Hypertension,
                Diabetes = patientHistory.Diabetes,
                StomachAche = patientHistory.StomachAche,
                PeriodontalDisease = patientHistory.PeriodontalDisease,
                IsPregnant = patientHistory.IsPregnant,
                IsBreastfeeding = patientHistory.IsBreastfeeding,
                IsSmoking = patientHistory.IsSmoking,
                KidneyDiseases = patientHistory.KidneyDiseases,
                HeartDiseases = patientHistory.HeartDiseases,
                Patient = new PatientForHistoryDto
                {
                    PatientId = patient.PatientId,
                    Name = patient.Name,
                    Gender = patient.Gender,
                    PhoneNumber = patient.PhoneNumber,
                    Address = patient.Address
                }
            };

            return CreatedAtAction(nameof(GetById), new { id = patientHistory.PatientHistoryID }, patientHistoryDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatientHistory(int id, PatientHistoryUpdateDto patientHistoryUpdate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var patientHistory = await context.PatientsHistory.FindAsync(id);
            if (patientHistory == null)
            {
                return NotFound("patientHistory not Found");
            }

            patientHistory.Hypertension = patientHistoryUpdate.Hypertension;
            patientHistory.Diabetes = patientHistoryUpdate.Diabetes;
            patientHistory.StomachAche = patientHistoryUpdate.StomachAche;
            patientHistory.PeriodontalDisease = patientHistoryUpdate.PeriodontalDisease;
            patientHistory.IsPregnant = patientHistoryUpdate.IsPregnant;
            patientHistory.IsBreastfeeding = patientHistoryUpdate.IsBreastfeeding;
            patientHistory.IsSmoking = patientHistoryUpdate.IsSmoking;
            patientHistory.KidneyDiseases = patientHistoryUpdate.KidneyDiseases;
            patientHistory.HeartDiseases = patientHistoryUpdate.HeartDiseases;

            await context.SaveChangesAsync();

            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatientHistory(int id)
        {
            var patientHistory = await context.PatientsHistory.FindAsync(id);
            if (patientHistory == null)
            {
                return NotFound("PatientHistory not found");
            }

            context.PatientsHistory.Remove(patientHistory);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
