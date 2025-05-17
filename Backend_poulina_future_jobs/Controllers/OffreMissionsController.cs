using Backend_poulina_future_jobs.Dtos;
using Backend_poulina_future_jobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace YourNamespace.Controllers
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
        // GET: api/OffreMissions?IdOffreEmploi=f3e85470-743c-44d9-b762-41ad3adbdf43
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OffreMission>>> GetOffreMissions([FromQuery] Guid IdOffreEmploi)
        {
            if (IdOffreEmploi == Guid.Empty)
            {
                return BadRequest(new { success = false, message = "L'ID de l'offre d'emploi est requis." });
            }

            // Verify that the OffreEmploi exists
            if (!await _context.OffresEmploi.AnyAsync(oe => oe.IdOffreEmploi == IdOffreEmploi))
            {
                return NotFound(new { success = false, message = $"L'offre d'emploi avec ID {IdOffreEmploi} n'existe pas." });
            }

            var offreMissions = await _context.OffreMissions
                .Where(om => om.IdOffreEmploi == IdOffreEmploi)
                .ToListAsync();

            return Ok(new { success = true, data = offreMissions });
        }
    

        [HttpPost]
        public async Task<ActionResult<object>> Create([FromBody] OffreMissionDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Données invalides",
                        errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }

                if (!await _context.OffresEmploi.AnyAsync(o => o.IdOffreEmploi == dto.IdOffreEmploi))
                {
                    return BadRequest(new { success = false, message = "L'offre spécifiée n'existe pas" });
                }

                var entity = new OffreMission
                {
                    IdOffreMission = Guid.NewGuid(),
                    IdOffreEmploi = dto.IdOffreEmploi,
                    DescriptionMission = dto.DescriptionMission,
                    Priorite = dto.Priorite
                };

                _context.OffreMissions.Add(entity);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = entity.IdOffreMission }, new
                {
                    success = true,
                    message = "Mission créée avec succès",
                    data = entity
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erreur interne du serveur",
                    detail = ex.Message
                });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<object>> Update(Guid id, [FromBody] OffreMissionDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Données invalides",
                        errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }

                var entity = await _context.OffreMissions.FindAsync(id);
                if (entity == null)
                {
                    return NotFound(new { success = false, message = "Mission non trouvée" });
                }

                if (!await _context.OffresEmploi.AnyAsync(o => o.IdOffreEmploi == dto.IdOffreEmploi))
                {
                    return BadRequest(new { success = false, message = "L'offre spécifiée n'existe pas" });
                }

                entity.IdOffreEmploi = dto.IdOffreEmploi;
                entity.DescriptionMission = dto.DescriptionMission;
                entity.Priorite = dto.Priorite;

                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Mission mise à jour avec succès",
                    data = entity
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erreur interne du serveur",
                    detail = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OffreMission>> GetById(Guid id)
        {
            var mission = await _context.OffreMissions.FindAsync(id);
            if (mission == null)
            {
                return NotFound();
            }
            return mission;
        }
        // Add this method to the existing OffreMissionsController

        // DELETE: api/OffreMissions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOffreMission(Guid id)
        {
            var offreMission = await _context.OffreMissions.FindAsync(id);
            if (offreMission == null)
            {
                return NotFound(new { success = false, message = $"La mission avec ID {id} n'existe pas." });
            }

            _context.OffreMissions.Remove(offreMission);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Mission supprimée avec succès." });
        }
        [HttpGet("by-offre/{idOffreEmploi}")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetByOffreId(Guid idOffreEmploi)
        {
            try
            {
                if (!await _context.OffresEmploi.AnyAsync(o => o.IdOffreEmploi == idOffreEmploi))
                {
                    return NotFound(new { success = false, message = $"L'offre d'emploi avec ID {idOffreEmploi} n'existe pas." });
                }

                var missions = await _context.OffreMissions
                    .Where(om => om.IdOffreEmploi == idOffreEmploi)
                    .ToListAsync();

                if (!missions.Any())
                {
                    return NotFound(new { success = false, message = "Aucune mission trouvée pour cette offre" });
                }

                return Ok(new
                {
                    success = true,
                    message = "Missions récupérées avec succès",
                    data = missions
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erreur lors de la récupération des missions",
                    detail = ex.Message
                });
            }
        }
    }

}