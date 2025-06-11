using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_poulina_future_jobs.Models;
using Microsoft.AspNetCore.Authorization;

namespace Backend_poulina_future_jobs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiplomesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DiplomesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Diplomes
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetDiplomes()
        {
            var diplomes = await _context.Diplomes.ToListAsync();
            return Ok(new { success = true, message = "Liste des diplômes récupérée", data = diplomes });
        }

        // GET: api/Diplomes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Diplome>> GetById(Guid id)
        {
            var diplome = await _context.Diplomes.FindAsync(id);
            if (diplome == null)
            {
                return NotFound();
            }
            return diplome;
        }

        // PUT: api/Diplomes/5
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> Update(Guid id, [FromBody] DiplomerequestDto dto)
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

                var entity = await _context.Diplomes.FindAsync(id);
                if (entity == null)
                {
                    return NotFound(new { success = false, message = "Diplôme non trouvé" });
                }

                entity.NomDiplome = dto.NomDiplome;
                entity.Niveau = dto.Niveau;
                entity.Domaine = dto.Domaine;
                entity.Institution = dto.Institution;

                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Diplôme mis à jour avec succès",
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

        // POST: api/Diplomes
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<object>> Create([FromBody] DiplomerequestDto dto)
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

                var entity = new Diplome
                {
                    IdDiplome = Guid.NewGuid(),
                    NomDiplome = dto.NomDiplome,
                    Niveau = dto.Niveau,
                    Domaine = dto.Domaine,
                    Institution = dto.Institution
                };

                _context.Diplomes.Add(entity);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = entity.IdDiplome }, new
                {
                    success = true,
                    message = "Diplôme créé avec succès",
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

        // DELETE: api/Diplomes/5
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteDiplome(Guid id)
        {
            var diplome = await _context.Diplomes.FindAsync(id);
            if (diplome == null)
            {
                return NotFound();
            }

            _context.Diplomes.Remove(diplome);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DiplomeExists(Guid id)
        {
            return _context.Diplomes.Any(e => e.IdDiplome == id);
        }

        //[HttpGet("by-offre/{idOffreEmploi}")]
        [HttpGet("by-offre/{idOffreEmploi}")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetByOffreId(Guid idOffreEmploi)
        {
            try
            {
                // Vérifier que l'offre existe
                var offre = await _context.OffresEmploi
                    .Include(o => o.DiplomesRequis)
                    .FirstOrDefaultAsync(o => o.IdOffreEmploi == idOffreEmploi);

                if (offre == null)
                {
                    return NotFound(new { success = false, message = $"L'offre d'emploi avec ID {idOffreEmploi} n'existe pas." });
                }

                // Récupérer les diplômes associés à l'offre via la navigation property
                var diplomes = offre.DiplomesRequis;

                if (diplomes == null || !diplomes.Any())
                {
                    return NotFound(new { success = false, message = "Aucun diplôme trouvé pour cette offre" });
                }

                return Ok(new
                {
                    success = true,
                    message = "Diplômes récupérés avec succès",
                    data = diplomes
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erreur lors de la récupération des diplômes",
                    detail = ex.Message
                });
            }
        }

        public class DiplomerequestDto
        {
            public string NomDiplome { get; set; }
            public string Niveau { get; set; }
            public string Domaine { get; set; }
            public string ?Institution { get; set; }
        }
    }
}