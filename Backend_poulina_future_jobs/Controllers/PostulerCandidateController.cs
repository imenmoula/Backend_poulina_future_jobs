
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_poulina_future_jobs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace Backend_poulina_future_jobs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class PostulerCandidateController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<PostulerCandidateController> _logger;
        public PostulerCandidateController(
            AppDbContext context,
            UserManager<AppUser> userManager,
            ILogger<PostulerCandidateController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }
        // Rechercher les candidats par statut de candidature
        [HttpGet("GetCandidatesByStatus/{statut}")]
[AllowAnonymous]        
        public async Task<ActionResult<IEnumerable<CandidatureDto>>> GetCandidatesByStatus(string statut)
        {
            try
            {
                if (string.IsNullOrEmpty(statut) || statut.Length > 50)
                {
                    _logger.LogWarning("Recherche par statut : Statut fourni est invalide ou vide.");
                    return BadRequest(new { error = "Le statut est requis et doit être valide." });
                }

                _logger.LogInformation("Recherche des candidatures avec Statut: {Statut}", statut);

                var candidatures = await _context.Candidatures
                    .Where(c => c.Statut == statut)
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.Experiences)
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.AppUserCompetences).ThenInclude(auc => auc.Competence)
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.Certificats)
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.DiplomesCandidate)
                    .Include(c => c.Offre)
                    .ToListAsync();

                if (!candidatures.Any())
                {
                    _logger.LogInformation("Aucune candidature trouvée pour le Statut: {Statut}", statut);
                    return NotFound(new { error = $"Aucune candidature trouvée pour le statut {statut}." });
                }

                var candidatureDtos = candidatures.Select(c => MapToCandidatureDto(c)).ToList();
                _logger.LogInformation("Récupération réussie de {Count} candidatures pour le Statut: {Statut}", candidatureDtos.Count, statut);
                return Ok(candidatureDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la recherche des candidatures par Statut: {Statut}", statut);
                return StatusCode(500, new { error = "Une erreur interne s'est produite lors de la recherche des candidatures." });
            }
        }

        // Récupérer toutes les candidatures
        [HttpGet("GetAllCandidates")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CandidatureDto>>> GetAllCandidates()
        {
            try
            {
                _logger.LogInformation("Récupération de toutes les candidatures pour un recruteur.");

                var candidatures = await _context.Candidatures
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.Experiences)
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.AppUserCompetences).ThenInclude(auc => auc.Competence)
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.Certificats)
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.DiplomesCandidate)
                    .Include(c => c.Offre)
                    .ToListAsync();

                var candidatureDtos = candidatures.Select(c => MapToCandidatureDto(c)).ToList();
                _logger.LogInformation("Récupération réussie de {Count} candidatures.", candidatureDtos.Count);
                return Ok(candidatureDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de toutes les candidatures.");
                return StatusCode(500, new { error = "Une erreur interne s'est produite lors de la récupération des candidatures." });
            }
        }

        // Création d'une candidature avec validation des compétences
        [HttpPost]
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult<CandidatureResponse>> CreateCandidature([FromBody] CandidatureCompleteDto dto)
        {
            _logger.LogInformation("Received DTO for CreateCandidature: {@Dto}", dto);

            if (dto == null)
            {
                _logger.LogWarning("CreateCandidature - DTO is null");
                return BadRequest(new { error = "Le DTO est requis." });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for CreateCandidature: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ModelState);
            }

            var userClaims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            _logger.LogInformation("User Claims: {@Claims}", userClaims);

            var userIdFromTokenString = User.FindFirst("userId")?.Value;
            _logger.LogInformation("CreateCandidature - Raw User ID from Token: {UserId}", userIdFromTokenString);

            if (string.IsNullOrEmpty(userIdFromTokenString) || !Guid.TryParse(userIdFromTokenString, out var appUserIdFromToken))
            {
                _logger.LogError("CreateCandidature - Failed to parse User ID from Token: {UserId}", userIdFromTokenString);
                return Unauthorized(new { error = "Identifiant utilisateur invalide." });
            }

            _logger.LogInformation("CreateCandidature - Token AppUserId: {TokenId}, DTO AppUserId: {DtoId}", appUserIdFromToken, dto.AppUserId);

            if (appUserIdFromToken != dto.AppUserId)
            {
                _logger.LogWarning("CreateCandidature - Unauthorized attempt: Token AppUserId {TokenId} does not match DTO AppUserId {DtoId}", appUserIdFromToken, dto.AppUserId);
                return Unauthorized(new { error = "Vous ne pouvez postuler que pour vous-même." });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("CreateCandidature - Checking offer with OffreId: {OffreId}", dto.OffreId);
                var offre = await _context.OffresEmploi.FindAsync(dto.OffreId);
                if (offre == null)
                {
                    _logger.LogWarning("CreateCandidature - Offer not found for OffreId: {OffreId}", dto.OffreId);
                    return NotFound(new { error = "Offre non trouvée." });
                }
                if ((int)offre.Statut != (int)StatutOffre.Ouvert)
                {
                    _logger.LogWarning("CreateCandidature - Offer {OffreId} is not open. Status: {Status}", dto.OffreId, offre.Statut);
                    return BadRequest(new { error = "L'offre est clôturée." });
                }

                _logger.LogInformation("CreateCandidature - Checking for existing candidature for AppUserId: {AppUserId}, OffreId: {OffreId}", appUserIdFromToken, dto.OffreId);
                if (await _context.Candidatures.AnyAsync(c => c.AppUserId == appUserIdFromToken && c.OffreId == dto.OffreId))
                {
                    _logger.LogWarning("CreateCandidature - Duplicate candidature found for AppUserId: {AppUserId}, OffreId: {OffreId}", appUserIdFromToken, dto.OffreId);
                    return BadRequest(new { error = "Vous avez déjà postulé pour cette offre." });
                }

                _logger.LogInformation("CreateCandidature - Validating required competences for OffreId: {OffreId}", dto.OffreId);
                var requiredCompetences = await _context.OffreCompetences
                    .Where(oc => oc.IdOffreEmploi == dto.OffreId)
                    .Select(oc => oc.IdCompetence)
                    .ToListAsync();
                var candidateCompetences = dto.Competences?.Select(c => c.CompetenceId).ToList() ?? new List<Guid>();
                var missingCompetences = requiredCompetences.Except(candidateCompetences).ToList();
                if (missingCompetences.Any())
                {
                    var missingNames = await _context.Competences
                        .Where(c => missingCompetences.Contains(c.Id))
                        .Select(c => c.Nom)
                        .ToListAsync();
                    _logger.LogWarning("CreateCandidature - Missing competences: {MissingCompetences}", string.Join(", ", missingNames));
                    return BadRequest(new { error = "Compétences requises manquantes.", missingCompetences = missingNames });
                }

                _logger.LogInformation("CreateCandidature - Updating user information for AppUserId: {AppUserId}", appUserIdFromToken);
                var user = await _userManager.FindByIdAsync(userIdFromTokenString);
                if (user == null)
                {
                    _logger.LogWarning("CreateCandidature - User not found for AppUserId: {AppUserId}", appUserIdFromToken);
                    return NotFound(new { error = "Utilisateur non trouvé." });
                }

                MapUserFromDto(user, dto);
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    _logger.LogError("CreateCandidature - Error updating user: {Errors}", string.Join(", ", updateResult.Errors.Select(e => e.Description)));
                    await transaction.RollbackAsync();
                    return BadRequest(new { error = "Erreur lors de la mise à jour de l'utilisateur.", details = updateResult.Errors });
                }

                _logger.LogInformation("CreateCandidature - Handling related entities for AppUserId: {AppUserId}", appUserIdFromToken);
                await HandleRelatedEntities(dto, appUserIdFromToken, true);

                _logger.LogInformation("CreateCandidature - Creating new candidature with AppUserId: {AppUserId}, OffreId: {OffreId}", appUserIdFromToken, dto.OffreId);
                var candidature = new Candidature
                {
                    IdCandidature = Guid.NewGuid(),
                    AppUserId = appUserIdFromToken,
                    OffreId = dto.OffreId,
                    Statut = "Soumise",
                    MessageMotivation = dto.MessageMotivation,
                    DateSoumission = DateTime.UtcNow,
                    CvFilePath = dto.CvFilePath,
                    LinkedIn = dto.LinkedIn,
                    Github = dto.Github,
                    Portfolio = dto.Portfolio,
                    StatutCandidate = dto.StatutCandidate
                };

                _context.Candidatures.Add(candidature);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("CreateCandidature - Candidature created successfully with IdCandidature: {IdCandidature}", candidature.IdCandidature);
                var savedCandidature = await LoadCandidature(candidature.IdCandidature);
                return CreatedAtAction(
                    nameof(GetCandidature),
                    new { id = candidature.IdCandidature },
                    new CandidatureResponse
                    {
                        Message = "Candidature créée avec succès.",
                        Data = MapToCandidatureDto(savedCandidature)
                    });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "CreateCandidature - Error during candidature creation for AppUserId: {AppUserId}, OffreId: {OffreId}", appUserIdFromToken, dto.OffreId);
                return StatusCode(500, new { error = "Une erreur interne s'est produite lors de la création de la candidature." });
            }
        }

        // Mise à jour d'une candidature
        [HttpPut("{id}")]
        [Authorize(Roles = "Candidate")]
        public async Task<ActionResult<CandidatureResponse>> UpdateCandidature(Guid id, [FromBody] CandidatureCompleteDto dto)
        {
            _logger.LogInformation("Received DTO for UpdateCandidature: {@Dto}", dto);

            if (dto == null)
            {
                _logger.LogWarning("UpdateCandidature - DTO is null");
                return BadRequest(new { error = "Le DTO est requis." });
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for UpdateCandidature: {Errors}", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ModelState);
            }

            var candidature = await LoadCandidature(id);
            if (candidature == null)
            {
                _logger.LogWarning("UpdateCandidature - Candidature not found for ID: {Id}", id);
                return NotFound(new { error = "Candidature non trouvée." });
            }

            var userIdFromTokenString = User.FindFirst("userId")?.Value;
            _logger.LogInformation("UpdateCandidature - Authenticated User ID from Token: {UserId}", userIdFromTokenString);

            if (!Guid.TryParse(userIdFromTokenString, out var appUserIdFromToken) || candidature.AppUserId != appUserIdFromToken)
            {
                _logger.LogWarning("UpdateCandidature - Unauthorized attempt: Token AppUserId {TokenId} does not match Candidature AppUserId {CandidatureId}", appUserIdFromToken, candidature.AppUserId);
                return Unauthorized(new { error = "Vous n'êtes pas autorisé à modifier cette candidature." });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _logger.LogInformation("UpdateCandidature - Checking offer with OffreId: {OffreId}", dto.OffreId);
                var offre = await _context.OffresEmploi.FindAsync(dto.OffreId);
                if (offre == null)
                {
                    _logger.LogWarning("UpdateCandidature - Offer not found for OffreId: {OffreId}", dto.OffreId);
                    return NotFound(new { error = "Offre non trouvée." });
                }
                if ((int)offre.Statut != (int)StatutOffre.Ouvert)
                {
                    _logger.LogWarning("UpdateCandidature - Offer {OffreId} is not open. Status: {Status}", dto.OffreId, offre.Statut);
                    return BadRequest(new { error = "L'offre est clôturée." });
                }

                _logger.LogInformation("UpdateCandidature - Validating required competences for OffreId: {OffreId}", dto.OffreId);
                var requiredCompetences = await _context.OffreCompetences
                    .Where(oc => oc.IdOffreEmploi == dto.OffreId)
                    .Select(oc => oc.IdCompetence)
                    .ToListAsync();
                var candidateCompetences = dto.Competences?.Select(c => c.CompetenceId).ToList() ?? new List<Guid>();
                var missingCompetences = requiredCompetences.Except(candidateCompetences).ToList();
                if (missingCompetences.Any())
                {
                    var missingNames = await _context.Competences
                        .Where(c => missingCompetences.Contains(c.Id))
                        .Select(c => c.Nom)
                        .ToListAsync();
                    _logger.LogWarning("UpdateCandidature - Missing competences: {MissingCompetences}", string.Join(", ", missingNames));
                    return BadRequest(new { error = "Compétences requises manquantes.", missingCompetences = missingNames });
                }

                _logger.LogInformation("UpdateCandidature - Updating user information for AppUserId: {AppUserId}", appUserIdFromToken);
                if (candidature.AppUser == null)
                {
                    _logger.LogError("UpdateCandidature - User not loaded for Candidature ID: {Id}", id);
                    throw new InvalidOperationException("Utilisateur non chargé pour la candidature.");
                }

                MapUserFromDto(candidature.AppUser, dto);
                var updateResult = await _userManager.UpdateAsync(candidature.AppUser);
                if (!updateResult.Succeeded)
                {
                    _logger.LogError("UpdateCandidature - Error updating user: {Errors}", string.Join(", ", updateResult.Errors.Select(e => e.Description)));
                    await transaction.RollbackAsync();
                    return BadRequest(new { error = "Erreur lors de la mise à jour de l'utilisateur.", details = updateResult.Errors });
                }

                _logger.LogInformation("UpdateCandidature - Handling related entities for AppUserId: {AppUserId}", appUserIdFromToken);
                await HandleRelatedEntities(dto, appUserIdFromToken, false);

                _logger.LogInformation("UpdateCandidature - Updating candidature with ID: {Id}", id);
                candidature.MessageMotivation = dto.MessageMotivation;
                candidature.CvFilePath = dto.CvFilePath;
                candidature.LinkedIn = dto.LinkedIn;
                candidature.Github = dto.Github;
                candidature.Portfolio = dto.Portfolio;
                candidature.StatutCandidate = dto.StatutCandidate;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                _logger.LogInformation("UpdateCandidature - Candidature updated successfully with ID: {Id}", id);
                var updatedCandidature = await LoadCandidature(id);
                return Ok(new CandidatureResponse
                {
                    Message = "Candidature mise à jour avec succès.",
                    Data = MapToCandidatureDto(updatedCandidature)
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "UpdateCandidature - Error during candidature update for ID: {Id}", id);
                return StatusCode(500, new { error = "Une erreur interne s'est produite lors de la mise à jour de la candidature." });
            }
        }

        // Récupérer une candidature
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CandidatureDto>> GetCandidature(Guid id)
        {
            try
            {
                var candidature = await LoadCandidature(id);
                if (candidature == null)
                    return NotFound(new { error = "Candidature non trouvée." });

                var userId = User.FindFirst("userId")?.Value;
                bool isRecruteur = User.IsInRole("Recruteur");

                if (candidature.AppUserId.ToString() != userId && !isRecruteur)
                    return Unauthorized(new { error = "Vous n'êtes pas autorisé à consulter cette candidature." });

                return Ok(MapToCandidatureDto(candidature));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la candidature");
                return StatusCode(500, new { error = "Une erreur interne s'est produite lors de la récupération de la candidature." });
            }
        }

        // Supprimer une candidature
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteCandidature(Guid id)
        {
            try
            {
                var candidature = await _context.Candidatures.FindAsync(id);
                if (candidature == null)
                    return NotFound(new { error = "Candidature non trouvée." });

                var userId = User.FindFirst("userId")?.Value;
                bool isRecruteur = User.IsInRole("Recruteur");

                if (candidature.AppUserId.ToString() != userId && !isRecruteur)
                    return Unauthorized(new { error = "Vous n'êtes pas autorisé à supprimer cette candidature." });

                _context.Candidatures.Remove(candidature);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de la candidature");
                return StatusCode(500, new { error = "Une erreur interne s'est produite lors de la suppression de la candidature." });
            }
        }

        // Récupérer les candidats pour une offre
        [HttpGet("GetCandidatesForOffre/{offreId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CandidatureDto>>> GetCandidatesForOffre(Guid offreId)
        {
            try
            {
                var offreExists = await _context.OffresEmploi.AnyAsync(o => o.IdOffreEmploi == offreId);
                if (!offreExists)
                    return NotFound(new { error = "Offre non trouvée." });

                var candidatures = await _context.Candidatures
                    .Where(c => c.OffreId == offreId)
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.Experiences)
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.AppUserCompetences).ThenInclude(auc => auc.Competence)
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.Certificats)
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.DiplomesCandidate)
                    .Include(c => c.Offre)
                    .ToListAsync();

                var candidatureDtos = candidatures.Select(c => MapToCandidatureDto(c)).ToList();
                return Ok(candidatureDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des candidats pour une offre");
                return StatusCode(500, new { error = "Une erreur interne s'est produite lors de la récupération des candidats." });
            }
        }

        // Filtrer les candidats selon les compétences requises
        [HttpGet("GetFilteredCandidates/{offreId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<UserInfoDto>>> GetFilteredCandidatesByOffer(Guid offreId)
        {
            try
            {
                var requiredCompetences = await _context.OffreCompetences
                    .Where(oc => oc.IdOffreEmploi == offreId)
                    .Select(oc => new { oc.IdCompetence, oc.NiveauRequis })
                    .ToListAsync();

                if (!requiredCompetences.Any())
                    return Ok(new List<UserInfoDto>());

                var candidatures = await _context.Candidatures
                    .Where(c => c.OffreId == offreId)
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.Experiences)
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.AppUserCompetences).ThenInclude(auc => auc.Competence)
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.Certificats)
                    .Include(c => c.AppUser)
                        .ThenInclude(u => u.DiplomesCandidate)
                    .ToListAsync();

                var filteredCandidates = candidatures
                    .Where(c =>
                    {
                        if (c.AppUser?.AppUserCompetences == null) return false;

                        var userCompetences = c.AppUser.AppUserCompetences
                            .Select(auc => new { auc.CompetenceId, auc.NiveauPossede })
                            .ToList();

                        int matchCount = 0;
                        foreach (var req in requiredCompetences)
                        {
                            var userComp = userCompetences.FirstOrDefault(uc => uc.CompetenceId == req.IdCompetence);
                            if (userComp != null && (int)userComp.NiveauPossede >= (int)req.NiveauRequis)
                                matchCount++;
                        }

                        double matchPercentage = requiredCompetences.Count > 0 ? (double)matchCount / requiredCompetences.Count : 0;
                        return matchPercentage >= 0.5;
                    })
                    .Select(c => MapToUserInfoDto(c.AppUser))
                    .DistinctBy(u => u.Id)
                    .ToList();

                return Ok(filteredCandidates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du filtrage des candidats");
                return StatusCode(500, new { error = "Une erreur interne s'est produite lors du filtrage des candidats." });
            }
        }

        // Mettre à jour le statut d'une candidature (par un recruteur)
        [HttpPatch("UpdateCandidatureStatus/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateCandidatureStatus(Guid id, [FromBody] StatutUpdateRequest request)
        {
            if (string.IsNullOrEmpty(request?.Statut) || request.Statut.Length > 50)
                return BadRequest(new { error = "Le statut fourni est invalide ou trop long." });

            try
            {
                var candidature = await _context.Candidatures.FindAsync(id);
                if (candidature == null)
                    return NotFound(new { error = "Candidature non trouvée." });

                candidature.Statut = request.Statut;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Statut de la candidature mis à jour avec succès." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du statut de la candidature");
                return StatusCode(500, new { error = "Une erreur interne s'est produite lors de la mise à jour du statut." });
            }
        }

        #region Helper Methods

        private async Task<Candidature> LoadCandidature(Guid id)
        {
            return await _context.Candidatures
                .Include(c => c.AppUser)
                    .ThenInclude(u => u.Experiences)
                .Include(c => c.AppUser)
                    .ThenInclude(u => u.AppUserCompetences).ThenInclude(auc => auc.Competence)
                .Include(c => c.AppUser)
                    .ThenInclude(u => u.Certificats)
                .Include(c => c.AppUser)
                    .ThenInclude(u => u.DiplomesCandidate)
                .Include(c => c.Offre)
                .FirstOrDefaultAsync(c => c.IdCandidature == id);
        }

        private async Task HandleRelatedEntities(CandidatureCompleteDto dto, Guid userId, bool isCreation)
        {
            if (dto.Competences != null)
            {
                foreach (var comp in dto.Competences)
                {
                    if (!await _context.Competences.AnyAsync(c => c.Id == comp.CompetenceId))
                        throw new ValidationException($"Compétence avec ID {comp.CompetenceId} non trouvée.");
                    if (!Enum.IsDefined(typeof(NiveauCompetence), comp.NiveauPossede))
                        throw new ValidationException($"NiveauPossede {comp.NiveauPossede} invalide.");
                }
            }

            if (!isCreation)
            {
                var oldUserCompetences = await _context.AppUserCompetences
                    .Where(c => c.AppUserId == userId && c.OffreId == dto.OffreId)
                    .ToListAsync();
                _context.AppUserCompetences.RemoveRange(oldUserCompetences);
            }

            if (dto.Diplomes != null)
            {
                foreach (var diplomeDto in dto.Diplomes)
                {
                    _context.DiplomesCandidate.Add(new DiplomeCandidate
                    {
                        IdDiplome = Guid.NewGuid(),
                        AppUserId = userId,
                        NomDiplome = diplomeDto.NomDiplome,
                        Institution = diplomeDto.Institution,
                        DateObtention = diplomeDto.DateObtention,
                        Specialite = diplomeDto.Specialite,
                        UrlDocument = diplomeDto.UrlDocument
                    });
                }
            }

            if (dto.Experiences != null)
            {
                foreach (var expDto in dto.Experiences)
                {
                    _context.Experiences.Add(new Experience
                    {
                        IdExperience = Guid.NewGuid(),
                        AppUserId = userId,
                        Poste = expDto.Poste,
                        NomEntreprise = expDto.NomEntreprise,
                        Description = expDto.Description,
                        CompetenceAcquise = expDto.CompetenceAcquise,
                        DateDebut = expDto.DateDebut,
                        DateFin = expDto.DateFin
                    });
                }
            }

            if (dto.Competences != null)
            {
                foreach (var compDto in dto.Competences)
                {
                    _context.AppUserCompetences.Add(new AppUserCompetence
                    {
                        Id = Guid.NewGuid(),
                        AppUserId = userId,
                        CompetenceId = compDto.CompetenceId,
                        NiveauPossede = (NiveauPossedeType)compDto.NiveauPossede,
                        OffreId = dto.OffreId
                    });
                }
            }

            if (dto.Certificats != null)
            {
                foreach (var certDto in dto.Certificats)
                {
                    _context.Certificats.Add(new Certificat
                    {
                        IdCertificat = Guid.NewGuid(),
                        AppUserId = userId,
                        Nom = certDto.Nom,
                        DateObtention = certDto.DateObtention,
                        Organisme = certDto.Organisme,
                        Description = certDto.Description,
                        UrlDocument = certDto.UrlDocument
                    });
                }
            }
        }

        private async Task ClearExistingRelations(Guid userId, Guid offreId)
        {
            await _context.DiplomesCandidate.Where(d => d.AppUserId == userId).ExecuteDeleteAsync();
            await _context.Experiences.Where(e => e.AppUserId == userId).ExecuteDeleteAsync();
            await _context.AppUserCompetences.Where(c => c.AppUserId == userId && c.OffreId == offreId).ExecuteDeleteAsync();
            await _context.Certificats.Where(c => c.AppUserId == userId).ExecuteDeleteAsync();
        }

        private void MapUserFromDto(AppUser user, CandidatureCompleteDto dto)
        {
            user.FullName = dto.FullName;
            user.Nom = dto.Nom;
            user.Prenom = dto.Prenom;
            user.DateNaissance = dto.DateNaissance;
            user.Adresse = dto.Adresse;
            user.Ville = dto.Ville;
            user.Pays = dto.Pays;
            user.PhoneNumber = dto.Phone;
            user.Entreprise = dto.Entreprise;
            user.Poste = dto.Poste;
            user.Photo = dto.PhotoUrl;
        }

        private CandidatureDto MapToCandidatureDto(Candidature candidature)
        {
            if (candidature == null) return null;

            return new CandidatureDto
            {
                Id = candidature.IdCandidature,
                Statut = candidature.Statut,
                MessageMotivation = candidature.MessageMotivation,
                DateSoumission = candidature.DateSoumission,
                CvFilePath = candidature.CvFilePath,
                LettreMotivation = candidature.LettreMotivation,
                LinkedIn = candidature.LinkedIn,
                Github = candidature.Github,
                Portfolio = candidature.Portfolio,
                StatutCandidate = candidature.StatutCandidate,
                Offre = candidature.Offre == null ? null : new OffreDto
                {
                    IdOffreEmploi = candidature.Offre.IdOffreEmploi,
                    Specialite = candidature.Offre.Specialite,
                    TypeContrat = candidature.Offre.TypeContrat.ToString(),
                    Statut = candidature.Offre.Statut.ToString()
                },
                UserInfo = MapToUserInfoDto(candidature.AppUser)
            };
        }

        private UserInfoDto MapToUserInfoDto(AppUser user)
        {
            if (user == null) return null;

            return new UserInfoDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                PhotoUrl = user.Photo,
                Experiences = user.Experiences?.Select(e => new CandidatureExperienceDto(
                    e.IdExperience,
                    e.Poste,
                    e.NomEntreprise,
                    e.DateDebut ?? DateTime.MinValue,
                    e.DateFin,
                    e.Description,
                    e.CompetenceAcquise
                )).ToList() ?? new List<CandidatureExperienceDto>(),
                Diplomes = user.DiplomesCandidate?.Select(d => new CandidatureDiplomeDto(
                    d.IdDiplome,
                    d.NomDiplome,
                    d.Institution,
                    d.DateObtention,
                    d.Specialite,
                    d.UrlDocument
                )).ToList() ?? new List<CandidatureDiplomeDto>(),
                Competences = user.AppUserCompetences?.Select(auc => new CandidatureCompetenceDto(
                    auc.CompetenceId,
                    auc.Competence?.Nom,
                    (NiveauCompetence)auc.NiveauPossede
                )).ToList() ?? new List<CandidatureCompetenceDto>(),
                Certificats = user.Certificats?.Select(c => new CandidatureCertificatDto(
                    c.IdCertificat,
                    c.Nom,
                    c.Organisme,
                    c.DateObtention,
                    c.Description,
                    c.UrlDocument
                )).ToList() ?? new List<CandidatureCertificatDto>()
            };
        }
        [HttpPost("upload-documents/{candidatureId}")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadDocuments(string candidatureId, [FromForm] List<IFormFile> files)
        {
            if (files == null || files.Count == 0)
                return BadRequest("Aucun fichier reçu.");

            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", candidatureId);

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                    // Optionnel : filtrer les formats autorisés
                    var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".odt", ".rtf", ".txt" };
                    if (!allowedExtensions.Contains(extension))
                        return BadRequest($"Format non autorisé : {extension}");

                    var filePath = Path.Combine(uploadPath, Path.GetFileName(file.FileName));

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
            }

            return Ok(new { message = "Fichiers téléchargés avec succès." });
        }

        #endregion

        #region DTOs

        // Response wrapper for Candidature operations
        public class CandidatureResponse
        {
            public string Message { get; set; }
            public CandidatureDto Data { get; set; }
        }

        public class CandidatureCompleteDto
        {
            [Required] public Guid AppUserId { get; set; }
            [Required] public Guid OffreId { get; set; }
            [Required, StringLength(1000)] public string MessageMotivation { get; set; }
            [Required, StringLength(255)] public string CvFilePath { get; set; }
            [Required, StringLength(255)] public string LettreMotivation { get; set; }
            [Required, Url, StringLength(255)] public string LinkedIn { get; set; }
            [Required, Url, StringLength(255)] public string Github { get; set; }
            [Required, Url, StringLength(255)] public string Portfolio { get; set; }
            [Required, StringLength(50)] public string StatutCandidate { get; set; } = "Debutant";
            [Required, StringLength(150)] public string FullName { get; set; }
            [Required, StringLength(150)] public string Nom { get; set; }
            [Required, StringLength(150)] public string Prenom { get; set; }
            public DateTime? DateNaissance { get; set; }
            [Required, StringLength(255)] public string Adresse { get; set; }
            [Required, StringLength(100)] public string Ville { get; set; }
            [Required, StringLength(100)] public string Pays { get; set; }
            [Required, Phone, StringLength(20)] public string Phone { get; set; }
            [Required, StringLength(255)] public string Entreprise { get; set; }
            [Required, StringLength(255)] public string Poste { get; set; }
            [Required, Url, StringLength(255)] public string PhotoUrl { get; set; }
            public List<CandidatureDiplomeDto> Diplomes { get; set; } = new();
            public List<CandidatureExperienceDto> Experiences { get; set; } = new();
            public List<CompetenceInputDto> Competences { get; set; } = new();
            public List<CandidatureCertificatDto> Certificats { get; set; } = new();
        }

        public class CompetenceInputDto
        {
            [Required] public Guid CompetenceId { get; set; }
            [Required] public NiveauCompetence NiveauPossede { get; set; }
        }

        public class CandidatureDto
        {
            public Guid Id { get; set; }
            public string Statut { get; set; }
            public string MessageMotivation { get; set; }
            public DateTime DateSoumission { get; set; }
            public string CvFilePath { get; set; }
            public string LettreMotivation { get; set; }
            public string LinkedIn { get; set; }
            public string Github { get; set; }
            public string Portfolio { get; set; }
            public string StatutCandidate { get; set; }
            public UserInfoDto UserInfo { get; set; }
            public OffreDto Offre { get; set; }
        }

        public class UserInfoDto
        {
            public Guid Id { get; set; }
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string PhotoUrl { get; set; }
            public List<CandidatureExperienceDto> Experiences { get; set; }
            public List<CandidatureDiplomeDto> Diplomes { get; set; }
            public List<CandidatureCompetenceDto> Competences { get; set; }
            public List<CandidatureCertificatDto> Certificats { get; set; }
        }

        public class OffreDto
        {
            public Guid IdOffreEmploi { get; set; }
            public string Specialite { get; set; }
            public string TypeContrat { get; set; }
            public string Statut { get; set; }
        }

        //public record CandidatureDiplomeDto(
        //    Guid Id,
        //    string NomDiplome,
        //    string Institution,
        //    DateTime DateObtention,
        //    string Specialite = null,
        //    string UrlDocument = null);

        //public record CandidatureExperienceDto(
        //    Guid Id,
        //    string Poste,
        //    string NomEntreprise,
        //    DateTime DateDebut,
        //    DateTime? DateFin,
        //    string Description = null,
        //    string CompetenceAcquise = null);

        public record CandidatureCompetenceDto(
            Guid Id,
            string Nom,
            NiveauCompetence NiveauPossede);

        //public record CandidatureCertificatDto(
        //    Guid Id,
        //    string Nom,
        //    string Organisme,
        //    DateTime DateObtention,
        //    string Description = null,
        //    string UrlDocument = null);

        public class StatutUpdateRequest
        {
            [Required]
            [StringLength(50)]
            public string Statut { get; set; }
        }
        #endregion

        #region Enums
        public enum StatutOffre
        {
            Ouvert,
            Cloture,
            Suspendu
        }

        public enum NiveauCompetence
        {
            Debutant = 0,
            Intermediaire = 1,
            Avance = 2,
            Expert = 3
        }
        #endregion
    }
}