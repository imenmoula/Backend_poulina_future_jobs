using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_poulina_future_jobs.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace Backend_poulina_future_jobs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PostesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Postes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Poste>>> GetPostes()
        {
            return await _context.Postes.ToListAsync();
        }

        // GET: api/Postes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Poste>> GetPoste(Guid id)
        {
            var poste = await _context.Postes.FindAsync(id);

            if (poste == null)
            {
                return NotFound(new { success = false, message = $"Le poste avec ID {id} n'existe pas." });
            }

            return Ok(new { success = true, data = poste });
        }

        // PUT: api/Postes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPoste(Guid id, [FromBody] PosteUpdateDto posteDto)
        {
            if (id != posteDto.IdPoste)
            {
                return BadRequest(new { success = false, message = "L'ID dans l'URL ne correspond pas à l'ID du poste." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Les données fournies sont invalides.", errors = ModelState });
            }

            // Validate that the related OffreEmploi exists
            if (!await _context.OffresEmploi.AnyAsync(oe => oe.IdOffreEmploi == posteDto.IdOffreEmploi))
            {
                return BadRequest(new { success = false, message = $"L'offre d'emploi avec ID {posteDto.IdOffreEmploi} n'existe pas." });
            }

            var poste = await _context.Postes.FindAsync(id);
            if (poste == null)
            {
                return NotFound(new { success = false, message = $"Le poste avec ID {id} n'existe pas." });
            }

            // Update properties
            poste.TitrePoste = posteDto.TitrePoste;
            poste.Description = posteDto.Description;
            poste.NombrePostes = posteDto.NombrePostes;
            poste.ExperienceSouhaitee = posteDto.ExperienceSouhaitee;
            poste.NiveauHierarchique = posteDto.NiveauHierarchique;
            poste.IdOffreEmploi = posteDto.IdOffreEmploi;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PosteExists(id))
                {
                    return NotFound(new { success = false, message = $"Le poste avec ID {id} n'existe pas." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Postes
        [HttpPost]
        public async Task<ActionResult<Poste>> PostPoste([FromBody] PosteCreateDto posteDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Les données fournies sont invalides.", errors = ModelState });
            }

            // Validate that the related OffreEmploi exists
            if (!await _context.OffresEmploi.AnyAsync(oe => oe.IdOffreEmploi == posteDto.IdOffreEmploi))
            {
                return BadRequest(new { success = false, message = $"L'offre d'emploi avec ID {posteDto.IdOffreEmploi} n'existe pas." });
            }

            var poste = new Poste
            {
                IdPoste = Guid.NewGuid(),
                IdOffreEmploi = posteDto.IdOffreEmploi,
                TitrePoste = posteDto.TitrePoste,
                Description = posteDto.Description,
                NombrePostes = posteDto.NombrePostes,
                ExperienceSouhaitee = posteDto.ExperienceSouhaitee,
                NiveauHierarchique = posteDto.NiveauHierarchique
            };

            _context.Postes.Add(poste);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPoste", new { id = poste.IdPoste }, new { success = true, message = "Poste créé avec succès.", data = poste });
        }

        // DELETE: api/Postes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePoste(Guid id)
        {
            var poste = await _context.Postes.FindAsync(id);
            if (poste == null)
            {
                return NotFound(new { success = false, message = $"Le poste avec ID {id} n'existe pas." });
            }

            _context.Postes.Remove(poste);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Poste supprimé avec succès." });
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

                var postes = await _context.Postes
                    .Where(p => p.IdOffreEmploi == idOffreEmploi)
                    .ToListAsync();

                if (!postes.Any())
                {
                    return NotFound(new { success = false, message = "Aucun poste trouvé pour cette offre" });
                }

                return Ok(new
                {
                    success = true,
                    message = "Postes récupérés avec succès",
                    data = postes
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erreur lors de la récupération des postes",
                    detail = ex.Message
                });
            }
        }
        private bool PosteExists(Guid id)
        {
            return _context.Postes.Any(e => e.IdPoste == id);
        }
    }

    // DTO for creating a new Poste
    public class PosteCreateDto
    {
        [Required]
        public Guid IdOffreEmploi { get; set; }
        [Required]
        public string TitrePoste { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int NombrePostes { get; set; }
        [Required]
        public string ExperienceSouhaitee { get; set; }
        [Required]
        public string NiveauHierarchique { get; set; }
    }

    // DTO for updating an existing Poste
    public class PosteUpdateDto
    {
        public Guid IdPoste { get; set; }
        [Required]
        public Guid IdOffreEmploi { get; set; }
        [Required]
        public string TitrePoste { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int NombrePostes { get; set; }
        [Required]
        public string ExperienceSouhaitee { get; set; }
        [Required]
        public string NiveauHierarchique { get; set; }
    }
}