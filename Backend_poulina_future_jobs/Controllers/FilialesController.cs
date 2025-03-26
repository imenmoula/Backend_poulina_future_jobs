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

    public class FilialesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly string[] _allowedExtensions = { ".png", ".jfif", ".jpeg", ".jpg", ".gif", ".bmp" }; // Add more as needed
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
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Filiale>>> SearchFiliales([FromQuery] string nom)
        {
            if (string.IsNullOrWhiteSpace(nom))
            {
                return await _context.Filiales.ToListAsync(); // Return all filiales if search term is empty
            }

            var filiales = await _context.Filiales
                .Where(f => f.Nom.ToLower().Contains(nom.ToLower()))
                .ToListAsync();

            return filiales;
        }

        // POST: api/Filiales
        [HttpPost("post")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateFiliale([FromBody] CreateFilialeDto createFilialeDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Erreur de validation",
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            // Mapper CreateFilialeDto vers l'entité Filiale
            var filiale = new Filiale
            {
                IdFiliale = Guid.NewGuid(),
                Nom = createFilialeDto.Nom,
                Adresse = createFilialeDto.Adresse,
                Description = createFilialeDto.Description,
                DateCreation = DateTime.UtcNow,
                Photo = createFilialeDto.Photo,
                Phone = createFilialeDto.Phone,
                Fax = createFilialeDto.Fax,
                Email = createFilialeDto.Email,
                SiteWeb = createFilialeDto.SiteWeb
            };

            // Ajouter à la base de données et sauvegarder
            _context.Filiales.Add(filiale);
            await _context.SaveChangesAsync();

            // Retourner un message de succès avec les données de la filiale
            return CreatedAtAction(nameof(GetFiliale), new { id = filiale.IdFiliale }, new
            {
                success = true,
                message = "Filiale créée avec succès",
                filiale = new
                {
                    filiale.IdFiliale,
                    filiale.Nom,
                    filiale.Adresse,
                    filiale.Description,
                    filiale.DateCreation,
                    filiale.Photo,
                    filiale.Phone,
                    filiale.Fax,
                    filiale.Email,
                    filiale.SiteWeb
                }
            });
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


        //[HttpPost("upload-photo")]
        //[AllowAnonymous]
        //public async Task<IActionResult> UploadPhoto(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //    {
        //        return BadRequest("Aucun fichier sélectionné.");
        //    }

        //    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        //    if (!Directory.Exists(uploadPath))
        //    {
        //        Directory.CreateDirectory(uploadPath);
        //    }

        //    var fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        //    var filePath = Path.Combine(uploadPath, fileName);

        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);
        //    }

        //    // Construire l'URL complète de l'image
        //    var baseUrl = $"{Request.Scheme}://{Request.Host}";
        //    var fileUrl = $"{baseUrl}/uploads/{fileName}";

        //    return Ok(new
        //    {
        //        Message = "Téléchargement réussi!",
        //        Url = fileUrl
        //    });
        //}

    

        [HttpPost("upload-photo")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadPhoto(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest(new { message = "Aucun fichier sélectionné." });
                }

                var allowedExtensions = new[] { ".png", ".jfif", ".jpeg", ".jpg", ".gif", ".bmp" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest(new { message = $"Format de fichier non pris en charge. Formats autorisés : {string.Join(", ", allowedExtensions)}" });
                }

                const long maxFileSize = 5 * 1024 * 1024; // 5MB
                if (file.Length > maxFileSize)
                {
                    return BadRequest(new { message = "La taille du fichier dépasse la limite de 5 Mo." });
                }

                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                var fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                var fileUrl = $"{baseUrl}/uploads/{fileName}";

                return Ok(new
                {
                    message = "Téléchargement réussi!",
                    url = fileUrl
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue lors du téléchargement : " + ex.Message });
            }
        }
    }
}

        

        
    




