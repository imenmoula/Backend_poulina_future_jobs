using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_poulina_future_jobs.Models;

namespace Backend_poulina_future_jobs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OffreMissionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OffreMissionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/OffreMissions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OffreMission>>> GetOffreMissions()
        {
            return await _context.OffreMissions.ToListAsync();
        }

        // GET: api/OffreMissions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OffreMission>> GetOffreMission(Guid id)
        {
            var offreMission = await _context.OffreMissions.FindAsync(id);

            if (offreMission == null)
            {
                return NotFound();
            }

            return offreMission;
        }

        // PUT: api/OffreMissions/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOffreMission(Guid id, OffreMission offreMission)
        {
            if (id != offreMission.IdOffreMission)
            {
                return BadRequest();
            }

            _context.Entry(offreMission).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OffreMissionExists(id))
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

        // POST: api/OffreMissions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OffreMission>> PostOffreMission(OffreMission offreMission)
        {
            _context.OffreMissions.Add(offreMission);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOffreMission", new { id = offreMission.IdOffreMission }, offreMission);
        }

        // DELETE: api/OffreMissions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOffreMission(Guid id)
        {
            var offreMission = await _context.OffreMissions.FindAsync(id);
            if (offreMission == null)
            {
                return NotFound();
            }

            _context.OffreMissions.Remove(offreMission);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OffreMissionExists(Guid id)
        {
            return _context.OffreMissions.Any(e => e.IdOffreMission == id);
        }
    }
}
