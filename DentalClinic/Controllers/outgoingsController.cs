using DentalClinic.DTO;
using DentalClinic.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DentalClinic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class outgoingsController : ControllerBase
    {
        private readonly ClinicContext _context;

        public outgoingsController(ClinicContext context)
        {
            _context = context;
        }

        // GET: api/outgoings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<outgoings>>> GetOutgoings()
        {
            return await _context.outgoings.ToListAsync();
        }

        // GET: api/outgoings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<outgoings>> GetOutgoing(int id)
        {
            var outgoing = await _context.outgoings.FindAsync(id);

            if (outgoing == null)
            {
                return NotFound();
            }

            return outgoing;
        }

        // POST: api/outgoings
        [HttpPost]
        public async Task<ActionResult<outgoings>> PostOutgoing(outgoingsDTO outgoingDto)
        {
            // Map the DTO to the outgoing entity
            var outgoing = new outgoings
            {
                Date = outgoingDto.Date,
                NameOfOutgoings = outgoingDto.NameOfOutgoings,
                Cost = outgoingDto.Cost,
                BranchID = outgoingDto.BranchID,
                DoctorId = outgoingDto.DoctorId
            };

            _context.outgoings.Add(outgoing);
            await _context.SaveChangesAsync();

            // Return the created outgoing with the new ID
            return CreatedAtAction(nameof(GetOutgoing), new { id = outgoing.outgoingsId }, outgoingDto);
        }

        // PUT: api/outgoings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOutgoing(int id, outgoings outgoing)
        {
            if (id != outgoing.outgoingsId)
            {
                return BadRequest();
            }

            _context.Entry(outgoing).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OutgoingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/outgoings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOutgoing(int id)
        {
            var outgoing = await _context.outgoings.FindAsync(id);
            if (outgoing == null)
            {
                return NotFound();
            }

            _context.outgoings.Remove(outgoing);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OutgoingExists(int id)
        {
            return _context.outgoings.Any(e => e.outgoingsId == id);
        }
    }
}