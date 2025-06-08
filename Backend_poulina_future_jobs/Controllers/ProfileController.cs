using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_poulina_future_jobs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;
using static Backend_poulina_future_jobs.Controllers.PostulerCandidateController;

namespace Backend_poulina_future_jobs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Candidate,Recruteur")] // Autoriser les deux rôles
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(AppDbContext context, UserManager<AppUser> userManager, ILogger<ProfileController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }
        // Ajouter ces méthodes dans ProfileController

        [HttpGet("diplomes")]
        [AllowAnonymous]

        public async Task<ActionResult<IEnumerable<CandidatureDiplomeDto>>> GetAllDiplomesByUser()
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var appUserId))
            {
                _logger.LogError("GetAllDiplomesByUser - Invalid user ID from token");
                return Unauthorized(new { error = "Identifiant utilisateur invalide." });
            }

            var diplomes = await _context.DiplomesCandidate
                .Where(d => d.AppUserId == appUserId)
                .ToListAsync();

            var diplomeDtos = diplomes.Select(d => new CandidatureDiplomeDto(
                d.IdDiplome,
                d.NomDiplome,
                d.Institution,
                d.DateObtention,
                d.Specialite,
                d.UrlDocument
            ));

            return Ok(diplomeDtos);
        }

        [HttpGet("experiences")]
        [AllowAnonymous]

        public async Task<ActionResult<IEnumerable<CandidatureExperienceDto>>> GetAllExperiencesByUser()
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var appUserId))
            {
                _logger.LogError("GetAllExperiencesByUser - Invalid user ID from token");
                return Unauthorized(new { error = "Identifiant utilisateur invalide." });
            }

            var experiences = await _context.Experiences
                .Where(e => e.AppUserId == appUserId)
                .ToListAsync();

            var experienceDtos = experiences.Select(e => new CandidatureExperienceDto(
                e.IdExperience,
                e.Poste,
                e.NomEntreprise,
                (DateTime)e.DateDebut,
                e.DateFin,
                e.Description,
                e.CompetenceAcquise
            ));

            return Ok(experienceDtos);
        }

        [HttpGet("certificats")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CandidatureCertificatDto>>> GetAllCertificatsByUser()
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var appUserId))
            {
                _logger.LogError("GetAllCertificatsByUser - Invalid user ID from token");
                return Unauthorized(new { error = "Identifiant utilisateur invalide." });
            }

            var certificats = await _context.Certificats
                .Where(c => c.AppUserId == appUserId)
                .ToListAsync();

            var certificatDtos = certificats.Select(c => new CandidatureCertificatDto(
                c.IdCertificat,
                c.Nom,
                c.Organisme,
                c.DateObtention,
                c.Description,
                c.UrlDocument
            ));

            return Ok(certificatDtos);
        }

        [HttpPost("diplomes")]
        public async Task<ActionResult<DiplomeResponse>> AddDiplome([FromBody] DiplomeInputDto dto)
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var appUserId))
            {
                _logger.LogError("AddDiplome - Invalid user ID from token");
                return Unauthorized(new { error = "Identifiant utilisateur invalide." });
            }

            var diplome = new DiplomeCandidate
            {
                IdDiplome = Guid.NewGuid(),
                AppUserId = appUserId,
                NomDiplome = dto.NomDiplome,
                Institution = dto.Institution,
                DateObtention = dto.DateObtention,
                Specialite = dto.Specialite,
                UrlDocument = dto.UrlDocument
            };

            _context.DiplomesCandidate.Add(diplome);
            await _context.SaveChangesAsync();

            return Ok(new DiplomeResponse
            {
                Message = "Diplôme ajouté avec succès.",
                Data = new CandidatureDiplomeDto(
                    diplome.IdDiplome,
                    diplome.NomDiplome,
                    diplome.Institution,
                    diplome.DateObtention,
                    diplome.Specialite,
                    diplome.UrlDocument
                )
            });
        }

        [HttpPut("diplomes/{id}")]
        public async Task<ActionResult<DiplomeResponse>> UpdateDiplome(Guid id, [FromBody] DiplomeInputDto dto)
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var appUserId))
            {
                _logger.LogError("UpdateDiplome - Invalid user ID from token");
                return Unauthorized(new { error = "Identifiant utilisateur invalide." });
            }

            var diplome = await _context.DiplomesCandidate.FindAsync(id);
            if (diplome == null)
            {
                _logger.LogWarning("UpdateDiplome - Diplome not found for ID: {Id}", id);
                return NotFound(new { error = "Diplôme non trouvé." });
            }

            if (diplome.AppUserId != appUserId)
            {
                _logger.LogWarning("UpdateDiplome - Unauthorized attempt for Diplome ID: {Id}", id);
                return Unauthorized(new { error = "Vous n'êtes pas autorisé à modifier ce diplôme." });
            }

            diplome.NomDiplome = dto.NomDiplome;
            diplome.Institution = dto.Institution;
            diplome.DateObtention = dto.DateObtention;
            diplome.Specialite = dto.Specialite;
            diplome.UrlDocument = dto.UrlDocument;

            await _context.SaveChangesAsync();

            return Ok(new DiplomeResponse
            {
                Message = "Diplôme mis à jour avec succès.",
                Data = new CandidatureDiplomeDto(
                    diplome.IdDiplome,
                    diplome.NomDiplome,
                    diplome.Institution,
                    diplome.DateObtention,
                    diplome.Specialite,
                    diplome.UrlDocument
                )
            });
        }

        [HttpDelete("diplomes/{id}")]
        public async Task<IActionResult> DeleteDiplome(Guid id)
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var appUserId))
                return Unauthorized(new { error = "Identifiant utilisateur invalide." });

            var diplome = await _context.DiplomesCandidate.FindAsync(id);
            if (diplome == null)
                return NotFound(new { error = "Diplôme non trouvé." });

            if (diplome.AppUserId != appUserId)
                return Unauthorized(new { error = "Vous n'êtes pas autorisé à supprimer ce diplôme." });

            _context.DiplomesCandidate.Remove(diplome);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Diplôme supprimé avec succès." });
        }


        [HttpPost("experiences")]
        public async Task<ActionResult<ExperienceResponse>> AddExperience([FromBody] ExperienceInputDto dto)
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var appUserId))
            {
                _logger.LogError("AddExperience - Invalid user ID from token");
                return Unauthorized(new { error = "Identifiant utilisateur invalide." });
            }

            var experience = new Experience
            {
                IdExperience = Guid.NewGuid(),
                AppUserId = appUserId,
                Poste = dto.Poste,
                NomEntreprise = dto.NomEntreprise,
                DateDebut = dto.DateDebut,
                DateFin = dto.DateFin,
                Description = dto.Description,
                CompetenceAcquise = dto.CompetenceAcquise
            };

            _context.Experiences.Add(experience);
            await _context.SaveChangesAsync();

            return Ok(new ExperienceResponse
            {
                Message = "Expérience ajoutée avec succès.",
                Data = new CandidatureExperienceDto(
                    experience.IdExperience,
                    experience.Poste,
                    experience.NomEntreprise,
                    (DateTime)experience.DateDebut,
                    experience.DateFin,
                    experience.Description,
                    experience.CompetenceAcquise
                )
            });
        }

        [HttpPut("experiences/{id}")]
        public async Task<ActionResult<ExperienceResponse>> UpdateExperience(Guid id, [FromBody] ExperienceInputDto dto)
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var appUserId))
            {
                _logger.LogError("UpdateExperience - Invalid user ID from token");
                return Unauthorized(new { error = "Identifiant utilisateur invalide." });
            }

            var experience = await _context.Experiences.FindAsync(id);
            if (experience == null)
            {
                _logger.LogWarning("UpdateExperience - Experience not found for ID: {Id}", id);
                return NotFound(new { error = "Expérience non trouvée." });
            }

            if (experience.AppUserId != appUserId)
            {
                _logger.LogWarning("UpdateExperience - Unauthorized attempt for Experience ID: {Id}", id);
                return Unauthorized(new { error = "Vous n'êtes pas autorisé à modifier cette expérience." });
            }

            experience.Poste = dto.Poste;
            experience.NomEntreprise = dto.NomEntreprise;
            experience.DateDebut = dto.DateDebut;
            experience.DateFin = dto.DateFin;
            experience.Description = dto.Description;
            experience.CompetenceAcquise = dto.CompetenceAcquise;

            await _context.SaveChangesAsync();

            return Ok(new ExperienceResponse
            {
                Message = "Expérience mise à jour avec succès.",
                Data = new CandidatureExperienceDto(
                    experience.IdExperience,
                    experience.Poste,
                    experience.NomEntreprise,
                    (DateTime)experience.DateDebut,
                    experience.DateFin,
                    experience.Description,
                    experience.CompetenceAcquise
                )
            });
        }

        [HttpDelete("experiences/{id}")]
        public async Task<IActionResult> DeleteExperience(Guid id)
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var appUserId))
                return Unauthorized(new { error = "Identifiant utilisateur invalide." });

            var experience = await _context.Experiences.FindAsync(id);
            if (experience == null)
                return NotFound(new { error = "Expérience non trouvée." });

            if (experience.AppUserId != appUserId)
                return Unauthorized(new { error = "Vous n'êtes pas autorisé à supprimer cette expérience." });

            _context.Experiences.Remove(experience);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Expérience supprimée avec succès." });
        }


        [HttpPost("certificats")]

        public async Task<ActionResult<CertificatResponse>> AddCertificat([FromBody] CertificatInputDto dto)
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var appUserId))
            {
                _logger.LogError("AddCertificat - Invalid user ID from token");
                return Unauthorized(new { error = "Identifiant utilisateur invalide." });
            }

            var certificat = new Certificat
            {
                IdCertificat = Guid.NewGuid(),
                AppUserId = appUserId,
                Nom = dto.Nom,
                Organisme = dto.Organisme,
                DateObtention = dto.DateObtention,
                Description = dto.Description,
                UrlDocument = dto.UrlDocument
            };

            _context.Certificats.Add(certificat);
            await _context.SaveChangesAsync();

            return Ok(new CertificatResponse
            {
                Message = "Certificat ajouté avec succès.",
                Data = new CandidatureCertificatDto(
                    certificat.IdCertificat,
                    certificat.Nom,
                    certificat.Organisme,
                    certificat.DateObtention,
                    certificat.Description,
                    certificat.UrlDocument
                )
            });
        }

        [HttpPut("certificats/{id}")]

        public async Task<ActionResult<CertificatResponse>> UpdateCertificat(Guid id, [FromBody] CertificatInputDto dto)
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var appUserId))
            {
                _logger.LogError("UpdateCertificat - Invalid user ID from token");
                return Unauthorized(new { error = "Identifiant utilisateur invalide." });
            }

            var certificat = await _context.Certificats.FindAsync(id);
            if (certificat == null)
            {
                _logger.LogWarning("UpdateCertificat - Certificat not found for ID: {Id}", id);
                return NotFound(new { error = "Certificat non trouvé." });
            }

            if (certificat.AppUserId != appUserId)
            {
                _logger.LogWarning("UpdateCertificat - Unauthorized attempt for Certificat ID: {Id}", id);
                return Unauthorized(new { error = "Vous n'êtes pas autorisé à modifier ce certificat." });
            }

            certificat.Nom = dto.Nom;
            certificat.Organisme = dto.Organisme;
            certificat.DateObtention = dto.DateObtention;
            certificat.Description = dto.Description;
            certificat.UrlDocument = dto.UrlDocument;

            await _context.SaveChangesAsync();

            return Ok(new CertificatResponse
            {
                Message = "Certificat mis à jour avec succès.",
                Data = new CandidatureCertificatDto(
                    certificat.IdCertificat,
                    certificat.Nom,
                    certificat.Organisme,
                    certificat.DateObtention,
                    certificat.Description,
                    certificat.UrlDocument
                )
            });
        }

        [HttpDelete("certificats/{id}")]
        public async Task<IActionResult> DeleteCertificat(Guid id)
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var appUserId))
                return Unauthorized(new { error = "Identifiant utilisateur invalide." });

            var certificat = await _context.Certificats.FindAsync(id);
            if (certificat == null)
                return NotFound(new { error = "Certificat non trouvé." });

            if (certificat.AppUserId != appUserId)
                return Unauthorized(new { error = "Vous n'êtes pas autorisé à supprimer ce certificat." });

            _context.Certificats.Remove(certificat);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Certificat supprimé avec succès." });
        }



        // Ajoutez ces méthodes dans le ProfileController

        [HttpGet("diplomes/{id}")]
        public async Task<ActionResult<DiplomeResponse>> GetDiplome(Guid id)
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var appUserId))
            {
                _logger.LogError("GetDiplome - Invalid user ID from token");
                return Unauthorized(new { error = "Identifiant utilisateur invalide." });
            }

            var diplome = await _context.DiplomesCandidate.FindAsync(id);
            if (diplome == null)
            {
                _logger.LogWarning("GetDiplome - Diplome not found for ID: {Id}", id);
                return NotFound(new { error = "Diplôme non trouvé." });
            }

            if (diplome.AppUserId != appUserId)
            {
                _logger.LogWarning("GetDiplome - Unauthorized attempt for Diplome ID: {Id}", id);
                return Unauthorized(new { error = "Accès non autorisé à ce diplôme." });
            }

            return Ok(new DiplomeResponse
            {
                Message = "Diplôme récupéré avec succès",
                Data = new CandidatureDiplomeDto(
                    diplome.IdDiplome,
                    diplome.NomDiplome,
                    diplome.Institution,
                    diplome.DateObtention,
                    diplome.Specialite,
                    diplome.UrlDocument
                )
            });
        }

        [HttpGet("experiences/{id}")]
        public async Task<ActionResult<ExperienceResponse>> GetExperience(Guid id)
        {

            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var appUserId))
            {
                _logger.LogError("GetExperience - Invalid user ID from token");
                return Unauthorized(new { error = "Identifiant utilisateur invalide." });
            }

            var experience = await _context.Experiences.FindAsync(id);
            if (experience == null)
            {
                _logger.LogWarning("GetExperience - Experience not found for ID: {Id}", id);
                return NotFound(new { error = "Expérience non trouvée." });
            }

            if (experience.AppUserId != appUserId)
            {
                _logger.LogWarning("GetExperience - Unauthorized attempt for Experience ID: {Id}", id);
                return Unauthorized(new { error = "Accès non autorisé à cette expérience." });
            }

            return Ok(new ExperienceResponse
            {
                Message = "Expérience récupérée avec succès",
                Data = new CandidatureExperienceDto(
                    experience.IdExperience,
                    experience.Poste,
                    experience.NomEntreprise,
                    (DateTime)experience.DateDebut,
                    experience.DateFin,
                    experience.Description,
                    experience.CompetenceAcquise
                )
            });
        }

        [HttpGet("certificats/{id}")]
        [Authorize(Roles = "Recruteur")]
        public async Task<ActionResult<CertificatResponse>> GetCertificat(Guid id)
        {
            var userId = User.FindFirst("userId")?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var appUserId))
            {
                _logger.LogError("GetCertificat - Invalid user ID from token");
                return Unauthorized(new { error = "Identifiant utilisateur invalide." });
            }

            var certificat = await _context.Certificats.FindAsync(id);
            if (certificat == null)
            {
                _logger.LogWarning("GetCertificat - Certificat not found for ID: {Id}", id);
                return NotFound(new { error = "Certificat non trouvé." });
            }

            if (certificat.AppUserId != appUserId)
            {
                _logger.LogWarning("GetCertificat - Unauthorized attempt for Certificat ID: {Id}", id);
                return Unauthorized(new { error = "Accès non autorisé à ce certificat." });
            }

            return Ok(new CertificatResponse
            {
                Message = "Certificat récupéré avec succès",
                Data = new CandidatureCertificatDto(
                    certificat.IdCertificat,
                    certificat.Nom,
                    certificat.Organisme,
                    certificat.DateObtention,
                    certificat.Description,
                    certificat.UrlDocument
                )
            });
        }

        // DTOs for Profile Updates
        public class DiplomeInputDto
        {
            [Required] public string NomDiplome { get; set; }
            [Required] public string Institution { get; set; }
            [Required] public DateTime DateObtention { get; set; }
            public string Specialite { get; set; }
            public string UrlDocument { get; set; }
        }

        public class ExperienceInputDto
        {
            [Required] public string Poste { get; set; }
            [Required] public string NomEntreprise { get; set; }
            [Required] public DateTime DateDebut { get; set; }
            public DateTime? DateFin { get; set; }
            public string Description { get; set; }
            public string CompetenceAcquise { get; set; }
        }

        public class CertificatInputDto
        {
            [Required] public string Nom { get; set; }
            [Required] public string Organisme { get; set; }
            [Required] public DateTime DateObtention { get; set; }
            public string Description { get; set; }
            public string UrlDocument { get; set; }
        }

        public class DiplomeResponse
        {
            public string Message { get; set; }
            public CandidatureDiplomeDto Data { get; set; }
        }

        public class ExperienceResponse
        {
            public string Message { get; set; }
            public CandidatureExperienceDto Data { get; set; }
        }

        public class CertificatResponse
        {
            public string Message { get; set; }
            public CandidatureCertificatDto Data { get; set; }
        }
    }
}