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
    public class OffreLanguesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OffreLanguesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/OffreLangues
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OffreLangue>>> GetOffreLangues()
        {
            return await _context.OffreLangues.ToListAsync();
        }

        // GET: api/OffreLangues/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OffreLangue>> GetById(Guid id)
        {
            var langue = await _context.OffreLangues.FindAsync(id);
            if (langue == null)
            {
                return NotFound();
            }
            return langue;
        }

        // PUT: api/OffreLangues/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<object>> Create([FromBody] OffreLangueDto dto)
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

                var entity = new OffreLangue
                {
                    IdOffreLangue = Guid.NewGuid(),
                    IdOffreEmploi = dto.IdOffreEmploi,
                    Langue = dto.Langue,
                    NiveauRequis = dto.NiveauRequis
                };

                _context.OffreLangues.Add(entity);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = entity.IdOffreLangue }, new
                {
                    success = true,
                    message = "Langue ajoutée avec succès",
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
        public async Task<ActionResult<object>> Update(Guid id, [FromBody] OffreLangueDto dto)
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

                var entity = await _context.OffreLangues.FindAsync(id);
                if (entity == null)
                {
                    return NotFound(new { success = false, message = "Langue non trouvée" });
                }

                if (!await _context.OffresEmploi.AnyAsync(o => o.IdOffreEmploi == dto.IdOffreEmploi))
                {
                    return BadRequest(new { success = false, message = "L'offre spécifiée n'existe pas" });
                }

                entity.IdOffreEmploi = dto.IdOffreEmploi;
                entity.Langue = dto.Langue;
                entity.NiveauRequis = dto.NiveauRequis;

                _context.Entry(entity).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Langue mise à jour avec succès",
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

        // DELETE: api/OffreLangues/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOffreLangue(Guid id)
        {
            var offreLangue = await _context.OffreLangues.FindAsync(id);
            if (offreLangue == null)
            {
                return NotFound();
            }

            _context.OffreLangues.Remove(offreLangue);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("by-offre/{idOffreEmploi}")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetByOffreId(Guid idOffreEmploi)
        {
            try
            {
                var langues = await _context.OffreLangues
                    .Where(ol => ol.IdOffreEmploi == idOffreEmploi)
                    .ToListAsync();

                if (!langues.Any())
                {
                    return NotFound(new { success = false, message = "Aucune langue trouvée pour cette offre" });
                }

                return Ok(new
                {
                    success = true,
                    message = "Langues récupérées avec succès",
                    data = langues
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erreur lors de la récupération des langues",
                    detail = ex.Message
                });
            }
        }

        private bool OffreLangueExists(Guid id)
        {
            return _context.OffreLangues.Any(e => e.IdOffreLangue == id);
        }
    }
   
}
