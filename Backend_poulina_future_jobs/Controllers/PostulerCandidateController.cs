using Backend_poulina_future_jobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend_poulina_future_jobs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostulerCandidateController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<PostulerCandidateController> _logger;

        public PostulerCandidateController(AppDbContext context, UserManager<AppUser> userManager, ILogger<PostulerCandidateController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// 1. Soumettre une candidature (non authentifié)
        /// </summary>
        [HttpPost("Soumettre")]
        [AllowAnonymous]
        public async Task<IActionResult> SoumettreCandidature([FromBody] CandidatureSoumissionDto model)
        {
            if (model == null)
            {
                return BadRequest(new { error = "Le corps de la requête est vide" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _context.Users.FindAsync(model.AppUserId);
                if (user == null)
                {
                    return NotFound(new { error = "Utilisateur non trouvé" });
                }

                var offre = await _context.OffresEmploi.FindAsync(model.OffreId);
                if (offre == null)
                {
                    return NotFound(new { error = "Offre non trouvée" });
                }

                if (await _context.Candidatures.AnyAsync(c => c.AppUserId == model.AppUserId && c.OffreId == model.OffreId))
                {
                    return BadRequest(new { error = "Vous avez déjà postulé pour cette offre." });
                }

                var candidature = new Candidature
                {
                    IdCandidature = Guid.NewGuid(),
                    AppUserId = model.AppUserId,
                    OffreId = model.OffreId,
                    Statut = "Soumise",
                    MessageMotivation = model.MessageMotivation,
                    DateSoumission = DateTime.UtcNow
                };

                _context.Candidatures.Add(candidature);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Candidature soumise", id = candidature.IdCandidature });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la soumission de candidature");
                return StatusCode(500, new { error = "Une erreur s'est produite lors de la soumission de la candidature." });
            }
        }

        /// <summary>
        /// 2. Créer une candidature complète (authentifié - Candidate)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult<CandidatureDto>> PostCandidature([FromBody] CandidatureInputDto input)
        {
            if (input == null)
            {
                return BadRequest(new { error = "Le corps de la requête est vide" });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for candidature submission: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            try
            {
                // Dans PostulerCandidateController.cs
                var userIdClaim = User.FindFirst("userId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { error = "Invalid or missing user ID in token.", userIdClaim });
                }


                if (input.AppUserId != userId)
                {
                    return Unauthorized(new { error = "Vous ne pouvez postuler que pour vous-même." });
                }

                // Vérification de l'offre
                var offre = await _context.OffresEmploi.FindAsync(input.OffreId);
                if (offre == null)
                {
                    return BadRequest(new { error = "L'offre n'existe pas." });
                }

                if (offre.Statut != StatutOffre.Ouvert)
                {
                    return BadRequest(new { error = "L'offre est clôturée." });
                }

                // Vérification de l'utilisateur
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return BadRequest(new { error = "L'utilisateur n'existe pas." });
                }

                // Vérification de candidature existante
                var existingCandidature = await _context.Candidatures
                    .AnyAsync(c => c.AppUserId == input.AppUserId && c.OffreId == input.OffreId);

                if (existingCandidature)
                {
                    return BadRequest(new { error = "Vous avez déjà postulé pour cette offre." });
                }

                // Mise à jour des informations utilisateur
                user.FullName = input.FullName;
                user.Nom = input.Nom;
                user.Prenom = input.Prenom;
                user.DateNaissance = input.DateNaissance;
                user.Adresse = input.Adresse;
                user.Ville = input.Ville;
                user.Pays = input.Pays;
                user.phone = input.Phone;

                user.cv = input.Cv;
                user.LettreMotivation = input.LettreMotivation;
                user.linkedIn = input.LinkedIn;
                user.github = input.Github;
                user.portfolio = input.Portfolio;
                user.Statut = input.Statut;

                await _userManager.UpdateAsync(user);

                // Ajout de diplômes candidats
                if (input.Diplomes != null)
                {
                    foreach (var d in input.Diplomes)
                    {
                        _context.DiplomesCandidate.Add(new DiplomeCandidate
                        {
                            IdDiplome = Guid.NewGuid(),
                            NomDiplome = d.NomDiplome,
                            Institution = d.Institution,
                            DateObtention = d.DateObtention,
                            Specialite = d.Specialite,
                            UrlDocument = d.UrlDocument,
                            AppUserId = user.Id
                        });
                    }
                }

                // Ajout expériences
                if (input.Experiences != null)
                {
                    foreach (var e in input.Experiences)
                    {
                        _context.Experiences.Add(new Experience
                        {
                            IdExperience = Guid.NewGuid(),
                            Poste = e.Poste,
                            Description = e.Description,
                            NomEntreprise = e.NomEntreprise,
                            DateDebut = e.DateDebut,
                            DateFin = e.DateFin,
                            AppUserId = input.AppUserId
                        });
                    }
                }

                // Ajout compétences
                if (input.Competences != null)
                {
                    foreach (var c in input.Competences)
                    {
                        _context.AppUserCompetences.Add(new AppUserCompetence
                        {
                            Id = Guid.NewGuid(),
                            CompetenceId = c.IdCompetence,
                            NiveauPossede = c.NiveauPossede,
                            AppUserId = user.Id,
                            OffreId = input.OffreId
                        });
                    }
                }

                // Ajout certificats
                if (input.Certificats != null)
                {
                    foreach (var cert in input.Certificats)
                    {
                        _context.Certificats.Add(new Certificat
                        {
                            IdCertificat = Guid.NewGuid(),
                            Nom = cert.Nom,
                            DateObtention = cert.DateObtention,
                            Organisme = cert.Organisme,
                            Description = cert.Description,
                            AppUserId = input.AppUserId
                        });
                    }
                }

                // Création de la candidature
                var candidature = new Candidature
                {
                    IdCandidature = Guid.NewGuid(),
                    AppUserId = input.AppUserId,
                    OffreId = input.OffreId,
                    Statut = "Soumise",
                    MessageMotivation = input.MessageMotivation,
                    DateSoumission = DateTime.UtcNow
                };

                _context.Candidatures.Add(candidature);
                await _context.SaveChangesAsync();

                var savedCandidature = await _context.Candidatures
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.Experiences)
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.AppUserCompetences).ThenInclude(cc => cc.Competence)
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.Certificats)
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.DiplomesCandidate)
                    .FirstOrDefaultAsync(c => c.IdCandidature == candidature.IdCandidature);

                var resultDto = MapToCandidatureDto(savedCandidature);
                return CreatedAtAction(nameof(GetCandidature), new { id = resultDto.IdCandidature }, resultDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création de candidature");
                return StatusCode(500, new { error = "Une erreur s'est produite lors de la création de la candidature." });
            }
        }

        /// <summary>
        /// 3. Modifier une candidature
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutCandidature(Guid id, [FromBody] CandidatureInputDto input)
        {
            if (input == null)
            {
                return BadRequest(new { error = "Le corps de la requête est vide" });
            }

            if (id != input.IdCandidature)
            {
                return BadRequest(new { error = "L'ID de la candidature ne correspond pas à l'ID dans l'URL" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var candidature = await _context.Candidatures
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.Experiences)
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.AppUserCompetences)
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.Certificats)
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.DiplomesCandidate)
                    .FirstOrDefaultAsync(c => c.IdCandidature == id);

                if (candidature == null)
                {
                    return NotFound(new { error = "Candidature non trouvée" });
                }

                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { error = "Invalid or missing user ID in token." });
                }

                bool isRecruteur = User.IsInRole("Recruteur");
                if (candidature.AppUserId != userId && !isRecruteur)
                {
                    return Unauthorized(new { error = "Vous n'êtes pas autorisé à modifier cette candidature" });
                }

                var user = candidature.AppUser;

                // Mise à jour utilisateur
                user.FullName = input.FullName;
                user.Nom = input.Nom;
                user.Prenom = input.Prenom;
                user.DateNaissance = input.DateNaissance;
                user.Adresse = input.Adresse;
                user.Ville = input.Ville;
                user.Pays = input.Pays;
                user.phone = input.Phone;

                user.cv = input.Cv;
                user.LettreMotivation = input.LettreMotivation;
                user.linkedIn = input.LinkedIn;
                user.github = input.Github;
                user.portfolio = input.Portfolio;
                user.Statut = input.Statut;

                // Nettoyage anciennes données
                if (user.DiplomesCandidate != null && user.DiplomesCandidate.Any())
                {
                    _context.DiplomesCandidate.RemoveRange(user.DiplomesCandidate);
                }

                if (user.Experiences != null && user.Experiences.Any())
                {
                    _context.Experiences.RemoveRange(user.Experiences);
                }

                if (user.AppUserCompetences != null && user.AppUserCompetences.Any())
                {
                    _context.AppUserCompetences.RemoveRange(user.AppUserCompetences);
                }

                if (user.Certificats != null && user.Certificats.Any())
                {
                    _context.Certificats.RemoveRange(user.Certificats);
                }

                // Ajout nouvelles données
                // Ajout de diplômes candidats
                if (input.Diplomes != null)
                {
                    foreach (var d in input.Diplomes)
                    {
                        _context.DiplomesCandidate.Add(new DiplomeCandidate
                        {
                            IdDiplome = Guid.NewGuid(),
                            NomDiplome = d.NomDiplome,
                            Institution = d.Institution,
                            DateObtention = d.DateObtention,
                            Specialite = d.Specialite,
                            UrlDocument = d.UrlDocument,
                            AppUserId = user.Id
                        });
                    }
                }

                if (input.Experiences != null)
                {
                    foreach (var e in input.Experiences)
                    {
                        _context.Experiences.Add(new Experience
                        {
                            IdExperience = Guid.NewGuid(),
                            Poste = e.Poste,
                            Description = e.Description,
                            NomEntreprise = e.NomEntreprise,
                            DateDebut = e.DateDebut,
                            DateFin = e.DateFin,
                            AppUserId = user.Id
                        });
                    }
                }

                if (input.Competences != null)
                {
                    foreach (var c in input.Competences)
                    {
                        _context.AppUserCompetences.Add(new AppUserCompetence
                        {
                            Id = Guid.NewGuid(),
                            CompetenceId = c.IdCompetence,
                            NiveauPossede = c.NiveauPossede,
                            AppUserId = user.Id,
                            OffreId = input.OffreId
                        });
                    }
                }

                if (input.Certificats != null)
                {
                    foreach (var cert in input.Certificats)
                    {
                        _context.Certificats.Add(new Certificat
                        {
                            IdCertificat = Guid.NewGuid(),
                            Nom = cert.Nom,
                            DateObtention = cert.DateObtention,
                            Organisme = cert.Organisme,
                            Description = cert.Description,
                            AppUserId = user.Id
                        });
                    }
                }

                candidature.MessageMotivation = input.MessageMotivation;
                if (isRecruteur && !string.IsNullOrEmpty(input.Statut) && input.Statut.Length <= 50)
                {
                    candidature.Statut = input.Statut;
                }

                try
                {
                    await _context.SaveChangesAsync();
                    return NoContent();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Database error while saving updates.");
                    return StatusCode(500, new { error = "Une erreur s'est produite lors de la mise à jour." });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour de candidature");
                return StatusCode(500, new { error = "Une erreur s'est produite lors de la mise à jour de la candidature." });
            }
        }

        /// <summary>
        /// 4. Récupérer une candidature
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<CandidatureDto>> GetCandidature(Guid id)
        {
            try
            {
                var candidature = await _context.Candidatures
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.Experiences)
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.AppUserCompetences).ThenInclude(cc => cc.Competence)
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.Certificats)
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.DiplomesCandidate)
                    .Include(c => c.Offre)
                    .FirstOrDefaultAsync(c => c.IdCandidature == id);

                if (candidature == null)
                {
                    return NotFound(new { error = "Candidature non trouvée" });
                }

                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { error = "Invalid or missing user ID in token." });
                }

                bool isRecruteur = User.IsInRole("Recruteur");
                if (candidature.AppUserId != userId && !isRecruteur)
                {
                    return Unauthorized(new { error = "Vous n'êtes pas autorisé à consulter cette candidature" });
                }

                return MapToCandidatureDto(candidature);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de candidature");
                return StatusCode(500, new { error = "Une erreur s'est produite lors de la récupération de la candidature." });
            }
        }

        /// <summary>
        /// 5. Supprimer une candidature
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var candidature = await _context.Candidatures.FindAsync(id);
                if (candidature == null)
                {
                    return NotFound(new { error = "Candidature non trouvée" });
                }

                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                {
                    return Unauthorized(new { error = "Invalid or missing user ID in token." });
                }

                bool isRecruteur = User.IsInRole("Recruteur");
                if (candidature.AppUserId != userId && !isRecruteur)
                {
                    return Unauthorized(new { error = "Vous n'êtes pas autorisé à supprimer cette candidature" });
                }

                _context.Candidatures.Remove(candidature);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de candidature");
                return StatusCode(500, new { error = "Une erreur s'est produite lors de la suppression de la candidature." });
            }
        }

        /// <summary>
        /// 6. Récupérer les candidats pour une offre
        /// </summary>
        [HttpGet("GetCandidatesForOffre/{offreId}")]
        [Authorize(Roles = "Recruteur")]
        public async Task<ActionResult<IEnumerable<CandidatureDto>>> GetCandidatesForOffre(Guid offreId)
        {
            try
            {
                var candidatures = await _context.Candidatures
                    .Include(c => c.AppUser)
                    .Include(c => c.Offre)
                    .Where(c => c.OffreId == offreId)
                    .ToListAsync();

                if (!candidatures.Any())
                {
                    return Ok(new List<CandidatureDto>());
                }

                var candidatureDtos = candidatures.Select(c => MapToCandidatureDto(c, false)).ToList();
                return Ok(candidatureDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des candidats pour une offre");
                return StatusCode(500, new { error = "Une erreur s'est produite lors de la récupération des candidats." });
            }
        }

        /// <summary>
        /// 7. Filtrer les candidats selon les compétences requises
        /// </summary>
        [HttpGet("GetFilteredCandidates/{offreId}")]
        [Authorize(Roles = "Recruteur")]
        public async Task<ActionResult<IEnumerable<AppUserDto>>> GetFilteredCandidatesByOffer(Guid offreId)
        {
            try
            {
                // Récupérer les compétences requises pour l'offre
                var requiredCompetences = await _context.OffreCompetences
                    .Where(oc => oc.IdOffreEmploi == offreId)
                    .ToListAsync();

                if (!requiredCompetences.Any())
                {
                    return BadRequest(new { error = "Aucune compétence requise pour cette offre" });
                }

                var requiredCompetenceIds = requiredCompetences.Select(rc => rc.IdCompetence).ToList();

                // Récupérer les candidats qui ont postulé à cette offre
                var candidatures = await _context.Candidatures
                    .Where(c => c.OffreId == offreId)
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.Experiences)
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.AppUserCompetences)
                            .ThenInclude(auc => auc.Competence)
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.Certificats)
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.DiplomesCandidate)
                    .ToListAsync();

                if (!candidatures.Any())
                {
                    return Ok(new List<AppUserDto>());
                }

                // Filtrer les candidats qui possèdent au moins 50% des compétences requises
                var filteredCandidates = candidatures
                    .Where(c =>
                    {
                        var userCompetences = c.AppUser.AppUserCompetences
                            .Select(auc => auc.CompetenceId)
                            .ToList();

                        double matchCount = requiredCompetenceIds.Intersect(userCompetences).Count();
                        double totalRequired = requiredCompetenceIds.Count;
                        double matchPercentage = totalRequired > 0 ? matchCount / totalRequired : 0;

                        return matchPercentage >= 0.5;
                    })
                    .Select(c => MapToAppUserDto(c.AppUser))
                    .ToList();

                return Ok(filteredCandidates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du filtrage des candidats");
                return StatusCode(500, new { error = "Une erreur s'est produite lors du filtrage des candidats." });
            }
        }

        /// <summary>
        /// 8. Mettre à jour le statut d'une candidature
        /// </summary>
        [HttpPut("UpdateCandidatureStatus/{id}")]
        [Authorize(Roles = "Recruteur")]
        public async Task<IActionResult> UpdateCandidatureStatus(Guid id, [FromBody] string statut)
        {
            if (string.IsNullOrEmpty(statut))
            {
                return BadRequest(new { error = "Le statut ne peut pas être vide" });
            }

            try
            {
                var candidature = await _context.Candidatures.FindAsync(id);
                if (candidature == null)
                {
                    return NotFound(new { error = "Candidature non trouvée" });
                }

                candidature.Statut = statut;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Statut mis à jour" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du statut");
                return StatusCode(500, new { error = "Une erreur s'est produite lors de la mise à jour du statut." });
            }
        }

        #region Mapping Methods

        private CandidatureDto MapToCandidatureDto(Candidature candidature, bool includeDetails = true)
        {
            if (candidature == null) return null;

            return new CandidatureDto
            {
                IdCandidature = candidature.IdCandidature,
                AppUserId = candidature.AppUserId,
                OffreId = candidature.OffreId,
                Statut = candidature.Statut,
                MessageMotivation = candidature.MessageMotivation,
                DateSoumission = candidature.DateSoumission,
                AppUser = includeDetails ? MapToAppUserDto(candidature.AppUser) : new AppUserDto
                {
                    Id = candidature.AppUser.Id,
                    FullName = candidature.AppUser.FullName,
                    Email = candidature.AppUser.Email
                },
                Offre = new OffreEmploicandidateDto
                {
                    IdOffreEmploi = candidature.Offre?.IdOffreEmploi ?? Guid.Empty
                }
            };
        }

        private AppUserDto MapToAppUserDto(AppUser u)
        {
            if (u == null) return null;

            return new AppUserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Nom = u.Nom,
                Prenom = u.Prenom,
                Email = u.Email,
                Phone = u.phone,

                LettreMotivation = u.LettreMotivation,
                Cv = u.cv,
                LinkedIn = u.linkedIn,
                Github = u.github,
                Portfolio = u.portfolio,
                Statut = u.Statut,

                Experiences = u.Experiences?.Select(e => new ExperienceDto
                {
                    IdExperience = e.IdExperience,
                    Poste = e.Poste,
                    Description = e.Description,
                    NomEntreprise = e.NomEntreprise,
                    DateDebut = (DateTime)e.DateDebut,
                    DateFin = e.DateFin
                }).ToList() ?? new List<ExperienceDto>(),

                AppUserCompetences = u.AppUserCompetences?.Select(ac => new CandidateCompetenceDto
                {
                    Id = ac.Id,
                    CompetenceId = ac.CompetenceId,
                    NiveauPossede = ac.NiveauPossede.ToString(),
                    Competence = new CompetenceCandidateDto
                    {
                        Id = ac.Competence?.Id ?? Guid.Empty,
                        Nom = ac.Competence?.Nom ?? string.Empty
                    }
                }).ToList() ?? new List<CandidateCompetenceDto>(),

                Certificats = u.Certificats?.Select(c => new CertificatDto
                {
                    IdCertificat = c.IdCertificat,
                    Nom = c.Nom,
                    DateObtention = c.DateObtention,
                    Organisme = c.Organisme,
                    Description = c.Description
                }).ToList() ?? new List<CertificatDto>(),

                DiplomesCandidate = u.DiplomesCandidate?.Select(d => new DiplomeCandidateDto
                {
                    IdDiplome = d.IdDiplome,
                    AppUserId = d.AppUserId,
                    NomDiplome = d.NomDiplome,
                    Institution = d.Institution,
                    DateObtention = d.DateObtention
                }).ToList() ?? new List<DiplomeCandidateDto>()
            };
        }

        #endregion

        #region DTOs

        /// <summary>
        /// DTO pour la soumission simple d'une candidature (sans authentification)
        /// </summary>
        public class CandidatureSoumissionDto
        {
            [Required]
            public Guid AppUserId { get; set; }

            [Required]
            public Guid OffreId { get; set; }

            [StringLength(500)]
            public string MessageMotivation { get; set; }
        }



        public class CandidatureInputDto
        {
            public Guid IdCandidature { get; set; }

            [Required]
            public Guid AppUserId { get; set; }

            [Required]
            public Guid OffreId { get; set; }

            [StringLength(500)]
            public string MessageMotivation { get; set; }

            // Informations utilisateur
            [Required, MaxLength(150)]
            public string FullName { get; set; }

            [Required, MaxLength(150)]
            public string Nom { get; set; }

            [Required, MaxLength(150)]
            public string Prenom { get; set; }

            public DateTime? DateNaissance { get; set; }

            [Required]
            public string Adresse { get; set; }

            [Required]
            public string Ville { get; set; }

            [Required]
            public string Pays { get; set; }

            [Required]
            public string Phone { get; set; }



            [Required]
            public string Cv { get; set; }

            [Required]
            public string LettreMotivation { get; set; }

            [MaxLength(255)]
            public string LinkedIn { get; set; }

            [MaxLength(255)]
            public string Github { get; set; }

            [MaxLength(255)]
            public string Portfolio { get; set; }

            [Required, MaxLength(20)]
            public string Statut { get; set; } = "Débutant";

            // Relations
            public List<DiplomeInputDto> Diplomes { get; set; } = new();
            public List<ExperienceInputDto> Experiences { get; set; } = new();
            public List<CompetenceInputDto> Competences { get; set; } = new();
            public List<CertificatInputDto> Certificats { get; set; } = new();
        }

        public class DiplomeInputDto
        {
            public string NomDiplome { get; set; }
            public string Institution { get; set; }
            public DateTime DateObtention { get; set; }
            public string Specialite { get; set; }
            public string UrlDocument { get; set; }
        }

        public class ExperienceInputDto
        {
            public string Poste { get; set; }
            public string Description { get; set; }
            public string NomEntreprise { get; set; }
            public DateTime DateDebut { get; set; }
            public DateTime? DateFin { get; set; }
        }

        public class CompetenceInputDto
        {
            public Guid IdCompetence { get; set; }
            public NiveauPossedeType NiveauPossede { get; set; }
        }

        public class CertificatInputDto
        {
            public string Nom { get; set; }
            public DateTime DateObtention { get; set; }
            public string Organisme { get; set; }
            public string Description { get; set; }
        }

        #endregion

        public class CandidatureDto
        {
            public Guid IdCandidature { get; set; }
            public Guid AppUserId { get; set; }
            public Guid OffreId { get; set; }
            public string Statut { get; set; }
            public string MessageMotivation { get; set; }
            public DateTime DateSoumission { get; set; }
            public AppUserDto AppUser { get; set; }
            public OffreEmploicandidateDto Offre { get; set; }
        }
        public class AppUserDto
        {
            public Guid Id { get; set; }
            public string FullName { get; set; }
            public string Nom { get; set; }
            public string Prenom { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string NiveauEtude { get; set; }
            public string Diplome { get; set; }
            public string Universite { get; set; }
            public string Specialite { get; set; }
            public string LettreMotivation { get; set; }
            public string Cv { get; set; }
            public string LinkedIn { get; set; }
            public string Github { get; set; }
            public string Portfolio { get; set; }
            public string Statut { get; set; }
            public List<ExperienceDto> Experiences { get; set; }
            public List<CandidateCompetenceDto> AppUserCompetences { get; set; }
            public List<CertificatDto> Certificats { get; set; }
            public List<DiplomeCandidateDto> DiplomesCandidate { get; set; }
        }
        public class ExperienceDto
        {
            public Guid IdExperience { get; set; }
            public string Poste { get; set; }
            public string Description { get; set; }
            public string NomEntreprise { get; set; }
            public DateTime DateDebut { get; set; }
            public DateTime? DateFin { get; set; }
        }
        public class CandidateCompetenceDto
        {
            public Guid Id { get; set; }
            public Guid CompetenceId { get; set; }
            public string NiveauPossede { get; set; }
            public CompetenceCandidateDto Competence { get; set; }
        }
        public class CompetenceCandidateDto
        {
            public Guid Id { get; set; }
            public string Nom { get; set; }
        }
        public class CertificatDto
        {
            public Guid IdCertificat { get; set; }
            public string Nom { get; set; }
            public DateTime DateObtention { get; set; }
            public string Organisme { get; set; }
            public string Description { get; set; }
        }
        public class DiplomeCandidateDto
        {
            public Guid IdDiplome { get; set; }
            public Guid AppUserId { get; set; }
            public string NomDiplome { get; set; }
            public string Institution { get; set; }
            public DateTime DateObtention { get; set; }
        }

        public class OffreEmploicandidateDto
        {
            public Guid IdOffreEmploi { get; set; }
        }


    }
}
/***********************************************************************************/

//using Backend_poulina_future_jobs.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.IO;
//using System.Linq;
//using System.Security.Claims;
//using System.Text.Json.Serialization;
//using System.Threading.Tasks;

//namespace Backend_poulina_future_jobs.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class PostulerCandidateController : ControllerBase
//    {
//        private readonly AppDbContext _context;
//        private readonly UserManager<AppUser> _userManager;
//        private readonly ILogger<PostulerCandidateController> _logger;
//        private readonly IWebHostEnvironment _environment;

//        public PostulerCandidateController(
//            AppDbContext context,
//            UserManager<AppUser> userManager,
//            ILogger<PostulerCandidateController> logger,
//            IWebHostEnvironment environment)
//        {
//            _context = context;
//            _userManager = userManager;
//            _logger = logger;
//            _environment = environment;
//        }

//        /// <summary>
//        /// 1. Soumettre une candidature simple (non authentifié)
//        /// </summary>
//        [HttpPost("Soumettre")]
//        [AllowAnonymous]
//        public async Task<IActionResult> SoumettreCandidature([FromBody] SoumissionCandidatureDto model)
//        {
//            _logger.LogInformation("Début de la soumission simple de candidature: {@Model}", model);

//            if (model == null)
//            {
//                _logger.LogWarning("Tentative de soumission avec un modèle null");
//                return BadRequest(new { error = "Le corps de la requête est vide" });
//            }

//            if (!ModelState.IsValid)
//            {
//                _logger.LogWarning("Validation du modèle échouée: {@ModelState}", ModelState);
//                return BadRequest(ModelState);
//            }

//            try
//            {
//                var user = await _context.Users.FindAsync(model.AppUserId);
//                if (user == null)
//                {
//                    _logger.LogWarning("Utilisateur non trouvé: {UserId}", model.AppUserId);
//                    return NotFound(new { error = "Utilisateur non trouvé" });
//                }

//                var offre = await _context.OffresEmploi.FindAsync(model.OffreId);
//                if (offre == null)
//                {
//                    _logger.LogWarning("Offre non trouvée: {OffreId}", model.OffreId);
//                    return NotFound(new { error = "Offre non trouvée" });
//                }

//                if (await _context.Candidatures.AnyAsync(c => c.AppUserId == model.AppUserId && c.OffreId == model.OffreId))
//                {
//                    _logger.LogWarning("Candidature déjà existante pour l'utilisateur {UserId} et l'offre {OffreId}", model.AppUserId, model.OffreId);
//                    return BadRequest(new { error = "Vous avez déjà postulé pour cette offre." });
//                }

//                var candidature = new Candidature
//                {
//                    IdCandidature = Guid.NewGuid(),
//                    AppUserId = model.AppUserId,
//                    OffreId = model.OffreId,
//                    Statut = Statuts.EnAttente,
//                    MessageMotivation = model.MessageMotivation,
//                    DateSoumission = DateTime.UtcNow
//                };

//                _context.Candidatures.Add(candidature);
//                await _context.SaveChangesAsync();

//                _logger.LogInformation("Candidature soumise avec succès: {CandidatureId}", candidature.IdCandidature);
//                return Ok(new { message = "Candidature soumise", id = candidature.IdCandidature });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erreur lors de la soumission de candidature");
//                return StatusCode(500, new { error = "Une erreur s'est produite lors de la soumission de la candidature." });
//            }
//        }

//        /// <summary>
//        /// 2. Créer une candidature complète avec fichiers (authentifié)
//        /// </summary>
//        [HttpPost]
//        [Authorize]
//        public async Task<ActionResult<CandidatureDto>> PostCandidature([FromForm] CandidatureFormDto input)
//        {
//            _logger.LogInformation("Début de la création complète de candidature avec fichiers: {@InputBasic}", new
//            {
//                input.IdCandidature,
//                input.AppUserId,
//                input.OffreId,
//                HasCvFile = input.CvFile != null,
//                HasLettreMotivationFile = input.LettreMotivationFile != null,
//                DiplomesCount = input.Diplomes?.Count ?? 0,
//                ExperiencesCount = input.Experiences?.Count ?? 0,
//                CompetencesCount = input.Competences?.Count ?? 0,
//                CertificatsCount = input.Certificats?.Count ?? 0
//            });

//            if (input == null)
//            {
//                _logger.LogWarning("Tentative de création avec un modèle null");
//                return BadRequest(new { error = "Le corps de la requête est vide" });
//            }

//            if (!ModelState.IsValid)
//            {
//                _logger.LogWarning("Validation du modèle échouée: {@ModelState}", ModelState);
//                return BadRequest(ModelState);
//            }

//            try
//            {
//                // Vérification de l'identité de l'utilisateur
//                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
//                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
//                {
//                    _logger.LogWarning("ID utilisateur invalide ou manquant dans le token: {UserIdClaim}", userIdClaim);
//                    return Unauthorized(new { error = "Invalid or missing user ID in token." });
//                }

//                if (input.AppUserId != userId)
//                {
//                    _logger.LogWarning("Tentative de postuler pour un autre utilisateur: {RequestedUserId} vs {AuthenticatedUserId}", input.AppUserId, userId);
//                    return Unauthorized(new { error = "Vous ne pouvez postuler que pour vous-même." });
//                }

//                // Vérification de l'offre
//                var offre = await _context.OffresEmploi.FindAsync(input.OffreId);
//                if (offre == null)
//                {
//                    _logger.LogWarning("Offre non trouvée: {OffreId}", input.OffreId);
//                    return BadRequest(new { error = "L'offre n'existe pas." });
//                }

//                // Vérification de l'utilisateur
//                var user = await _userManager.FindByIdAsync(userId.ToString());
//                if (user == null)
//                {
//                    _logger.LogWarning("Utilisateur non trouvé: {UserId}", userId);
//                    return BadRequest(new { error = "L'utilisateur n'existe pas." });
//                }

//                // Vérification de candidature existante
//                var existingCandidature = await _context.Candidatures
//                    .AnyAsync(c => c.AppUserId == input.AppUserId && c.OffreId == input.OffreId);

//                if (existingCandidature)
//                {
//                    _logger.LogWarning("Candidature déjà existante pour l'utilisateur {UserId} et l'offre {OffreId}", input.AppUserId, input.OffreId);
//                    return BadRequest(new { error = "Vous avez déjà postulé pour cette offre." });
//                }

//                // Traitement des fichiers
//                string cvUrl = null;
//                if (input.CvFile != null)
//                {
//                    _logger.LogInformation("Traitement du fichier CV: {FileName}, {Length} octets", input.CvFile.FileName, input.CvFile.Length);
//                    cvUrl = await ProcessUploadedFile(input.CvFile, userId, "cv");
//                    _logger.LogInformation("Fichier CV enregistré: {Url}", cvUrl);
//                }

//                string lettreMotivationUrl = null;
//                if (input.LettreMotivationFile != null)
//                {
//                    _logger.LogInformation("Traitement du fichier lettre de motivation: {FileName}, {Length} octets", input.LettreMotivationFile.FileName, input.LettreMotivationFile.Length);
//                    lettreMotivationUrl = await ProcessUploadedFile(input.LettreMotivationFile, userId, "lettres");
//                    _logger.LogInformation("Fichier lettre de motivation enregistré: {Url}", lettreMotivationUrl);
//                }

//                // Mise à jour des informations utilisateur
//                user.FullName = input.FullName;
//                user.Nom = input.Nom;
//                user.Prenom = input.Prenom;
//                user.DateNaissance = input.DateNaissance;
//                user.Adresse = input.Adresse;
//                user.Ville = input.Ville;
//                user.Pays = input.Pays;
//                user.phone = input.Phone;
//                user.linkedIn = input.LinkedIn;
//                user.github = input.GitHub;
//                user.portfolio = input.Portfolio;
//                user.Statut = input.Statut;

//                // Mise à jour des URLs des fichiers
//                if (cvUrl != null)
//                {
//                    user.cV = cvUrl;
//                }

//                if (lettreMotivationUrl != null)
//                {
//                    user.LettreMotivation = lettreMotivationUrl;
//                }

//                _logger.LogInformation("Mise à jour des informations utilisateur: {UserId}", userId);
//                await _userManager.UpdateAsync(user);

//                // Ajout de diplômes candidats
//                if (input.Diplomes != null && input.Diplomes.Any())
//                {
//                    _logger.LogInformation("Traitement de {Count} diplômes", input.Diplomes.Count);
//                    foreach (var d in input.Diplomes)
//                    {
//                        string diplomeUrl = d.UrlDocument;
//                        if (d.DiplomeFile != null)
//                        {
//                            _logger.LogInformation("Traitement du fichier diplôme: {FileName}, {Length} octets", d.DiplomeFile.FileName, d.DiplomeFile.Length);
//                            diplomeUrl = await ProcessUploadedFile(d.DiplomeFile, userId, "diplomes");
//                            _logger.LogInformation("Fichier diplôme enregistré: {Url}", diplomeUrl);
//                        }

//                        _context.DiplomesCandidate.Add(new DiplomeCandidate
//                        {
//                            IdDiplome = Guid.NewGuid(),
//                            NomDiplome = d.NomDiplome,
//                            Institution = d.Institution,
//                            DateObtention = (DateTime)d.DateObtention,
//                            Specialite = d.Specialite,
//                            UrlDocument = diplomeUrl,
//                            AppUserId = user.Id
//                        });
//                    }
//                }

//                // Ajout expériences
//                if (input.Experiences != null && input.Experiences.Any())
//                {
//                    _logger.LogInformation("Traitement de {Count} expériences", input.Experiences.Count);
//                    foreach (var e in input.Experiences)
//                    {
//                        _context.Experiences.Add(new Experience
//                        {
//                            IdExperience = Guid.NewGuid(),
//                            Poste = e.Poste,
//                            Description = e.Description,
//                            NomEntreprise = e.NomEntreprise,
//                            DateDebut = e.DateDebut,
//                            DateFin = e.DateFin,
//                            AppUserId = input.AppUserId
//                        });
//                    }
//                }

//                // Ajout compétences
//                if (input.Competences != null && input.Competences.Any())
//                {
//                    _logger.LogInformation("Traitement de {Count} compétences", input.Competences.Count);
//                    foreach (var c in input.Competences)
//                    {
//                        _context.AppUserCompetences.Add(new AppUserCompetence
//                        {
//                            Id = Guid.NewGuid(),
//                            CompetenceId = c.IdCompetence,
//                            NiveauPossede = c.NiveauPossede,
//                            AppUserId = user.Id,
//                            OffreId = c.EstLieeOffre ? input.OffreId : null
//                        });
//                    }
//                }

//                // Ajout certificats
//                if (input.Certificats != null && input.Certificats.Any())
//                {
//                    _logger.LogInformation("Traitement de {Count} certificats", input.Certificats.Count);
//                    foreach (var cert in input.Certificats)
//                    {
//                        string certificatUrl = cert.UrlDocument;
//                        if (cert.CertificatFile != null)
//                        {
//                            _logger.LogInformation("Traitement du fichier certificat: {FileName}, {Length} octets", cert.CertificatFile.FileName, cert.CertificatFile.Length);
//                            certificatUrl = await ProcessUploadedFile(cert.CertificatFile, userId, "certificats");
//                            _logger.LogInformation("Fichier certificat enregistré: {Url}", certificatUrl);
//                        }

//                        _context.Certificats.Add(new Certificat
//                        {
//                            IdCertificat = Guid.NewGuid(),
//                            Nom = cert.Nom,
//                            DateObtention = (DateTime)cert.DateObtention,
//                            Organisme = cert.Organisme,
//                            Description = cert.Description,
//                            UrlDocument = certificatUrl,
//                            AppUserId = input.AppUserId
//                        });
//                    }
//                }

//                // Création de la candidature
//                var candidature = new Candidature
//                {
//                    IdCandidature = Guid.NewGuid(),
//                    AppUserId = input.AppUserId,
//                    OffreId = input.OffreId,
//                    Statut = StatutCandidature.EnAttente,
//                    MessageMotivation = input.MessageMotivation,
//                    DateSoumission = DateTime.UtcNow,
//                    CV = cvUrl ?? user.CV,
//                    LettreMotivation = lettreMotivationUrl ?? user.LettreMotivation
//                };

//                _context.Candidatures.Add(candidature);
//                _logger.LogInformation("Sauvegarde des modifications en base de données");
//                await _context.SaveChangesAsync();

//                _logger.LogInformation("Candidature créée avec succès: {CandidatureId}", candidature.IdCandidature);
//                return Ok(new { message = "Candidature créée avec succès", candidatureId = candidature.IdCandidature });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erreur lors de la création de candidature");
//                return StatusCode(500, new { error = "Une erreur s'est produite lors de la création de la candidature." });
//            }
//        }

//        /// <summary>
//        /// 3. Modifier une candidature avec fichiers
//        /// </summary>
//        [HttpPut("{id}")]
//        [Authorize]
//        public async Task<IActionResult> PutCandidature(Guid id, [FromForm] CandidatureFormDto input)
//        {
//            _logger.LogInformation("Début de la modification de candidature {CandidatureId} avec fichiers", id);

//            if (input == null)
//            {
//                _logger.LogWarning("Tentative de modification avec un modèle null");
//                return BadRequest(new { error = "Le corps de la requête est vide" });
//            }

//            if (id != input.IdCandidature)
//            {
//                _logger.LogWarning("ID de candidature incohérent: {UrlId} vs {BodyId}", id, input.IdCandidature);
//                return BadRequest(new { error = "L'ID de la candidature ne correspond pas à l'ID dans l'URL" });
//            }

//            if (!ModelState.IsValid)
//            {
//                _logger.LogWarning("Validation du modèle échouée: {@ModelState}", ModelState);
//                return BadRequest(ModelState);
//            }

//            try
//            {
//                var candidature = await _context.Candidatures
//                    .Include(c => c.AppUser)
//                    .FirstOrDefaultAsync(c => c.IdCandidature == id);

//                if (candidature == null)
//                {
//                    _logger.LogWarning("Candidature non trouvée: {CandidatureId}", id);
//                    return NotFound(new { error = "Candidature non trouvée" });
//                }

//                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
//                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
//                {
//                    _logger.LogWarning("ID utilisateur invalide ou manquant dans le token: {UserIdClaim}", userIdClaim);
//                    return Unauthorized(new { error = "Invalid or missing user ID in token." });
//                }

//                bool isRecruteur = User.IsInRole("Recruteur");
//                if (candidature.AppUserId != userId && !isRecruteur)
//                {
//                    _logger.LogWarning("Tentative non autorisée de modification: {RequestedUserId} vs {AuthenticatedUserId}, IsRecruteur: {IsRecruteur}",
//                        candidature.AppUserId, userId, isRecruteur);
//                    return Unauthorized(new { error = "Vous n'êtes pas autorisé à modifier cette candidature" });
//                }

//                var user = candidature.AppUser;

//                // Traitement des fichiers
//                string cvUrl = candidature.CV;
//                if (input.CvFile != null)
//                {
//                    _logger.LogInformation("Traitement du fichier CV mis à jour: {FileName}, {Length} octets", input.CvFile.FileName, input.CvFile.Length);
//                    cvUrl = await ProcessUploadedFile(input.CvFile, userId, "cv");
//                    _logger.LogInformation("Fichier CV mis à jour enregistré: {Url}", cvUrl);
//                }

//                string lettreMotivationUrl = candidature.LettreMotivation;
//                if (input.LettreMotivationFile != null)
//                {
//                    _logger.LogInformation("Traitement du fichier lettre de motivation mis à jour: {FileName}, {Length} octets",
//                        input.LettreMotivationFile.FileName, input.LettreMotivationFile.Length);
//                    lettreMotivationUrl = await ProcessUploadedFile(input.LettreMotivationFile, userId, "lettres");
//                    _logger.LogInformation("Fichier lettre de motivation mis à jour enregistré: {Url}", lettreMotivationUrl);
//                }

//                // Mise à jour des informations utilisateur
//                user.FullName = input.FullName;
//                user.Nom = input.Nom;
//                user.Prenom = input.Prenom;
//                user.DateNaissance = input.DateNaissance;
//                user.Adresse = input.Adresse;
//                user.Ville = input.Ville;
//                user.Pays = input.Pays;
//                user.phone = input.Phone;
//                user.linkedIn = input.LinkedIn;
//                user.github = input.GitHub;
//                user.portfolio = input.Portfolio;
//                user.Statut = input.Statut;

//                // Mise à jour des URLs des fichiers
//                user.cV = cvUrl;
//                user.LettreMotivation = lettreMotivationUrl;

//                _logger.LogInformation("Mise à jour des informations utilisateur: {UserId}", userId);
//                await _userManager.UpdateAsync(user);

//                // Mise à jour de la candidature
//                candidature.MessageMotivation = input.MessageMotivation;
//                candidature.cV = cvUrl;
//                candidature.lettreMotivation = lettreMotivationUrl;

//                // Nettoyage et mise à jour des relations
//                _logger.LogInformation("Suppression des anciennes relations pour l'utilisateur {UserId}", userId);

//                // Diplômes
//                var existingDiplomes = await _context.DiplomesCandidate
//                    .Where(d => d.AppUserId == userId)
//                    .ToListAsync();

//                if (existingDiplomes.Any())
//                {
//                    _logger.LogInformation("Suppression de {Count} diplômes existants", existingDiplomes.Count);
//                    _context.DiplomesCandidate.RemoveRange(existingDiplomes);
//                }

//                // Expériences
//                var existingExperiences = await _context.Experiences
//                    .Where(e => e.AppUserId == userId)
//                    .ToListAsync();

//                if (existingExperiences.Any())
//                {
//                    _logger.LogInformation("Suppression de {Count} expériences existantes", existingExperiences.Count);
//                    _context.Experiences.RemoveRange(existingExperiences);
//                }

//                // Compétences
//                var existingCompetences = await _context.AppUserCompetences
//                    .Where(c => c.AppUserId == userId)
//                    .ToListAsync();

//                if (existingCompetences.Any())
//                {
//                    _logger.LogInformation("Suppression de {Count} compétences existantes", existingCompetences.Count);
//                    _context.AppUserCompetences.RemoveRange(existingCompetences);
//                }

//                // Certificats
//                var existingCertificats = await _context.Certificats
//                    .Where(c => c.AppUserId == userId)
//                    .ToListAsync();

//                if (existingCertificats.Any())
//                {
//                    _logger.LogInformation("Suppression de {Count} certificats existants", existingCertificats.Count);
//                    _context.Certificats.RemoveRange(existingCertificats);
//                }

//                // Ajout des nouvelles relations
//                // Diplômes
//                if (input.Diplomes != null && input.Diplomes.Any())
//                {
//                    _logger.LogInformation("Ajout de {Count} nouveaux diplômes", input.Diplomes.Count);
//                    foreach (var d in input.Diplomes)
//                    {
//                        string diplomeUrl = d.UrlDocument;
//                        if (d.DiplomeFile != null)
//                        {
//                            _logger.LogInformation("Traitement du fichier diplôme: {FileName}, {Length} octets", d.DiplomeFile.FileName, d.DiplomeFile.Length);
//                            diplomeUrl = await ProcessUploadedFile(d.DiplomeFile, userId, "diplomes");
//                            _logger.LogInformation("Fichier diplôme enregistré: {Url}", diplomeUrl);
//                        }

//                        _context.DiplomesCandidate.Add(new DiplomeCandidate
//                        {
//                            IdDiplome = Guid.NewGuid(),
//                            NomDiplome = d.NomDiplome,
//                            Institution = d.Institution,
//                            DateObtention = (DateTime)d.DateObtention,
//                            Specialite = d.Specialite,
//                            UrlDocument = diplomeUrl,
//                            AppUserId = userId
//                        });
//                    }
//                }

//                // Expériences
//                if (input.Experiences != null && input.Experiences.Any())
//                {
//                    _logger.LogInformation("Ajout de {Count} nouvelles expériences", input.Experiences.Count);
//                    foreach (var e in input.Experiences)
//                    {
//                        _context.Experiences.Add(new Experience
//                        {
//                            IdExperience = Guid.NewGuid(),
//                            Poste = e.Poste,
//                            Description = e.Description,
//                            NomEntreprise = e.NomEntreprise,
//                            DateDebut = e.DateDebut,
//                            DateFin = e.DateFin,
//                            AppUserId = userId
//                        });
//                    }
//                }

//                // Compétences
//                if (input.Competences != null && input.Competences.Any())
//                {
//                    _logger.LogInformation("Ajout de {Count} nouvelles compétences", input.Competences.Count);
//                    foreach (var c in input.Competences)
//                    {
//                        _context.AppUserCompetences.Add(new AppUserCompetence
//                        {
//                            Id = Guid.NewGuid(),
//                            CompetenceId = c.IdCompetence,
//                            NiveauPossede = c.NiveauPossede,
//                            AppUserId = userId,
//                            OffreId = c.EstLieeOffre ? candidature.OffreId : null
//                        });
//                    }
//                }

//                // Certificats
//                if (input.Certificats != null && input.Certificats.Any())
//                {
//                    _logger.LogInformation("Ajout de {Count} nouveaux certificats", input.Certificats.Count);
//                    foreach (var cert in input.Certificats)
//                    {
//                        string certificatUrl = cert.UrlDocument;
//                        if (cert.CertificatFile != null)
//                        {
//                            _logger.LogInformation("Traitement du fichier certificat: {FileName}, {Length} octets", cert.CertificatFile.FileName, cert.CertificatFile.Length);
//                            certificatUrl = await ProcessUploadedFile(cert.CertificatFile, userId, "certificats");
//                            _logger.LogInformation("Fichier certificat enregistré: {Url}", certificatUrl);
//                        }

//                        _context.Certificats.Add(new Certificat
//                        {
//                            IdCertificat = Guid.NewGuid(),
//                            Nom = cert.Nom,
//                            DateObtention = (DateTime)cert.DateObtention,
//                            Organisme = cert.Organisme,
//                            Description = cert.Description,
//                            UrlDocument = certificatUrl,
//                            AppUserId = userId
//                        });
//                    }
//                }

//                _logger.LogInformation("Sauvegarde des modifications en base de données");
//                await _context.SaveChangesAsync();

//                _logger.LogInformation("Candidature mise à jour avec succès: {CandidatureId}", id);
//                return Ok(new { message = "Candidature mise à jour avec succès" });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erreur lors de la modification de candidature {CandidatureId}", id);
//                return StatusCode(500, new { error = "Une erreur s'est produite lors de la modification de la candidature." });
//            }
//        }

//        /// <summary>
//        /// 4. Récupérer une candidature
//        /// </summary>
//        [HttpGet("{id}")]
//        [Authorize]
//        public async Task<ActionResult<CandidatureDto>> GetCandidature(Guid id)
//        {
//            _logger.LogInformation("Récupération de la candidature: {CandidatureId}", id);

//            try
//            {
//                var candidature = await _context.Candidatures
//                    .Include(c => c.AppUser)
//                    .Include(c => c.OffreEmploi)
//                    .Include(c => c.AppUser.Experiences)
//                    .Include(c => c.AppUser.AppUserCompetences)
//                        .ThenInclude(uc => uc.Competence)
//                    .Include(c => c.AppUser.Certificats)
//                    .Include(c => c.AppUser.DiplomesCandidate)
//                    .FirstOrDefaultAsync(c => c.IdCandidature == id);

//                if (candidature == null)
//                {
//                    _logger.LogWarning("Candidature non trouvée: {CandidatureId}", id);
//                    return NotFound(new { error = "Candidature non trouvée" });
//                }

//                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
//                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
//                {
//                    _logger.LogWarning("ID utilisateur invalide ou manquant dans le token: {UserIdClaim}", userIdClaim);
//                    return Unauthorized(new { error = "Invalid or missing user ID in token." });
//                }

//                bool isRecruteur = User.IsInRole("Recruteur");
//                if (candidature.AppUserId != userId && !isRecruteur)
//                {
//                    _logger.LogWarning("Tentative non autorisée d'accès: {RequestedUserId} vs {AuthenticatedUserId}, IsRecruteur: {IsRecruteur}",
//                        candidature.AppUserId, userId, isRecruteur);
//                    return Unauthorized(new { error = "Vous n'êtes pas autorisé à accéder à cette candidature" });
//                }

//                var result = MapToCandidatureDto(candidature);
//                _logger.LogInformation("Candidature récupérée avec succès: {CandidatureId}", id);
//                return Ok(result);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erreur lors de la récupération de candidature {CandidatureId}", id);
//                return StatusCode(500, new { error = "Une erreur s'est produite lors de la récupération de la candidature." });
//            }
//        }

//        /// <summary>
//        /// 5. Supprimer une candidature
//        /// </summary>
//        [HttpDelete("{id}")]
//        [Authorize]
//        public async Task<IActionResult> DeleteCandidature(Guid id)
//        {
//            _logger.LogInformation("Suppression de la candidature: {CandidatureId}", id);

//            try
//            {
//                var candidature = await _context.Candidatures.FindAsync(id);
//                if (candidature == null)
//                {
//                    _logger.LogWarning("Candidature non trouvée: {CandidatureId}", id);
//                    return NotFound(new { error = "Candidature non trouvée" });
//                }

//                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
//                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
//                {
//                    _logger.LogWarning("ID utilisateur invalide ou manquant dans le token: {UserIdClaim}", userIdClaim);
//                    return Unauthorized(new { error = "Invalid or missing user ID in token." });
//                }

//                bool isRecruteur = User.IsInRole("Recruteur");
//                if (candidature.AppUserId != userId && !isRecruteur)
//                {
//                    _logger.LogWarning("Tentative non autorisée de suppression: {RequestedUserId} vs {AuthenticatedUserId}, IsRecruteur: {IsRecruteur}",
//                        candidature.AppUserId, userId, isRecruteur);
//                    return Unauthorized(new { error = "Vous n'êtes pas autorisé à supprimer cette candidature" });
//                }

//                _context.Candidatures.Remove(candidature);
//                await _context.SaveChangesAsync();

//                _logger.LogInformation("Candidature supprimée avec succès: {CandidatureId}", id);
//                return Ok(new { message = "Candidature supprimée avec succès" });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erreur lors de la suppression de candidature {CandidatureId}", id);
//                return StatusCode(500, new { error = "Une erreur s'est produite lors de la suppression de la candidature." });
//            }
//        }

//        /// <summary>
//        /// 6. Récupérer les candidatures d'un utilisateur
//        /// </summary>
//        [HttpGet("User/{userId}")]
//        [Authorize]
//        public async Task<ActionResult<IEnumerable<CandidatureDto>>> GetCandidaturesByUser(Guid userId)
//        {
//            _logger.LogInformation("Récupération des candidatures de l'utilisateur: {UserId}", userId);

//            try
//            {
//                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
//                if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var authenticatedUserId))
//                {
//                    _logger.LogWarning("ID utilisateur invalide ou manquant dans le token: {UserIdClaim}", userIdClaim);
//                    return Unauthorized(new { error = "Invalid or missing user ID in token." });
//                }

//                bool isRecruteur = User.IsInRole("Recruteur");
//                if (userId != authenticatedUserId && !isRecruteur)
//                {
//                    _logger.LogWarning("Tentative non autorisée d'accès aux candidatures: {RequestedUserId} vs {AuthenticatedUserId}, IsRecruteur: {IsRecruteur}",
//                        userId, authenticatedUserId, isRecruteur);
//                    return Unauthorized(new { error = "Vous n'êtes pas autorisé à accéder aux candidatures de cet utilisateur" });
//                }

//                var candidatures = await _context.Candidatures
//                    .Include(c => c.AppUser)
//                    .Include(c => c.OffreEmploi)
//                    .Where(c => c.AppUserId == userId)
//                    .ToListAsync();

//                _logger.LogInformation("{Count} candidatures trouvées pour l'utilisateur {UserId}", candidatures.Count, userId);
//                return Ok(candidatures.Select(c => MapToCandidatureDto(c)));
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erreur lors de la récupération des candidatures de l'utilisateur {UserId}", userId);
//                return StatusCode(500, new { error = "Une erreur s'est produite lors de la récupération des candidatures." });
//            }
//        }

//        /// <summary>
//        /// 7. Récupérer les candidatures pour une offre
//        /// </summary>
//        [HttpGet("Offre/{offreId}")]
//        [Authorize(Roles = "Admin,Recruteur")]
//        public async Task<ActionResult<IEnumerable<CandidatureDto>>> GetCandidaturesByOffre(Guid offreId)
//        {
//            _logger.LogInformation("Récupération des candidatures pour l'offre: {OffreId}", offreId);

//            try
//            {
//                var candidatures = await _context.Candidatures
//                    .Include(c => c.AppUser)
//                    .Include(c => c.OffreEmploi)
//                    .Where(c => c.OffreId == offreId)
//                    .ToListAsync();

//                _logger.LogInformation("{Count} candidatures trouvées pour l'offre {OffreId}", candidatures.Count, offreId);
//                return Ok(candidatures.Select(c => MapToCandidatureDto(c)));
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erreur lors de la récupération des candidatures pour l'offre {OffreId}", offreId);
//                return StatusCode(500, new { error = "Une erreur s'est produite lors de la récupération des candidatures." });
//            }
//        }

//        /// <summary>
//        /// 8. Mettre à jour le statut d'une candidature
//        /// </summary>
//        [HttpPut("{id}/Statut")]
//        [Authorize(Roles = "Admin,Recruteur")]
//        public async Task<IActionResult> UpdateStatut(Guid id, [FromBody] UpdateStatutDto input)
//        {
//            _logger.LogInformation("Mise à jour du statut de la candidature {CandidatureId} vers {Statut}", id, input.Statut);

//            if (input == null)
//            {
//                _logger.LogWarning("Tentative de mise à jour avec un modèle null");
//                return BadRequest(new { error = "Le corps de la requête est vide" });
//            }

//            if (!ModelState.IsValid)
//            {
//                _logger.LogWarning("Validation du modèle échouée: {@ModelState}", ModelState);
//                return BadRequest(ModelState);
//            }

//            try
//            {
//                var candidature = await _context.Candidatures.FindAsync(id);
//                if (candidature == null)
//                {
//                    _logger.LogWarning("Candidature non trouvée: {CandidatureId}", id);
//                    return NotFound(new { error = "Candidature non trouvée" });
//                }

//                candidature.Statut = input.Statut;
//                await _context.SaveChangesAsync();

//                _logger.LogInformation("Statut de la candidature {CandidatureId} mis à jour avec succès vers {Statut}", id, input.Statut);
//                return Ok(new { message = "Statut de la candidature mis à jour avec succès" });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erreur lors de la mise à jour du statut de la candidature {CandidatureId}", id);
//                return StatusCode(500, new { error = "Une erreur s'est produite lors de la mise à jour du statut de la candidature." });
//            }
//        }

//        /// <summary>
//        /// 9. Filtrer les candidatures
//        /// </summary>
//        [HttpGet("Filtrer")]
//        [Authorize(Roles = "Admin,Recruteur")]
//        public async Task<ActionResult<IEnumerable<CandidatureDto>>> FiltrerCandidatures([FromQuery] FiltresCandidatureDto filtres)
//        {
//            _logger.LogInformation("Filtrage des candidatures: {@Filtres}", new
//            {
//                filtres.OffreId,
//                filtres.Statut,
//                filtres.DateDebut,
//                filtres.DateFin,
//                CompetencesCount = filtres.CompetencesIds?.Count ?? 0,
//                filtres.NiveauEtude
//            });

//            try
//            {
//                var query = _context.Candidatures
//                    .Include(c => c.AppUser)
//                    .Include(c => c.OffreEmploi)
//                    .Include(c => c.AppUser.AppUserCompetences)
//                        .ThenInclude(uc => uc.Competence)
//                    .AsQueryable();

//                // Filtrage par offre
//                if (filtres.OffreId.HasValue)
//                {
//                    query = query.Where(c => c.OffreId == filtres.OffreId.Value);
//                }

//                // Filtrage par statut
//                if (filtres.Statut.HasValue)
//                {
//                    query = query.Where(c => c.Statut == filtres.Statut.Value);
//                }

//                // Filtrage par date
//                if (filtres.DateDebut.HasValue)
//                {
//                    query = query.Where(c => c.DateSoumission >= filtres.DateDebut.Value);
//                }

//                if (filtres.DateFin.HasValue)
//                {
//                    query = query.Where(c => c.DateSoumission <= filtres.DateFin.Value);
//                }

//                // Filtrage par niveau d'étude
//                if (!string.IsNullOrEmpty(filtres.NiveauEtude))
//                {
//                    query = query.Where(c => c.AppUser.Statut == filtres.NiveauEtude);
//                }

//                var candidatures = await query.ToListAsync();

//                // Filtrage par compétences (en mémoire car plus complexe)
//                if (filtres.CompetencesIds != null && filtres.CompetencesIds.Any())
//                {
//                    candidatures = candidatures
//                        .Where(c => c.AppUser.AppUserCompetences
//                            .Any(uc => filtres.CompetencesIds.Contains(uc.CompetenceId)))
//                        .ToList();
//                }

//                _logger.LogInformation("{Count} candidatures trouvées après filtrage", candidatures.Count);
//                return Ok(candidatures.Select(c => MapToCandidatureDto(c)));
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erreur lors du filtrage des candidatures");
//                return StatusCode(500, new { error = "Une erreur s'est produite lors du filtrage des candidatures." });
//            }
//        }

//        #region Méthodes privées

//        /// <summary>
//        /// Traite un fichier uploadé et le sauvegarde dans le dossier approprié
//        /// </summary>
//        private async Task<string> ProcessUploadedFile(IFormFile file, Guid userId, string subFolder)
//        {
//            if (file == null || file.Length == 0)
//            {
//                _logger.LogWarning("Tentative de traitement d'un fichier null ou vide");
//                return null;
//            }

//            try
//            {
//                // Création du dossier de base pour les uploads
//                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
//                if (!Directory.Exists(uploadsFolder))
//                {
//                    _logger.LogInformation("Création du dossier uploads: {Path}", uploadsFolder);
//                    Directory.CreateDirectory(uploadsFolder);
//                }

//                // Création du dossier spécifique à l'utilisateur
//                string userFolder = Path.Combine(uploadsFolder, userId.ToString());
//                if (!Directory.Exists(userFolder))
//                {
//                    _logger.LogInformation("Création du dossier utilisateur: {Path}", userFolder);
//                    Directory.CreateDirectory(userFolder);
//                }

//                // Création du sous-dossier spécifique au type de fichier
//                string typeFolder = Path.Combine(userFolder, subFolder);
//                if (!Directory.Exists(typeFolder))
//                {
//                    _logger.LogInformation("Création du sous-dossier: {Path}", typeFolder);
//                    Directory.CreateDirectory(typeFolder);
//                }

//                // Génération d'un nom de fichier unique
//                string fileExtension = Path.GetExtension(file.FileName);
//                string uniqueFileName = $"{subFolder}_{Guid.NewGuid()}{fileExtension}";
//                string filePath = Path.Combine(typeFolder, uniqueFileName);

//                _logger.LogInformation("Sauvegarde du fichier: {Path}", filePath);

//                // Sauvegarde du fichier
//                using (var stream = new FileStream(filePath, FileMode.Create))
//                {
//                    await file.CopyToAsync(stream);
//                }

//                // Retourne l'URL relative du fichier
//                return $"/uploads/{userId}/{subFolder}/{uniqueFileName}";
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erreur lors du traitement du fichier {FileName}", file.FileName);
//                throw;
//            }
//        }

//        /// <summary>
//        /// Convertit une entité Candidature en DTO
//        /// </summary>
//        private CandidatureDto MapToCandidatureDto(Candidature candidature)
//        {
//            if (candidature == null)
//            {
//                _logger.LogWarning("Tentative de mapping d'une candidature null");
//                return null;
//            }

//            try
//            {
//                _logger.LogInformation("Mapping de la candidature {CandidatureId} vers DTO", candidature.IdCandidature);

//                var result = new CandidatureDto
//                {
//                    IdCandidature = candidature.IdCandidature,
//                    AppUserId = candidature.AppUserId,
//                    OffreEmploiId = candidature.OffreId,
//                    MessageMotivation = candidature.MessageMotivation,
//                    DatePostulation = candidature.DateSoumission,
//                    Statut = candidature.Statut,
//                    CV = candidature.CV,
//                    LettreMotivation = candidature.lettreMotivation,
//                };

//                // Mapping de l'utilisateur
//                if (candidature.AppUser != null)
//                {
//                    result.AppUser = new AppUserDto
//                    {
//                        Id = candidature.AppUser.Id,
//                        FullName = candidature.AppUser.FullName,
//                        Email = candidature.AppUser.Email,
//                        Phone = candidature.AppUser.phone,
//                        Nom = candidature.AppUser.Nom,
//                        Prenom = candidature.AppUser.Prenom,
//                        DateNaissance = candidature.AppUser.DateNaissance,
//                        Adresse = candidature.AppUser.Adresse,
//                        Ville = candidature.AppUser.Ville,
//                        Pays = candidature.AppUser.Pays,
//                        LinkedIn = candidature.AppUser.linkedIn,
//                        GitHub = candidature.AppUser.github,
//                        Portfolio = candidature.AppUser.portfolio,
//                        Statut = candidature.AppUser.Statut
//                    };

//                    // Mapping des expériences
//                    if (candidature.AppUser.Experiences != null)
//                    {
//                        result.Experiences = candidature.AppUser.Experiences.Select(e => new ExperienceDto
//                        {
//                            Id = e.IdExperience,
//                            Poste = e.Poste,
//                            Description = e.Description,
//                            NomEntreprise = e.NomEntreprise,
//                            DateDebut = e.DateDebut,
//                            DateFin = e.DateFin
//                        }).ToList();
//                    }

//                    // Mapping des compétences
//                    if (candidature.AppUser.AppUserCompetences != null)
//                    {
//                        result.Competences = candidature.AppUser.AppUserCompetences.Select(c => new CompetenceCandidateDto
//                        {
//                            Id = c.CompetenceId,
//                            Nom = c.Competence?.Nom,
//                            Description = c.Competence?.Description,
//                            NiveauPossede = c.NiveauPossede,
//                            EstLieeOffre = c.OffreId.HasValue,
//                            OffreId = c.OffreId
//                        }).ToList();
//                    }

//                    // Mapping des certificats
//                    if (candidature.AppUser.Certificats != null)
//                    {
//                        result.Certificats = candidature.AppUser.Certificats.Select(c => new CertificatDto
//                        {
//                            Id = c.IdCertificat,
//                            Nom = c.Nom,
//                            DateObtention = c.DateObtention,
//                            Organisme = c.Organisme,
//                            Description = c.Description,
//                            UrlDocument = c.UrlDocument
//                        }).ToList();
//                    }

//                    // Mapping des diplômes
//                    if (candidature.AppUser.DiplomesCandidate != null)
//                    {
//                        result.Diplomes = candidature.AppUser.DiplomesCandidate.Select(d => new DiplomeCandidateDto
//                        {
//                            Id = d.IdDiplome,
//                            NomDiplome = d.NomDiplome,
//                            Institution = d.Institution,
//                            DateObtention = d.DateObtention,
//                            Specialite = d.Specialite,
//                            UrlDocument = d.UrlDocument
//                        }).ToList();
//                    }
//                }

//                // Mapping de l'offre d'emploi
//                if (candidature.OffreEmploi != null)
//                {
//                    result.OffreEmploi = new OffreEmploicandidateDto
//                    {
//                        IdOffreEmploi = candidature.OffreEmploi.IdOffreEmploi,
//                        Specialite = candidature.OffreEmploi.Specialite,
//                        DatePublication = candidature.OffreEmploi.DatePublication,
//                        DateExpiration = candidature.OffreEmploi.DateExpiration,
//                        TypeContrat = candidature.OffreEmploi.TypeContrat,
//                        ModeTravail = candidature.OffreEmploi.ModeTravail
//                    };
//                }

//                return result;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Erreur lors du mapping de la candidature {CandidatureId}", candidature?.IdCandidature);
//                throw;
//            }
//        }

//        #endregion
//    }

//    #region DTOs

//    public class SoumissionCandidatureDto
//    {
//        [Required]
//        public Guid AppUserId { get; set; }

//        [Required]
//        public Guid OffreId { get; set; }

//        [Required]
//        public string MessageMotivation { get; set; }
//    }

//    public class CandidatureFormDto
//    {
//        public Guid? IdCandidature { get; set; }

//        [Required]
//        public Guid AppUserId { get; set; }

//        [Required]
//        public Guid OffreId { get; set; }

//        [Required]
//        public string MessageMotivation { get; set; }

//        [Required]
//        public string FullName { get; set; }

//        [Required]
//        public string Nom { get; set; }

//        [Required]
//        public string Prenom { get; set; }

//        public DateTime? DateNaissance { get; set; }
//        public string Adresse { get; set; }
//        public string Ville { get; set; }
//        public string Pays { get; set; }
//        public string Phone { get; set; }
//        public string LinkedIn { get; set; }
//        public string GitHub { get; set; }
//        public string Portfolio { get; set; }
//        public string Statut { get; set; }

//        // Fichiers
//        public IFormFile CvFile { get; set; }
//        public IFormFile LettreMotivationFile { get; set; }

//        // Collections
//        public List<DiplomeFormDto> Diplomes { get; set; }
//        public List<ExperienceInputDto> Experiences { get; set; }
//        public List<CompetenceInputDto> Competences { get; set; }
//        public List<CertificatFormDto> Certificats { get; set; }
//    }

//    public class DiplomeFormDto
//    {
//        public Guid? Id { get; set; }
//        public string NomDiplome { get; set; }
//        public string Institution { get; set; }
//        public DateTime? DateObtention { get; set; }
//        public string Specialite { get; set; }
//        public string UrlDocument { get; set; }
//        public IFormFile DiplomeFile { get; set; }
//    }

//    public class CertificatFormDto
//    {
//        public Guid? Id { get; set; }
//        public string Nom { get; set; }
//        public DateTime? DateObtention { get; set; }
//        public string Organisme { get; set; }
//        public string Description { get; set; }
//        public string UrlDocument { get; set; }
//        public IFormFile CertificatFile { get; set; }
//    }

//    public class ExperienceInputDto
//    {
//        public Guid? Id { get; set; }
//        public string Poste { get; set; }
//        public string Description { get; set; }
//        public string NomEntreprise { get; set; }
//        public DateTime? DateDebut { get; set; }
//        public DateTime? DateFin { get; set; }
//    }

//    public class CompetenceInputDto
//    {
//        public Guid IdCompetence { get; set; }

//        [JsonConverter(typeof(JsonStringEnumConverter))]
//        public NiveauPossedeType NiveauPossede { get; set; }

//        public bool EstLieeOffre { get; set; } = true;
//    }

//    public class UpdateStatutDto
//    {
//        [Required]
//        [JsonConverter(typeof(JsonStringEnumConverter))]
//        public StatutCandidature Statut { get; set; }
//    }

//    public class FiltresCandidatureDto
//    {
//        public Guid? OffreId { get; set; }

//        [JsonConverter(typeof(JsonStringEnumConverter))]
//        public StatutCandidature? Statut { get; set; }

//        public DateTime? DateDebut { get; set; }
//        public DateTime? DateFin { get; set; }
//        public List<Guid> CompetencesIds { get; set; }
//        public string NiveauEtude { get; set; }
//    }

//    public class CandidatureDto
//    {
//        public Guid IdCandidature { get; set; }
//        public Guid AppUserId { get; set; }
//        public Guid OffreEmploiId { get; set; }
//        public string MessageMotivation { get; set; }
//        public DateTime DatePostulation { get; set; }

//        [JsonConverter(typeof(JsonStringEnumConverter))]
//        public StatutCandidature Statut { get; set; }

//        public string CV { get; set; }
//        public string LettreMotivation { get; set; }

//        // Relations
//        public AppUserDto AppUser { get; set; }
//        public OffreEmploicandidateDto OffreEmploi { get; set; }
//        public List<ExperienceDto> Experiences { get; set; }
//        public List<CompetenceCandidateDto> Competences { get; set; }
//        public List<CertificatDto> Certificats { get; set; }
//        public List<DiplomeCandidateDto> Diplomes { get; set; }
//    }

//    public class AppUserDto
//    {
//        public Guid Id { get; set; }
//        public string FullName { get; set; }
//        public string Email { get; set; }
//        public string Phone { get; set; }
//        public string Nom { get; set; }
//        public string Prenom { get; set; }
//        public DateTime? DateNaissance { get; set; }
//        public string Adresse { get; set; }
//        public string Ville { get; set; }
//        public string Pays { get; set; }
//        public string LinkedIn { get; set; }
//        public string GitHub { get; set; }
//        public string Portfolio { get; set; }
//        public string Statut { get; set; }
//    }

//    public class ExperienceDto
//    {
//        public Guid Id { get; set; }
//        public string Poste { get; set; }
//        public string Description { get; set; }
//        public string NomEntreprise { get; set; }
//        public DateTime? DateDebut { get; set; }
//        public DateTime? DateFin { get; set; }
//    }

//    public class CompetenceCandidateDto
//    {
//        public Guid Id { get; set; }
//        public string Nom { get; set; }
//        public string Description { get; set; }

//        [JsonConverter(typeof(JsonStringEnumConverter))]
//        public NiveauPossedeType NiveauPossede { get; set; }

//        public bool EstLieeOffre { get; set; }
//        public Guid? OffreId { get; set; }
//    }

//    public class CertificatDto
//    {
//        public Guid Id { get; set; }
//        public string Nom { get; set; }
//        public DateTime? DateObtention { get; set; }
//        public string Organisme { get; set; }
//        public string Description { get; set; }
//        public string UrlDocument { get; set; }
//    }

//    public class DiplomeCandidateDto
//    {
//        public Guid Id { get; set; }
//        public string NomDiplome { get; set; }
//        public string Institution { get; set; }
//        public DateTime? DateObtention { get; set; }
//        public string Specialite { get; set; }
//        public string UrlDocument { get; set; }
//    }

//    public class OffreEmploicandidateDto
//    {
//        public Guid IdOffreEmploi { get; set; }
//        public string Specialite { get; set; }
//        public DateTime DatePublication { get; set; }
//        public DateTime DateExpiration { get; set; }
//        public string TypeContrat { get; set; }
//        public string ModeTravail { get; set; }
//    }

//    [JsonConverter(typeof(JsonStringEnumConverter))]
//    public enum StatutCandidature
//    {
//        [Display(Name = "En attente")]
//        EnAttente = 0,

//        [Display(Name = "En cours de traitement")]
//        EnCours = 1,

//        [Display(Name = "Acceptée")]
//        Acceptee = 2,

//        [Display(Name = "Refusée")]
//        Refusee = 3,

//        [Display(Name = "Entretien planifié")]
//        Entretien = 4
//    }

//    [JsonConverter(typeof(JsonStringEnumConverter))]
//    public enum StatutOffre
//    {
//        [Display(Name = "Brouillon")]
//        Brouillon = 0,

//        [Display(Name = "Ouvert")]
//        Ouvert = 1,

//        [Display(Name = "Fermé")]
//        Ferme = 2,

//        [Display(Name = "Archivé")]
//        Archive = 3
//    }

//    #endregion
//}
