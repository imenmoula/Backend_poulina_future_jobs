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
                    return Unauthorized(new { error = "Invalid or missing user ID in token." });
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