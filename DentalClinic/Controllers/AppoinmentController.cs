using DentalClinic.DTO;
using DentalClinic.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
                    PatientId=a.Patient.PatientId,
                    DoctorId=a.DoctorId,
                    Status = (int)a.Status
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
                    PatientAge=a.Patient.Age,
                    Status = (int)a.Status
                })
                .FirstOrDefaultAsync();

            if (appointment == null)
            {
                return NotFound();
            }

            return Ok(appointment);
        }


        [HttpPost]
        public async Task<ActionResult<AppointmentDto>> ADD(AppointmentDto appointmentDto)
        {

            var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Name == appointmentDto.DoctorName);
            if (doctor == null)
            {
                return BadRequest("Doctor not found");
            }
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.PatientId == appointmentDto.PatientId);
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
            var resultDto = new AppointmentDto
            {
                AppointmentId = appointment.AppointmentId,
                Cost = appointment.Cost,
                Time = appointment.Time.ToString(),
                Date = appointment.Date,
                Reports = appointment.Reports,
                Type = appointment.Type,
                DoctorName = doctor.Name,
                PatientName = patient.Name
            };

            return CreatedAtAction("GetAppointment", new { id = appointment.AppointmentId }, resultDto);
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
            appointment.Status = (AppointmentStatus)appointmentDto.Status;

            _context.Entry(appointment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment == null)
                {
                    return NotFound();
                }
                var patientId = appointment.PatientId;
                var appointmentTime = appointment.Time;
                var appointmentDate = appointment.Date.ToString("yyyy-MM-dd");
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();

                var notification = new Notification
                {
                    PatientId = patientId,
                    Message = $"تم إلغاء الحجز المسجل بتاريخ {appointmentDate} , وبمعاد {appointmentTime}  قم بالحجز مره اخري",
                    IsRead = false,
                    CreatedDate = DateTime.Now
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex) {
                return StatusCode(500, "حدث خطأ أثناء معالجة الطلب.");

            }

        }






        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointmentsForPatient(int patientId)
        {
            var appointments = await _context.Appointments
                .Where(a => a.PatientId == patientId)
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
                    PatientPhoneNumber = a.Patient.PhoneNumber,
                    PatientGender = a.Patient.Gender,
                    PatientAge = a.Patient.Age
                })
                .ToListAsync();
            if (appointments == null || !appointments.Any())
            {
                return NotFound("No appointments found for the user.");
            }
            return Ok(appointments);
        }

    }
}
