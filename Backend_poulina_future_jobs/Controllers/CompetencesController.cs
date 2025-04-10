using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_poulina_future_jobs.Models;
using Microsoft.AspNetCore.Authorization;
using Backend_poulina_future_jobs.Models.DTOs;

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
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetCompetences()
        {
            var competences = await _context.Competences.ToListAsync();
            return new
            {
                success = true,
                message = "Compétences récupérées avec succès",
                data = competences,
                count = competences.Count
            };
        }

        // GET: api/Competenc
        // es/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetCompetence(Guid id)
        {
            var competence = await _context.Competences.FindAsync(id);

            if (competence == null)
            {
                return NotFound(new { success = false, message = $"Compétence avec l'ID {id} non trouvée" });
            }

            return new
            {
                success = true,
                message = "Compétence récupérée avec succès",
                data = competence
            };
        }
        // PUT: api/Competences/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> PutCompetence(Guid id, CompetenceUpdateDTO competenceDto)
        {
            if (id != competenceDto.Id)
            {
                return BadRequest(new { success = false, message = "L'ID fourni ne correspond pas à l'ID de la compétence" });
            }

            var competence = await _context.Competences.FindAsync(id);
            if (competence == null)
            {
                return NotFound(new { success = false, message = $"Compétence avec l'ID {id} non trouvée" });
            }

            // Mettre à jour les propriétés de la compétence existante
            competence.Nom = competenceDto.Nom;
            competence.Description = competenceDto.Description;
            competence.estTechnique = competenceDto.EstTechnique;
            competence.estSoftSkill = competenceDto.EstSoftSkill;
            competence.DateModification = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Compétence mise à jour avec succès", data = competence });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompetenceExists(id))
                {
                    return NotFound(new { success = false, message = $"Compétence avec l'ID {id} non trouvée" });
                }
                else
                {
                    throw;
                }
            }
        }


        // POST: api/Competences
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<object>> PostCompetence(CompetenceCreateDto competenceDto)
        {
            // Créer une nouvelle instance de Competence à partir du DTO
            var competence = new Competence
            {
                Id = Guid.NewGuid(),
                Nom = competenceDto.Nom,
                Description = competenceDto.Description,
                estTechnique = competenceDto.EstTechnique,
                estSoftSkill = competenceDto.EstSoftSkill,
                dateAjout = DateTime.UtcNow,
                DateModification = DateTime.UtcNow
            };

            _context.Competences.Add(competence);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompetence",
                new { id = competence.Id },
                new { success = true, message = "Compétence créée avec succès", data = competence });
        }

        // DELETE: api/Competences/5
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteCompetence(Guid id)
        {
            var competence = await _context.Competences.FindAsync(id);
            if (competence == null)
            {
                return NotFound(new { success = false, message = $"Compétence avec l'ID {id} non trouvée" });
            }

            _context.Competences.Remove(competence);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Compétence supprimée avec succès" });
        }
       
            [HttpGet("search")]
        [AllowAnonymous]
            public async Task<IActionResult> SearchCompetences([FromQuery] string term)
            {
                if (string.IsNullOrWhiteSpace(term))
                {
                    return BadRequest(new { message = "Le terme de recherche est requis." });
                }

                var results = await _context.Competences
                    .Where(c => c.Nom.ToLower().Contains(term.ToLower()))
                    .Select(c => new
                    {
                        c.Id,
                        c.Nom,
                        c.Description,
                        c.estTechnique,
                        c.estSoftSkill
                    })
                    .ToListAsync();

                return Ok(results);
            }
        



        private bool CompetenceExists(Guid id)
        {
            return _context.Competences.Any(e => e.Id == id);
        }
    }
}
