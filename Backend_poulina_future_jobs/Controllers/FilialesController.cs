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
    [AllowAnonymous]

    public class FilialesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public FilialesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Filiales
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetFiliales()
        {
            var filiales = await _context.Filiales.ToListAsync();
            if (!filiales.Any())
            {
                return Ok(new { message = "Aucune filiale n'a été trouvée." });
            }

            return Ok(new { data = filiales, message = "Liste des filiales récupérée avec succès." });
        }

        // GET: api/Filiales/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetFiliale(Guid id)
        {
            var filiale = await _context.Filiales.FindAsync(id);

            if (filiale == null)
            {
                return NotFound(new { data = filiale, message = "La filiale spécifiée n'existe pas." });
            }

            return Ok(new { data = filiale, message = "Détails de la filiale récupérés avec succès." });
        }

        // PUT: api/Filiales/5
        [HttpPut("{id}")]
        [AllowAnonymous]

        public async Task<IActionResult> PutFiliale(Guid id, Filiale filiale)
        {
            if (id != filiale.IdFiliale)
            {
                return BadRequest(new { message = "L'ID dans l'URL ne correspond pas à l'ID dans le corps de la requête." });
            }

            _context.Entry(filiale).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { message = "La filiale a été mise à jour avec succès." });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FilialeExists(id))
                {
                    return NotFound(new { message = "La filiale spécifiée n'existe pas." });
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/Filiales
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<Filiale>> PostFiliale(Filiale filiale)
        {
            filiale.IdFiliale = Guid.NewGuid();
            _context.Filiales.Add(filiale);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFiliale), new { id = filiale.IdFiliale },
                new { data = filiale, message = "Nouvelle filiale créée avec succès." });
        }

        // DELETE: api/Filiales/5
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteFiliale(Guid id)
        {
            var filiale = await _context.Filiales.FindAsync(id);
            Console.WriteLine($"ID reçu pour suppression: {id}");
            if (filiale == null)
            {
                return NotFound(new { message = "La filiale spécifiée n'existe pas." });
            }

            _context.Filiales.Remove(filiale);
            await _context.SaveChangesAsync();

            return Ok(new { message = "La filiale a été supprimée avec succès." });
        }

        private bool FilialeExists(Guid id)
        {
            return _context.Filiales.Any(e => e.IdFiliale == id);
        }


        [HttpPost("upload-photo")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadPhoto(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Aucun fichier sélectionné.");
            }

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Construire l'URL complète de l'image
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var fileUrl = $"{baseUrl}/uploads/{fileName}";

            return Ok(new
            {
                Message = "Téléchargement réussi!",
                Url = fileUrl
            });
        }

    }
}




