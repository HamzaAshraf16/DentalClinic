using DentalClinic.DTO;
using DentalClinic.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DentalClinic.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly ClinicContext _context;

        public BranchController(ClinicContext context)
        {
            _context = context;
        }

        // GET: api/Branch
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BranchDto>>> GetBranches()
        {
            var branches = await _context.Branchs.ToListAsync();
            var branchDtos = branches.Select(b => new BranchDto
            {
                BranchId = b.BranchId,
                BranchName = b.Name,
                BranchLocation = b.Location,
                UserId = b.UserId
            }).ToList();

            return branchDtos;
        }

        // GET: api/Branch/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BranchDto>> GetBranchById(int id)
        {
            var branch = await _context.Branchs.FindAsync(id);

            if (branch == null)
            {
                return NotFound(new {message="The Branch Not Found"});
            }

            var branchDto = new BranchDto
            {
                BranchId = branch.BranchId,
                BranchName = branch.Name,
                BranchLocation = branch.Location
            };

            return branchDto;
        }


        // PUT: api/Branch/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBranch(int id, BranchDto branchDto)
        {
            if (id != branchDto.BranchId)
            {
                return BadRequest(new {message= "The branch ID in the URL does not match the branch ID in the body." });
            }

            var branch = await _context.Branchs.FindAsync(id);
            if (branch == null)
            {
                return NotFound(new { message = "Branch not found." });
            }

            branch.Name = branchDto.BranchName;
            branch.Location = branchDto.BranchLocation;

            _context.Entry(branch).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BranchExists(id))
                {
                    return NotFound(new { message = "Branch not found." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        // POST: api/Branch
        [HttpPost]
        public async Task<ActionResult<BranchDto>> PostBranch(BranchDto branchDto)
        {
            var branch = new Branch
            {
                Name = branchDto.BranchName,
                Location = branchDto.BranchLocation
            };

            _context.Branchs.Add(branch);
            await _context.SaveChangesAsync();

            branchDto.BranchId = branch.BranchId;

            return CreatedAtAction("GetBranch", new { id = branch.BranchId }, branchDto);
        }


        // DELETE: api/Branch/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBranch(int id)
        {
            var branch = await _context.Branchs.FindAsync(id);
            if (branch == null)
            {
                return NotFound();
            }

            _context.Branchs.Remove(branch);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BranchExists(int id)
        {
            return _context.Branchs.Any(e => e.BranchId == id);
        }
    }
}
