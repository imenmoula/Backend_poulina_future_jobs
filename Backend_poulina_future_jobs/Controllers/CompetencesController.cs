using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<ActionResult<IEnumerable<CompetenceDTO>>> GetCompetences()
        {
            var competences = await _context.Competences.ToListAsync();
            var competencesDTO = competences.Select(c => new CompetenceDTO
            {
                Id = c.Id,
                Nom = c.Nom,
                Description = c.Description,
                DateModification = c.DateModification,
                HardSkills = c.HardSkills,
                SoftSkills = c.SoftSkills
            }).ToList();

            return Ok(competencesDTO);
        }

        // GET: api/Competences/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CompetenceDTO>> GetCompetence(Guid id)
        {
            var competence = await _context.Competences.FindAsync(id);
            if (competence == null)
            {
                return NotFound(new { message = "Compétence non trouvée." });
            }

            var competenceDTO = new CompetenceDTO
            {
                Id = competence.Id,
                Nom = competence.Nom,
                Description = competence.Description,
                DateModification = competence.DateModification,
                HardSkills = competence.HardSkills,
                SoftSkills = competence.SoftSkills
            };

            return Ok(competenceDTO);
        }

        // POST: api/Competences
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<CompetenceDTO>> CreateCompetence(CompetenceDTO competenceDTO)
        {
            // Valider HardSkills
            if (competenceDTO.HardSkills == null || !competenceDTO.HardSkills.All(hs => Enum.IsDefined(typeof(HardSkillType), hs)))
            {
                return BadRequest(new { message = "Une ou plusieurs valeurs de HardSkills sont invalides." });
            }

            // Valider SoftSkills
            if (competenceDTO.SoftSkills == null || !competenceDTO.SoftSkills.All(ss => Enum.IsDefined(typeof(SoftSkillType), ss)))
            {
                return BadRequest(new { message = "Une ou plusieurs valeurs de SoftSkills sont invalides." });
            }

            var competence = new Competence
            {
                Id = Guid.NewGuid(),
                Nom = competenceDTO.Nom,
                Description = competenceDTO.Description,
                DateModification = DateTime.UtcNow,
                HardSkills = competenceDTO.HardSkills,
                SoftSkills = competenceDTO.SoftSkills
            };

            _context.Competences.Add(competence);
            await _context.SaveChangesAsync();

            competenceDTO.Id = competence.Id;
            competenceDTO.DateModification = competence.DateModification;

            return CreatedAtAction(nameof(GetCompetence), new { id = competence.Id }, competenceDTO);
        }

        // PUT: api/Competences/{id}
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateCompetence(Guid id, CompetenceDTO competenceDTO)
        {
            var competence = await _context.Competences.FindAsync(id);
            if (competence == null)
            {
                return NotFound(new { message = "Compétence non trouvée." });
            }

            // Valider HardSkills
            if (competenceDTO.HardSkills == null || !competenceDTO.HardSkills.All(hs => Enum.IsDefined(typeof(HardSkillType), hs)))
            {
                return BadRequest(new { message = "Une ou plusieurs valeurs de HardSkills sont invalides." });
            }

            // Valider SoftSkills
            if (competenceDTO.SoftSkills == null || !competenceDTO.SoftSkills.All(ss => Enum.IsDefined(typeof(SoftSkillType), ss)))
            {
                return BadRequest(new { message = "Une ou plusieurs valeurs de SoftSkills sont invalides." });
            }

            competence.Nom = competenceDTO.Nom;
            competence.Description = competenceDTO.Description;
            competence.DateModification = DateTime.UtcNow;
            competence.HardSkills = competenceDTO.HardSkills;
            competence.SoftSkills = competenceDTO.SoftSkills;

            _context.Entry(competence).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Compétence mise à jour avec succès." });
        }

        // DELETE: api/Competences/{id}
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteCompetence(Guid id)
        {
            var competence = await _context.Competences
                .Include(c => c.OffreCompetences)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (competence == null)
            {
                return NotFound(new { message = "Compétence non trouvée." });
            }

            _context.OffreCompetences.RemoveRange(competence.OffreCompetences);
            _context.Competences.Remove(competence);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Compétence supprimée avec succès." });
        }
    }
}