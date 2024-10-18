using DentalClinic.DTO;
using DentalClinic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentalClinic.Services
{
    public class PatientHistoryService : IPatientHistoryService
    {
        private readonly ClinicContext _context;

        public PatientHistoryService(ClinicContext context)
        {
            _context = context;
        }

        public async Task<ActionResult<IEnumerable<PatientHistoryDto>>> GetAll()
        {
            var PatientHistories = await _context.PatientsHistory
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
            return new OkObjectResult(PatientHistories);
        }

        public async Task<ActionResult<PatientHistoryDto>> GetById(int id)
        {
            var PatientHistory = await _context.PatientsHistory
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
                return new NotFoundResult();
            }

            return new OkObjectResult(PatientHistory);
        }

        public async Task<ActionResult<PatientHistoryDto>> GetPatientHistoryByPatientId(int patientId)
        {
            var patientHistory = await _context.PatientsHistory
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
                return new NotFoundObjectResult($"No patient history found for patient with ID {patientId}");
            }

            return new OkObjectResult(patientHistory);
        }

        public async Task<ActionResult<PatientHistoryDto>> CreatePatientHistory(PatientHistoryCreateDto patientHistoryCreate)
        {
            var patient = await _context.Patients.FindAsync(patientHistoryCreate.PatientId);
            if (patient == null)
            {
                return new BadRequestObjectResult("Invalid PatientId");
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

            _context.PatientsHistory.Add(patientHistory);
            await _context.SaveChangesAsync();

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

            return new CreatedAtActionResult(nameof(GetById), "PatientHistory", new { id = patientHistory.PatientHistoryID }, patientHistoryDto);
        }

        public async Task<IActionResult> UpdatePatientHistory(int id, PatientHistoryUpdateDto patientHistoryUpdate)
        {
            var patientHistory = await _context.PatientsHistory.FindAsync(id);
            if (patientHistory == null)
            {
                return new NotFoundObjectResult("PatientHistory not found");
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

            await _context.SaveChangesAsync();

            return new NoContentResult();
        }

        public async Task<IActionResult> DeletePatientHistory(int id)
        {
            var patientHistory = await _context.PatientsHistory.FindAsync(id);
            if (patientHistory == null)
            {
                return new NotFoundObjectResult("PatientHistory not found");
            }

            _context.PatientsHistory.Remove(patientHistory);
            await _context.SaveChangesAsync();

            return new NoContentResult();
        }
    }
}
