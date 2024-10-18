using DentalClinic.DTO;
using DentalClinic.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentalClinic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly ClinicContext _context;
        public NotificationController(ClinicContext context)
        {
            _context = context;

        }
        [HttpGet("GetNotification")]
        public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetNotification()
        {
            var notification = await _context.Notifications
                .Include(p => p.Patient)
                .OrderByDescending(n => n.CreatedDate)
                .Select(p => new NotificationDTO
                {
                    NotificationId=p.NotificationId,
                    Message = p.Message,
                    CreatedDate=p.CreatedDate,
                    IsRead = p.IsRead,
                    PatientId = p.Patient.PatientId
                })
                .ToListAsync();

            return Ok(notification);
        }


        [HttpPut("{id}/markAsRead")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return NoContent(); // 204 No Content
        }
    }
}
