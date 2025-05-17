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
                return NotFound(new { success = false, message = $"La compétence avec ID {id} n'existe pas." });
            }

            return Ok(new { success = true, data = competence });
        }

        // PUT: api/Competences/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompetence(Guid id, Competence competence)
        {
            if (id != competence.Id)
            {
                return BadRequest(new { success = false, message = "L'ID dans l'URL ne correspond pas à l'ID de la compétence." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Les données fournies sont invalides.", errors = ModelState });
            }

            competence.DateModification = DateTime.UtcNow; // Update modification date
            _context.Entry(competence).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompetenceExists(id))
                {
                    return NotFound(new { success = false, message = $"La compétence avec ID {id} n'existe pas." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Competences
        [HttpPost]
        public async Task<ActionResult<Competence>> PostCompetence(Competence competence)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Les données fournies sont invalides.", errors = ModelState });
            }

            competence.Id = Guid.NewGuid(); // Ensure a new GUID if not provided
            competence.DateModification = DateTime.UtcNow; // Set initial modification date
            _context.Competences.Add(competence);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompetence", new { id = competence.Id }, new { success = true, message = "Compétence créée avec succès.", data = competence });
        }

        // DELETE: api/Competences/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompetence(Guid id)
        {
            var competence = await _context.Competences.FindAsync(id);
            if (competence == null)
            {
                return NotFound(new { success = false, message = $"La compétence avec ID {id} n'existe pas." });
            }

            _context.Competences.Remove(competence);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Compétence supprimée avec succès." });
        }

        private bool CompetenceExists(Guid id)
        {
            return _context.Competences.Any(e => e.Id == id);
        }
    }
}