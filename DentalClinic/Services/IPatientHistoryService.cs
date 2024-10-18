using DentalClinic.DTO;
using Microsoft.AspNetCore.Mvc;

namespace DentalClinic.Services
{
    public interface IPatientHistoryService
    {
        Task<ActionResult<IEnumerable<PatientHistoryDto>>> GetAll();
        Task<ActionResult<PatientHistoryDto>> GetById(int id);
        Task<ActionResult<PatientHistoryDto>> GetPatientHistoryByPatientId(int patientId);
        Task<ActionResult<PatientHistoryDto>> CreatePatientHistory(PatientHistoryCreateDto patientHistoryCreate);
        Task<IActionResult> UpdatePatientHistory(int id, PatientHistoryUpdateDto patientHistoryUpdate);
        Task<IActionResult> DeletePatientHistory(int id);
    }
}
