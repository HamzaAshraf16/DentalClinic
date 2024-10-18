using DentalClinic.DTO;
using DentalClinic.Models;
using DentalClinic.Services;
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
        private readonly IPatientHistoryService _patientHistoryService;

        public PatientHistoryController(IPatientHistoryService patientHistoryService)
        {
            _patientHistoryService = patientHistoryService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientHistoryDto>>> GetAll()
        {
            return await _patientHistoryService.GetAll();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientHistoryDto>> GetById(int id)
        {
            return await _patientHistoryService.GetById(id);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<PatientHistoryDto>> GetPatientHistoryByPatientId(int patientId)
        {
            return await _patientHistoryService.GetPatientHistoryByPatientId(patientId);
        }

        [HttpPost]
        public async Task<ActionResult<PatientHistoryDto>> CreatePatientHistory(PatientHistoryCreateDto patientHistoryCreate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return await _patientHistoryService.CreatePatientHistory(patientHistoryCreate);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatientHistory(int id, PatientHistoryUpdateDto patientHistoryUpdate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return await _patientHistoryService.UpdatePatientHistory(id, patientHistoryUpdate);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatientHistory(int id)
        {
            return await _patientHistoryService.DeletePatientHistory(id);
        }
    }
}
