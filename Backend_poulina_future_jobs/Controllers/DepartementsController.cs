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
    [AllowAnonymous]

    public class DepartementsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DepartementsController(AppDbContext context)
        {
            _context = context;
        }

        //GET: api/Departements

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<object>>> GetDepartements()
        {
            var departements = await _context.Departements
                .Include(d => d.Filiale)
                .Select(d => new
                {
                    d.IdDepartement,
                    d.Nom,
                    d.Description,
                    d.DateCreation,
                    Filiale = new
                    {
                        d.Filiale.IdFiliale,
                        d.Filiale.Nom,
                        d.Filiale.Adresse
                    }
                })
                .ToListAsync();

            return Ok(departements);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]

        public async Task<ActionResult<Departement>> GetDepartement(Guid id)
        {
            var departement = await _context.Departements.FindAsync(id);
            if (departement == null)
            {
                return NotFound();
            }
            return departement;
        }




        //[HttpGet("{id}")]
        //[AllowAnonymous]

        //public async Task<IActionResult> GetDepartement(Guid id)
        //{
        //    var departement = await _context.Departements
        //        .Include(d => d.Filiale) // Inclure la relation avec la filiale
        //        .FirstOrDefaultAsync(d => d.IdDepartement == id);

        //    if (departement == null)
        //        return NotFound();

        //    return Ok(departement);
        //}


        //// PUT: api/Departements/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateDepartement(Guid id, [FromBody] UpdateDepartementDTO departementDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var departement = await _context.Departements.FindAsync(id);

            if (departement == null)
            {
                return NotFound(new { message = "Département non trouvé" });
            }

            // Mettre à jour les champs
            departement.Nom = departementDTO.Nom;
            departement.Description = departementDTO.Description;
            departement.IdFiliale = departementDTO.IdFiliale;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return StatusCode(500, new { message = "Erreur lors de la mise à jour du département" });
            }

            return Ok(new { message = "Département mis à jour avec succès", departement });
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateDepartement([FromBody] CreateDepartementDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Validation échouée",
                    errors = ModelState
                });
            }

            var filiale = await _context.Filiales.FindAsync(dto.IdFiliale);
            if (filiale == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Filiale introuvable"
                });
            }

            var departement = new Departement
            {
                IdDepartement = Guid.NewGuid(),
                Nom = dto.Nom,
                Description = dto.Description,
                IdFiliale = dto.IdFiliale,
                DateCreation = DateTime.UtcNow
            };

            _context.Departements.Add(departement);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDepartement), new { id = departement.IdDepartement },
                new
                {
                    success = true,
                    message = "Département créé avec succès",
                    data = departement
                });
        }





        // DELETE: api/Departements/5
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteDepartement(Guid id)
        {
            var departement = await _context.Departements.FindAsync(id);
            if (departement == null)
            {
                return NotFound(new { message = "Département non trouvé." });
            }

            _context.Departements.Remove(departement);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Département supprimé avec succès." });
        }

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Departement>>> SearchDepartement(string nom)
        {
            var departements = await _context.Departements
                .Where(d => d.Nom.Contains(nom))
                .ToListAsync();

            if (!departements.Any())
            {
                return NotFound(new { message = "Aucun département trouvé pour le nom fourni." });
            }

            return Ok(new { message = "Résultats de la recherche.", data = departements });
        }



        private bool DepartementExists(Guid id)
        {
            return _context.Departements.Any(e => e.IdDepartement == id);
        }
    }
}
