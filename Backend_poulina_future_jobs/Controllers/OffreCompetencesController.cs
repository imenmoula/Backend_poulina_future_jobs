// Controllers/OffreCompetencesController.cs
using Backend_poulina_future_jobs.Models;
using Backend_poulina_future_jobs.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_poulina_future_jobs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OffreCompetencesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OffreCompetencesController(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: api/OffreCompetences/offre/{idOffreEmploi}
        [HttpGet("offre/{idOffreEmploi}")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetCompetencesByOffre(Guid idOffreEmploi)
        {
            try
            {
                var offerExists = await _context.OffresEmploi.AnyAsync(o => o.IdOffreEmploi == idOffreEmploi);
                if (!offerExists)
                {
                    return NotFound(new { message = "L'offre d'emploi spécifiée n'existe pas." });
                }

                var competences = await _context.OffreCompetences
                    .Include(oc => oc.Competence)
                    .Where(oc => oc.IdOffreEmploi == idOffreEmploi)
                    .Select(oc => new OffreCompetenceDTO
                    {
                        IdOffreEmploi = oc.IdOffreEmploi,
                        IdCompetence = oc.IdCompetence,
                        NiveauRequis = oc.NiveauRequis,
                        Competence = new CompetenceCreateDto
                        {
                            Id = oc.Competence.Id,
                            Nom = oc.Competence.Nom,
                            Description = oc.Competence.Description,
                            EstTechnique = oc.Competence.estTechnique,
                            EstSoftSkill = oc.Competence.estSoftSkill
                        }
                    })
                    .ToListAsync();

                return Ok(new
                {
                    message = "Compétences de l'offre récupérées avec succès.",
                    competences
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue.", detail = ex.Message });
            }
        }

        // POST: api/OffreCompetences
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<object>> AddCompetenceToOffre([FromBody] OffreCompetenceDTO dto)
        {
            try
            {
                if (dto == null || dto.Competence == null || string.IsNullOrWhiteSpace(dto.Competence.Nom))
                {
                    return BadRequest(new { message = "Données de compétence invalides." });
                }

                var offerExists = await _context.OffresEmploi.AnyAsync(o => o.IdOffreEmploi == dto.IdOffreEmploi);
                if (!offerExists)
                {
                    return NotFound(new { message = "L'offre d'emploi spécifiée n'existe pas." });
                }

                var competence = await _context.Competences
                    .FirstOrDefaultAsync(c => c.Nom.ToLower() == dto.Competence.Nom.ToLower());

                Guid competenceId;
                if (competence != null)
                {
                    competenceId = competence.Id;
                }
                else
                {
                    var newCompetence = new Competence
                    {
                        Id = Guid.NewGuid(),
                        Nom = dto.Competence.Nom,
                        Description = dto.Competence.Description,
                        estTechnique = dto.Competence.EstTechnique,
                        estSoftSkill = dto.Competence.EstSoftSkill,
                        dateAjout = DateTime.UtcNow,
                        DateModification = DateTime.UtcNow
                    };
                    _context.Competences.Add(newCompetence);
                    competenceId = newCompetence.Id;
                }

                var existingOffreCompetence = await _context.OffreCompetences
                    .FirstOrDefaultAsync(oc => oc.IdOffreEmploi == dto.IdOffreEmploi && oc.IdCompetence == competenceId);
                if (existingOffreCompetence != null)
                {
                    return Conflict(new { message = "Cette compétence est déjà associée à l'offre." });
                }

                var offreCompetence = new OffreCompetences
                {
                    IdOffreEmploi = dto.IdOffreEmploi,
                    IdCompetence = competenceId,
                    NiveauRequis = dto.NiveauRequis
                };

                _context.OffreCompetences.Add(offreCompetence);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetCompetencesByOffre), new { idOffreEmploi = dto.IdOffreEmploi }, new
                {
                    message = "Compétence ajoutée à l'offre avec succès.",
                    competence = new OffreCompetenceDTO
                    {
                        IdOffreEmploi = offreCompetence.IdOffreEmploi,
                        IdCompetence = offreCompetence.IdCompetence,
                        NiveauRequis = offreCompetence.NiveauRequis,
                        Competence = dto.Competence
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue.", detail = ex.Message });
            }
        }

        // PUT: api/OffreCompetences/{idOffreEmploi}/{idCompetence}
        [HttpPut("{idOffreEmploi}/{idCompetence}")]
[AllowAnonymous]        public async Task<ActionResult<object>> UpdateOffreCompetence(Guid idOffreEmploi, Guid idCompetence, [FromBody] OffreCompetenceDTO dto)
        {
            try
            {
                if (dto == null || dto.Competence == null || string.IsNullOrWhiteSpace(dto.Competence.Nom))
                {
                    return BadRequest(new { message = "Données de compétence invalides." });
                }

                var offreCompetence = await _context.OffreCompetences
                    .Include(oc => oc.Competence)
                    .FirstOrDefaultAsync(oc => oc.IdOffreEmploi == idOffreEmploi && oc.IdCompetence == idCompetence);

                if (offreCompetence == null)
                {
                    return NotFound(new { message = "Compétence associée non trouvée." });
                }

                var offerExists = await _context.OffresEmploi.AnyAsync(o => o.IdOffreEmploi == dto.IdOffreEmploi);
                if (!offerExists)
                {
                    return NotFound(new { message = "L'offre d'emploi spécifiée n'existe pas." });
                }

                var competence = await _context.Competences
                    .FirstOrDefaultAsync(c => c.Nom.ToLower() == dto.Competence.Nom.ToLower());

                Guid newCompetenceId;
                if (competence != null)
                {
                    newCompetenceId = competence.Id;
                }
                else
                {
                    var newCompetence = new Competence
                    {
                        Id = Guid.NewGuid(),
                        Nom = dto.Competence.Nom,
                        Description = dto.Competence.Description,
                        estTechnique = dto.Competence.EstTechnique,
                        estSoftSkill = dto.Competence.EstSoftSkill,
                        dateAjout = DateTime.UtcNow,
                        DateModification = DateTime.UtcNow
                    };
                    _context.Competences.Add(newCompetence);
                    newCompetenceId = newCompetence.Id;
                }

                // If the competence or offer changed, remove the old entry and create a new one
                if (offreCompetence.IdOffreEmploi != dto.IdOffreEmploi || offreCompetence.IdCompetence != newCompetenceId)
                {
                    _context.OffreCompetences.Remove(offreCompetence);
                    offreCompetence = new OffreCompetences
                    {
                        IdOffreEmploi = dto.IdOffreEmploi,
                        IdCompetence = newCompetenceId,
                        NiveauRequis = dto.NiveauRequis
                    };
                    _context.OffreCompetences.Add(offreCompetence);
                }
                else
                {
                    offreCompetence.NiveauRequis = dto.NiveauRequis;
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Compétence mise à jour avec succès.",
                    competence = new OffreCompetenceDTO
                    {
                        IdOffreEmploi = offreCompetence.IdOffreEmploi,
                        IdCompetence = offreCompetence.IdCompetence,
                        NiveauRequis = offreCompetence.NiveauRequis,
                        Competence = dto.Competence
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue.", detail = ex.Message });
            }
        }

        // DELETE: api/OffreCompetences/{idOffreEmploi}/{idCompetence}
        [HttpDelete("{idOffreEmploi}/{idCompetence}")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> DeleteOffreCompetence(Guid idOffreEmploi, Guid idCompetence)
        {
            try
            {
                var offreCompetence = await _context.OffreCompetences
                    .FirstOrDefaultAsync(oc => oc.IdOffreEmploi == idOffreEmploi && oc.IdCompetence == idCompetence);

                if (offreCompetence == null)
                {
                    return NotFound(new { message = "Compétence associée non trouvée." });
                }

                _context.OffreCompetences.Remove(offreCompetence);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Compétence supprimée de l'offre avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue.", detail = ex.Message });
            }
        }
    }
}

public class OffreCompetenceDTO
{
    public Guid IdOffreEmploi { get; set; }
    public Guid IdCompetence { get; set; }
    public NiveauRequisType NiveauRequis { get; set; }
    public CompetenceCreateDto Competence { get; set; }
}

public class CompetenceCreateDto
{
    public Guid Id { get; set; }
    public string Nom { get; set; }
    public string Description { get; set; }
    public bool EstTechnique { get; set; }
    public bool EstSoftSkill { get; set; }
}