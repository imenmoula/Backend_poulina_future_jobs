using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_poulina_future_jobs.Models;
using Backend_poulina_future_jobs.Dtos;
using Microsoft.AspNetCore.Authorization;

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
            public async Task<ActionResult<object>> GetAll()
            {
                var competences = await _context.OffreCompetences
                    .Include(oc => oc.Competence)
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    message = "Liste des relations offre-compétence récupérée",
                    data = competences
                });
            }

        // GET: api/OffreCompetences/5
        [HttpGet("{idOffreEmploi}/{idCompetence}")]
        public async Task<ActionResult<OffreCompetences>> GetById(Guid idOffreEmploi, Guid idCompetence)
        {
            var competence = await _context.OffreCompetences
                .FirstOrDefaultAsync(oc => oc.IdOffreEmploi == idOffreEmploi && oc.IdCompetence == idCompetence);
            if (competence == null)
            {
                return NotFound();
            }
            return competence;
        }

        // POST: api/OffreCompetences
        [HttpPost]
        public async Task<ActionResult<object>> Create([FromBody] OffreCompetenceDto dto)
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

                if (!await _context.Competences.AnyAsync(c => c.Id == dto.IdCompetence))
                {
                    return BadRequest(new { success = false, message = "La compétence spécifiée n'existe pas" });
                }

                var entity = new OffreCompetences
                {
                    IdOffreEmploi = dto.IdOffreEmploi,
                    IdCompetence = dto.IdCompetence,
                    NiveauRequis = dto.NiveauRequis
                };

                _context.OffreCompetences.Add(entity);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { idOffreEmploi = entity.IdOffreEmploi, idCompetence = entity.IdCompetence }, new
                {
                    success = true,
                    message = "Compétence ajoutée avec succès",
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
        [HttpGet("by-offre/{idOffreEmploi}")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetByOffreId(Guid idOffreEmploi)
        {
            try
            {
                var competences = await _context.OffreCompetences
                    .Where(oc => oc.IdOffreEmploi == idOffreEmploi)
                    .Include(oc => oc.Competence)
                    .ToListAsync();

                if (!competences.Any())
                {
                    return NotFound(new { success = false, message = "Aucune compétence trouvée pour cette offre." });
                }

                return Ok(new
                {
                    success = true,
                    message = "Compétences récupérées avec succès",
                    data = competences
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erreur lors de la récupération des compétences",
                    detail = ex.Message
                });
            }
        }
        [HttpGet("competences-disponibles")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetCompetencesDisponibles()
        {
            try
            {
                var competences = await _context.Competences.ToListAsync();
                return Ok(new
                {
                    success = true,
                    message = "Compétences disponibles récupérées",
                    data = competences
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erreur lors de la récupération des compétences disponibles",
                    detail = ex.Message
                });
            }
        }

        [HttpPut("{idOffreEmploi}/{idCompetence}")]
        public async Task<ActionResult<object>> Update(Guid idOffreEmploi, Guid idCompetence, [FromBody] OffreCompetenceDto dto)
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

                if (idOffreEmploi != dto.IdOffreEmploi || idCompetence != dto.IdCompetence)
                {
                    return BadRequest(new { success = false, message = "Les IDs dans l'URL et le corps ne correspondent pas" });
                }

                var entity = await _context.OffreCompetences
                    .FirstOrDefaultAsync(oc => oc.IdOffreEmploi == idOffreEmploi && oc.IdCompetence == idCompetence);
                if (entity == null)
                {
                    return NotFound(new { success = false, message = "Compétence non trouvée" });
                }

                if (!await _context.OffresEmploi.AnyAsync(o => o.IdOffreEmploi == dto.IdOffreEmploi))
                {
                    return BadRequest(new { success = false, message = "L'offre spécifiée n'existe pas" });
                }

                if (!await _context.Competences.AnyAsync(c => c.Id == dto.IdCompetence))
                {
                    return BadRequest(new { success = false, message = "La compétence spécifiée n'existe pas" });
                }

                entity.NiveauRequis = dto.NiveauRequis;

                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Compétence mise à jour avec succès",
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
        // DELETE: api/OffreCompetences/5
        [HttpDelete("{idOffre}/{idCompetence}")]
            public async Task<IActionResult> Delete(Guid idOffre, Guid idCompetence)
            {
                try
                {
                    var entity = await _context.OffreCompetences
                        .FirstOrDefaultAsync(oc => oc.IdOffreEmploi == idOffre && oc.IdCompetence == idCompetence);

                    if (entity == null)
                    {
                        return NotFound(new
                        {
                            success = false,
                            message = "Relation non trouvée"
                        });
                    }

                    _context.OffreCompetences.Remove(entity);
                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        success = true,
                        message = "Relation supprimée"
                    });
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new
                    {
                        success = false,
                        message = "Erreur serveur",
                        detail = ex.Message
                    });
                }
            }
        }
    
}

