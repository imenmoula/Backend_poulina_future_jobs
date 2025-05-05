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
    public class CompetencesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CompetencesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Competences
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Competence>>> GetCompetences()
        {
            return await _context.Competences.ToListAsync();
        }

        // GET: api/Competences/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Competence>> GetCompetence(Guid id)
        {
            var competence = await _context.Competences.FindAsync(id);

            if (competence == null)
            {
                return NotFound();
            }

            return competence;
        }

        // PUT: api/Competences/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompetence(Guid id, Competence competence)
        {
            if (id != competence.Id)
            {
                return BadRequest();
            }

            _context.Entry(competence).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompetenceExists(id))
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

        // POST: api/Competences
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Competence>> PostCompetence(Competence competence)
        {
            _context.Competences.Add(competence);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompetence", new { id = competence.Id }, competence);
        }

        // DELETE: api/Competences/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompetence(Guid id)
        {
            var competence = await _context.Competences.FindAsync(id);
            if (competence == null)
            {
                return NotFound();
            }

            _context.Competences.Remove(competence);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CompetenceExists(Guid id)
        {
            return _context.Competences.Any(e => e.Id == id);
        }
    }
}
