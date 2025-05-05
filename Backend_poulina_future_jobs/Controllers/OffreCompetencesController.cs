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
    public class OffreCompetencesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OffreCompetencesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/OffreCompetences
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OffreCompetences>>> GetOffreCompetences()
        {
            return await _context.OffreCompetences.ToListAsync();
        }

        // GET: api/OffreCompetences/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OffreCompetences>> GetOffreCompetences(Guid id)
        {
            var offreCompetences = await _context.OffreCompetences.FindAsync(id);

            if (offreCompetences == null)
            {
                return NotFound();
            }

            return offreCompetences;
        }

        // PUT: api/OffreCompetences/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOffreCompetences(Guid id, OffreCompetences offreCompetences)
        {
            if (id != offreCompetences.IdOffreEmploi)
            {
                return BadRequest();
            }

            _context.Entry(offreCompetences).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OffreCompetencesExists(id))
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

        // POST: api/OffreCompetences
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OffreCompetences>> PostOffreCompetences(OffreCompetences offreCompetences)
        {
            _context.OffreCompetences.Add(offreCompetences);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (OffreCompetencesExists(offreCompetences.IdOffreEmploi))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetOffreCompetences", new { id = offreCompetences.IdOffreEmploi }, offreCompetences);
        }

        // DELETE: api/OffreCompetences/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOffreCompetences(Guid id)
        {
            var offreCompetences = await _context.OffreCompetences.FindAsync(id);
            if (offreCompetences == null)
            {
                return NotFound();
            }

            _context.OffreCompetences.Remove(offreCompetences);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OffreCompetencesExists(Guid id)
        {
            return _context.OffreCompetences.Any(e => e.IdOffreEmploi == id);
        }
    }
}
