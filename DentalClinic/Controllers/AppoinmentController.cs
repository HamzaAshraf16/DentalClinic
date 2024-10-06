using DentalClinic.DTO;
using DentalClinic.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentalClinic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppoinmentController : ControllerBase
    {
        private readonly ClinicContext _context;

        public AppoinmentController(ClinicContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Select(a => new AppointmentDto
                {
                    AppointmentId = a.AppointmentId,
                    Cost = a.Cost,
                    Time = a.Time.ToString(),
                    Date = a.Date,
                    Reports = a.Reports,
                    Type = a.Type,
                    DoctorName = a.Doctor.Name,
                    PatientName = a.Patient.Name,
                    PatientPhoneNumber=a.Patient.PhoneNumber,
                    PatientGender=a.Patient.Gender,
                    PatientAge=a.Patient.Age,
                    PatientId=a.Patient.PatientId
                })
                .ToListAsync();

            return Ok(appointments);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentDto>> GetAppointment(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Where(a => a.AppointmentId == id)
                .Select(a => new AppointmentDto
                {
                    AppointmentId = a.AppointmentId,
                    Cost = a.Cost,
                    Time = a.Time.ToString(),
                    Date = a.Date,
                    Reports = a.Reports,
                    Type = a.Type,
                    DoctorName = a.Doctor.Name,
                    PatientName = a.Patient.Name,
                    PatientPhoneNumber=a.Patient.PhoneNumber,
                    PatientGender=a.Patient.Gender,
                    PatientAge=a.Patient.Age
                })
                .FirstOrDefaultAsync();

            if (appointment == null)
            {
                return NotFound();
            }

            return Ok(appointment);
        }


        [HttpPost]
        public async Task<ActionResult<Appointment>> ADD(AppointmentDto appointmentDto)
        {

            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Name == appointmentDto.DoctorName);
            if (doctor == null)
            {
                return BadRequest("Doctor not found");
            }

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Name == appointmentDto.PatientName);
            if (patient == null)
            {
                return BadRequest("Patient not found");
            }
            if (!TimeSpan.TryParse(appointmentDto.Time, out var time))
            {
                return BadRequest("Invalid time format");
            }

            var appointment = new Appointment
            {
                Cost = appointmentDto.Cost,
                Time = time,
                Date = appointmentDto.Date,
                Reports = appointmentDto.Reports,
                Type = appointmentDto.Type,
                DoctorId = doctor.DoctorId,
                PatientId = patient.PatientId
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAppointment", new { id = appointment.AppointmentId }, appointment);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, AppointmentDto appointmentDto)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }


            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Name == appointmentDto.DoctorName);
            if (doctor == null)
            {
                return BadRequest("Doctor not found");
            }

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.Name == appointmentDto.PatientName);
            if (patient == null)
            {
                return BadRequest("Patient not found");
            }
            if (!TimeSpan.TryParse(appointmentDto.Time, out var time))
            {
                return BadRequest("Invalid time format");
            }

            appointment.Cost = appointmentDto.Cost;
            appointment.Time = time;
            appointment.Date = appointmentDto.Date;
            appointment.Reports = appointmentDto.Reports;
            appointment.Type = appointmentDto.Type;
            appointment.DoctorId = doctor.DoctorId;
            appointment.PatientId = patient.PatientId;

            _context.Entry(appointment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
