

////using Backend_poulina_future_jobs.Models;
////using Microsoft.AspNetCore.Authorization;
////using Microsoft.AspNetCore.Identity;
////using Microsoft.AspNetCore.Mvc;
////using Microsoft.EntityFrameworkCore;
////using System;
////using System.Collections.Generic;
////using System.ComponentModel.DataAnnotations;
////using System.Linq;
////using System.Threading.Tasks;

////namespace Backend_poulina_future_jobs.Controllers
////{
////    [Route("api/[controller]")]
////    [ApiController]
////    public class CandidaturesController : ControllerBase
////    {
////        private readonly AppDbContext _context;
////        private readonly UserManager<AppUser> _userManager;

////        public CandidaturesController(AppDbContext context, UserManager<AppUser> userManager)
////        {
////            _context = context;
////            _userManager = userManager;
////        }

////        [HttpGet]
////        [AllowAnonymous]
////        public async Task<ActionResult<object>> GetAllCandidatures()
////        {
////            var candidatures = await _context.Candidatures
////                .Include(c => c.AppUser)
////                .Include(c => c.Offre)
////                .Include(c => c.AppUser).ThenInclude(u => u.Experiences).ThenInclude(e => e.Certificats)
////                .Include(c => c.AppUser).ThenInclude(u => u.AppUserCompetences).ThenInclude(cc => cc.Competence)
////                .ToListAsync();

////            var response = candidatures.Select(c => MapToResponseDto(c)).ToList();
////            return Ok(response);
////        }

////        [HttpGet("{id}")]
////        [AllowAnonymous]
////        public async Task<IActionResult> GetCandidature(Guid id)
////        {
////            var candidature = await _context.Candidatures
////                .Include(c => c.AppUser)
////                .Include(c => c.Offre)
////                .Include(c => c.AppUser).ThenInclude(u => u.Experiences).ThenInclude(e => e.Certificats)
////                .Include(c => c.AppUser).ThenInclude(u => u.AppUserCompetences).ThenInclude(cc => cc.Competence)
////                .FirstOrDefaultAsync(c => c.IdCandidature == id);

////            if (candidature == null)
////            {
////                return NotFound(new { message = "Candidature introuvable." });
////            }

////            return Ok(MapToResponseDto(candidature));
////        }

////        [HttpPost]
////        [AllowAnonymous]
////        public async Task<IActionResult> CreateCandidature([FromBody] CandidatureFormDto candidatureDto)
////        {
////            try
////            {
////                // Validation des champs requis  
////                if (candidatureDto.OffreId == Guid.Empty || candidatureDto.AppUserId == Guid.Empty || string.IsNullOrEmpty(candidatureDto.MessageMotivation))
////                {
////                    return BadRequest(new { message = "Les champs OffreId, AppUserId et MessageMotivation sont requis." });
////                }

////                var user = await _userManager.FindByIdAsync(candidatureDto.AppUserId.ToString());
////                var offre = await _context.OffresEmploi.FirstOrDefaultAsync(o => o.IdOffreEmploi == candidatureDto.OffreId);

////                if (user == null)
////                {
////                    return BadRequest(new { message = "Utilisateur introuvable." });
////                }

////                if (offre == null)
////                {
////                    return BadRequest(new { message = "Offre introuvable." });
////                }

////                // Vérifier si une candidature existe déjà  
////                var existingCandidature = await _context.Candidatures
////                    .AnyAsync(c => c.AppUserId == candidatureDto.AppUserId && c.OffreId == candidatureDto.OffreId);
////                if (existingCandidature)
////                {
////                    return BadRequest(new { message = "Une candidature existe déjà pour cette offre." });
////                }

////                // Vérifier les compétences requises  
////                var offreCompetenceIds = await _context.OffreCompetences
////                    .Where(oc => oc.IdOffreEmploi == candidatureDto.OffreId)
////                    .Select(oc => oc.Competence.Id)
////                    .ToListAsync();

////                var candidatCompetenceIds = candidatureDto.Competences
////                    .Select(c => c.CompetenceId)
////                    .ToList();

////                var missingCompetenceIds = offreCompetenceIds
////                    .Where(id => !candidatCompetenceIds.Contains(id))
////                    .ToList();

////                if (missingCompetenceIds.Any())
////                {
////                    var missingCompetences = await _context.Competences
////                        .Where(c => missingCompetenceIds.Contains(c.Id))
////                        .Select(c => c.Nom)
////                        .ToListAsync();

////                    return BadRequest(new
////                    {
////                        message = "Le candidat ne possède pas toutes les compétences requises.",
////                        competencesManquantes = missingCompetences
////                    });
////                }

////                user.FullName = candidatureDto.FullName ?? user.FullName;
////                user.Nom = candidatureDto.Nom ?? user.Nom;
////                user.Prenom = candidatureDto.Prenom ?? user.Prenom;
////                user.DateNaissance = candidatureDto.DateNaissance ?? user.DateNaissance;
////                user.Adresse = candidatureDto.Adresse ?? user.Adresse;
////                user.Ville = candidatureDto.Ville ?? user.Ville;
////                user.Pays = candidatureDto.Pays ?? user.Pays;
////                user.phone = candidatureDto.Phone ?? user.phone;
////                user.NiveauEtude = candidatureDto.NiveauEtude ?? user.NiveauEtude;
////                user.Diplome = candidatureDto.Diplome ?? user.Diplome;
////                user.Universite = candidatureDto.Universite ?? user.Universite;
////                user.specialite = candidatureDto.Specialite ?? user.specialite;
////                user.cv = candidatureDto.Cv ?? user.cv;
////                user.linkedIn = candidatureDto.LinkedIn ?? user.linkedIn;
////                user.github = candidatureDto.Github ?? user.github;
////                user.portfolio = candidatureDto.Portfolio ?? user.portfolio;
////                user.Statut = candidatureDto.UserStatut ?? user.Statut;

////                await _userManager.UpdateAsync(user);

////                // Créer la candidature  
////                var candidature = new Candidature
////                {
////                    IdCandidature = Guid.NewGuid(),
////                    AppUserId = candidatureDto.AppUserId,
////                    OffreId = candidatureDto.OffreId,
////                    Statut = "Soumise",
////                    MessageMotivation = candidatureDto.MessageMotivation,
////                    DateSoumission = DateTime.UtcNow
////                };

////                _context.Candidatures.Add(candidature);

////                // Gérer les expériences  
////                var existingExperiences = await _context.Experiences
////                    .Where(e => e.AppUserId == candidatureDto.AppUserId)
////                    .ToListAsync();

////                if (existingExperiences.Any())
////                {
////                    _context.Experiences.RemoveRange(existingExperiences);
////                }

////                foreach (var expDto in candidatureDto.Experiences)
////                {
////                    var experience = new Experience
////                    {
////                        IdExperience = Guid.NewGuid(),
////                        AppUserId = candidatureDto.AppUserId,
////                        Poste = expDto.Poste,
////                        NomEntreprise = expDto.NomEntreprise,
////                        Description = expDto.Description,
////                        CompetenceAcquise = expDto.CompetenceAcquise,
////                        DateDebut = expDto.DateDebut,
////                        DateFin = expDto.DateFin,
////                        Certificats = new List<Certificat>()
////                    };

////                    foreach (var certDto in expDto.Certificats)
////                    {
////                        if (certDto.DateObtention.HasValue)
////                        {
////                            var certificat = new Certificat
////                            {
////                                IdCertificat = Guid.NewGuid(),
////                                ExperienceId = experience.IdExperience,
////                                Nom = certDto.Nom,
////                                DateObtention = certDto.DateObtention.Value,
////                                Organisme = certDto.Organisme,
////                                Description = certDto.Description,
////                                UrlDocument = certDto.UrlDocument
////                            };
////                            experience.Certificats.Add(certificat);
////                        }
////                    }

////                    _context.Experiences.Add(experience);
////                }

////                // Gérer les compétences  
////                var existingCompetences = await _context.AppUserCompetences
////                    .Where(cc => cc.AppUserId == candidatureDto.AppUserId)
////                    .ToListAsync();

////                if (existingCompetences.Any())
////                {
////                    _context.AppUserCompetences.RemoveRange(existingCompetences);
////                }

////                foreach (var compDto in candidatureDto.Competences)
////                {
////                    // Vérifier que la compétence existe  
////                    var competence = await _context.Competences
////                        .FirstOrDefaultAsync(c => c.Id == compDto.CompetenceId);

////                    if (competence == null)
////                    {
////                        return BadRequest(new { message = $"Compétence avec ID {compDto.CompetenceId} non trouvée." });
////                    }

////                    // Parse NiveauPossede as int  
////                    if (!int.TryParse(compDto.NiveauPossede, out var niveauPossede) || niveauPossede < 1 || niveauPossede > 4)
////                    {
////                        return BadRequest(new { message = $"NiveauPossede '{compDto.NiveauPossede}' est invalide. Valeurs acceptées : 1 (Débutant), 2 (Intermédiaire), 3 (Avancé), 4 (Expert)." });
////                    }

////                    var candidateCompetence = new AppUserCompetence
////                    {
////                        Id = Guid.NewGuid(),
////                        AppUserId = candidatureDto.AppUserId,
////                        CompetenceId = compDto.CompetenceId,
////                        NiveauPossede = (NiveauPossedeType)niveauPossede
////                    };
////                    _context.AppUserCompetences.Add(candidateCompetence);
////                }

////                await _context.SaveChangesAsync();

////                var response = MapToResponseDto(candidature);
////                return CreatedAtAction(nameof(GetCandidature), new { id = candidature.IdCandidature }, response);
////            }
////            catch (Exception ex)
////            {
////                return StatusCode(500, new { message = "Une erreur est survenue lors de la création de la candidature.", details = ex.Message });
////            }
////        }

////        [HttpPut("{id}")]
////        [AllowAnonymous]
////        public async Task<IActionResult> UpdateCandidature(Guid id, [FromBody] CandidatureFormDto candidatureDto)
////        {
////            try
////            {
////                if (id != candidatureDto.IdCandidature)
////                {
////                    return BadRequest(new { message = "L'ID de la candidature ne correspond pas." });
////                }

////                var candidature = await _context.Candidatures
////                    .Include(c => c.AppUser)
////                    .Include(c => c.Offre)
////                    .FirstOrDefaultAsync(c => c.IdCandidature == id);

////                if (candidature == null)
////                {
////                    return NotFound(new { message = "Candidature introuvable." });
////                }

////                var user = await _userManager.FindByIdAsync(candidatureDto.AppUserId.ToString());
////                if (user == null)
////                {
////                    return BadRequest(new { message = "Utilisateur introuvable." });
////                }

////                // Vérifier les compétences requises  
////                var offreCompetenceIds = await _context.OffreCompetences
////                    .Where(oc => oc.IdOffreEmploi == candidatureDto.OffreId)
////                    .Select(oc => oc.IdCompetence)
////                    .ToListAsync();

////                var candidatCompetenceIds = candidatureDto.Competences
////                    .Select(c => c.CompetenceId)
////                    .ToList();

////                var missingCompetenceIds = offreCompetenceIds
////                    .Except(candidatCompetenceIds)
////                    .ToList();

////                if (missingCompetenceIds.Any())
////                {
////                    var missingCompetences = await _context.Competences
////                        .Where(c => missingCompetenceIds.Contains(c.Id))
////                        .Select(c => c.Nom)
////                        .ToListAsync();

////                    return BadRequest(new
////                    {
////                        message = "Le candidat ne possède pas toutes les compétences requises.",
////                        competencesManquantes = missingCompetences
////                    });
////                }

////                // Mettre à jour les informations de l'utilisateur  
////                user.FullName = candidatureDto.FullName ?? user.FullName;
////                user.Nom = candidatureDto.Nom ?? user.Nom;
////                user.Prenom = candidatureDto.Prenom ?? user.Prenom;
////                user.DateNaissance = candidatureDto.DateNaissance ?? user.DateNaissance;
////                user.Adresse = candidatureDto.Adresse ?? user.Adresse;
////                user.Ville = candidatureDto.Ville ?? user.Ville;
////                user.Pays = candidatureDto.Pays ?? user.Pays;
////                user.phone = candidatureDto.Phone ?? user.phone;
////                user.NiveauEtude = candidatureDto.NiveauEtude ?? user.NiveauEtude;
////                user.Diplome = candidatureDto.Diplome ?? user.Diplome;
////                user.Universite = candidatureDto.Universite ?? user.Universite;
////                user.specialite = candidatureDto.Specialite ?? user.specialite;
////                user.cv = candidatureDto.Cv ?? user.cv;
////                user.linkedIn = candidatureDto.LinkedIn ?? user.linkedIn;
////                user.github = candidatureDto.Github ?? user.github;
////                user.portfolio = candidatureDto.Portfolio ?? user.portfolio;
////                user.Statut = candidatureDto.UserStatut ?? user.Statut;

////                await _userManager.UpdateAsync(user);

////                // Mettre à jour la candidature  
////                candidature.MessageMotivation = candidatureDto.MessageMotivation;
////                candidature.Statut = candidatureDto.Statut ?? candidature.Statut;

////                // Gérer les expériences  
////                var existingExperiences = await _context.Experiences
////                    .Where(e => e.AppUserId == candidature.AppUserId)
////                    .ToListAsync();
////                _context.Experiences.RemoveRange(existingExperiences);

////                foreach (var expDto in candidatureDto.Experiences)
////                {
////                    var experience = new Experience
////                    {
////                        IdExperience = Guid.NewGuid(),
////                        AppUserId = candidatureDto.AppUserId,
////                        Poste = expDto.Poste,
////                        NomEntreprise = expDto.NomEntreprise,
////                        Description = expDto.Description,
////                        CompetenceAcquise = expDto.CompetenceAcquise,
////                        DateDebut = expDto.DateDebut,
////                        DateFin = expDto.DateFin,
////                        Certificats = new List<Certificat>()
////                    };

////                    foreach (var certDto in expDto.Certificats)
////                    {
////                        if (certDto.DateObtention.HasValue)
////                        {
////                            var certificat = new Certificat
////                            {
////                                IdCertificat = Guid.NewGuid(),
////                                ExperienceId = experience.IdExperience,
////                                Nom = certDto.Nom,
////                                DateObtention = certDto.DateObtention.Value,
////                                Organisme = certDto.Organisme,
////                                Description = certDto.Description,
////                                UrlDocument = certDto.UrlDocument
////                            };
////                            experience.Certificats.Add(certificat);
////                        }
////                    }

////                    _context.Experiences.Add(experience);
////                }

////                // Gérer les compétences  
////                var existingCompetences = await _context.AppUserCompetences
////                    .Where(cc => cc.AppUserId == candidatureDto.AppUserId)
////                    .ToListAsync();
////                _context.AppUserCompetences.RemoveRange(existingCompetences);

////                foreach (var compDto in candidatureDto.Competences)
////                {
////                    var competence = await _context.Competences
////                        .FirstOrDefaultAsync(c => c.Id == compDto.CompetenceId);
////                    if (competence == null)
////                    {
////                        return BadRequest(new { message = $"Compétence avec ID {compDto.CompetenceId} non trouvée." });
////                    }

////                    // Parse NiveauPossede as int  
////                    if (!int.TryParse(compDto.NiveauPossede, out var niveauPossede) || niveauPossede < 1 || niveauPossede > 4)
////                    {
////                        return BadRequest(new { message = $"NiveauPossede '{compDto.NiveauPossede}' est invalide. Valeurs acceptées : 1 (Débutant), 2 (Intermédiaire), 3 (Avancé), 4 (Expert)." });
////                    }

////                    var candidateCompetence = new AppUserCompetence
////                    {
////                        Id = Guid.NewGuid(),
////                        AppUserId = candidatureDto.AppUserId,
////                        CompetenceId = compDto.CompetenceId,
////                        NiveauPossede = (NiveauPossedeType)niveauPossede
////                    };
////                    _context.AppUserCompetences.Add(candidateCompetence);
////                }

////                await _context.SaveChangesAsync();

////                return NoContent();
////            }
////            catch (Exception ex)
////            {
////                return StatusCode(500, new { message = "Une erreur est survenue lors de la mise à jour de la candidature.", details = ex.Message });
////            }
////        }

////        [HttpPatch("{id}/status")]
////        [AllowAnonymous]
////        public async Task<IActionResult> SetCandidatureStatus(Guid id, [FromBody] string statut)
////        {
////            var candidature = await _context.Candidatures.FindAsync(id);
////            if (candidature == null)
////            {
////                return NotFound(new { message = "Candidature introuvable." });
////            }

////            candidature.Statut = statut;
////            await _context.SaveChangesAsync();

////            return NoContent();
////        }
////        [HttpDelete("{id}")]
////        [AllowAnonymous]
////        public async Task<IActionResult> DeleteCandidature(Guid id)
////        {
////            var candidature = await _context.Candidatures.FindAsync(id);
////            if (candidature == null)
////            {
////                return NotFound(new { message = "Candidature introuvable." });
////            }

////            _context.Candidatures.Remove(candidature);
////            await _context.SaveChangesAsync();

////            return NoContent();
////        }

////        [HttpGet("download-cv/{userId}")]
////        [AllowAnonymous]
////        public async Task<IActionResult> DownloadCandidateCV(Guid userId)
////        {
////            var user = await _userManager.FindByIdAsync(userId.ToString());
////            if (user == null)
////            {
////                return NotFound(new { message = "Utilisateur introuvable." });
////            }

////            if (string.IsNullOrEmpty(user.cv))
////            {
////                return BadRequest(new { message = "Aucun CV disponible pour cet utilisateur." });
////            }

////            // Handle local file path
////            if (System.IO.File.Exists(user.cv))
////            {
////                var fileBytes = await System.IO.File.ReadAllBytesAsync(user.cv);
////                var fileName = Path.GetFileName(user.cv);
////                return File(fileBytes, "application/pdf", fileName); // Use "application/pdf" for PDFs
////            }

////            // Handle Google Drive URLs
////            if (Uri.IsWellFormedUriString(user.cv, UriKind.Absolute))
////            {
////                var uri = new Uri(user.cv);
////                if (uri.Host.Contains("drive.google.com"))
////                {
////                    // Extract file ID from Google Drive URL
////                    var fileId = "";
////                    if (uri.AbsolutePath.Contains("/file/d/"))
////                    {
////                        var segments = uri.AbsolutePath.Split('/');
////                        var index = Array.IndexOf(segments, "d");
////                        if (index + 1 < segments.Length)
////                        {
////                            fileId = segments[index + 1];
////                        }
////                    }

////                    if (!string.IsNullOrEmpty(fileId))
////                    {
////                        // Construct direct download URL
////                        var downloadUrl = $"https://drive.usercontent.google.com/uc?id={fileId}&export=download";
////                        return Redirect(downloadUrl);
////                    }
////                    else
////                    {
////                        // Fallback to redirecting to the original URL
////                        return Redirect(user.cv);
////                    }
////                }
////                // Handle other URLs
////                return Redirect(user.cv);
////            }

////            return BadRequest(new { message = "Format de CV non pris en charge." });
////        }

////        private CandidatureResponseDto MapToResponseDto(Candidature candidature)
////        {
////            return new CandidatureResponseDto
////            {
////                IdCandidature = candidature.IdCandidature,
////                OffreId = candidature.OffreId,
////                OffreTitre = candidature.Offre?.Titre,
////                MessageMotivation = candidature.MessageMotivation,
////                Statut = candidature.Statut,
////                AppUserId = candidature.AppUserId,
////                AppUser = new AppUserResponseDto
////                {
////                    FullName = candidature.AppUser.FullName,
////                    Nom = candidature.AppUser.Nom,
////                    Prenom = candidature.AppUser.Prenom,
////                    DateNaissance = candidature.AppUser.DateNaissance?.ToString("yyyy-MM-dd"),
////                    Adresse = candidature.AppUser.Adresse,
////                    Ville = candidature.AppUser.Ville,
////                    Pays = candidature.AppUser.Pays,
////                    Phone = candidature.AppUser.phone,
////                    NiveauEtude = candidature.AppUser.NiveauEtude,
////                    Diplome = candidature.AppUser.Diplome,
////                    Universite = candidature.AppUser.Universite,
////                    Specialite = candidature.AppUser.specialite,
////                    Cv = candidature.AppUser.cv,
////                    LinkedIn = candidature.AppUser.linkedIn,
////                    Github = candidature.AppUser.github,
////                    Portfolio = candidature.AppUser.portfolio,
////                    Statut = candidature.AppUser.Statut
////                },
////                Experiences = candidature.AppUser.Experiences.Select(e => new ExperienceResponseDto
////                {
////                    Poste = e.Poste,
////                    NomEntreprise = e.NomEntreprise,
////                    Description = e.Description,
////                    CompetenceAcquise = e.CompetenceAcquise,
////                    DateDebut = e.DateDebut,
////                    DateFin = e.DateFin,
////                    Certificats = e.Certificats.Select(c => new CertificatDto
////                    {
////                        Nom = c.Nom,
////                        DateObtention = c.DateObtention,
////                        Organisme = c.Organisme,
////                        Description = c.Description,
////                        UrlDocument = c.UrlDocument
////                    }).ToList()
////                }).ToList(),
////                Competences = candidature.AppUser.AppUserCompetences.Select(cc => new CandidateCompetenceResponseDto
////                {
////                    CompetenceId = cc.CompetenceId,
////                    CompetenceNom = cc.Competence.Nom,
////                    NiveauPossede = cc.NiveauPossede switch
////                    {
////                        NiveauPossedeType.Debutant => "Débutant",
////                        NiveauPossedeType.Intermediaire => "Intermédiaire",
////                        NiveauPossedeType.Avance => "Avancé",
////                        NiveauPossedeType.Expert => "Expert",
////                        _ => "Inconnu"
////                    }
////                }).ToList(),
////                DateSoumission = candidature.DateSoumission
////            };
////        }
////    }

////    public class ExperienceDto
////    {
////        public string Poste { get; set; }
////        public string NomEntreprise { get; set; }
////        public string Description { get; set; }
////        public string CompetenceAcquise { get; set; }
////        public DateTime? DateDebut { get; set; }
////        public DateTime? DateFin { get; set; }
////        public List<CertificatDto> Certificats { get; set; } = new List<CertificatDto>();
////    }

////    public class CertificatDto
////    {
////        public string Nom { get; set; }
////        public DateTime? DateObtention { get; set; }
////        public string Organisme { get; set; }
////        public string Description { get; set; }
////        public string UrlDocument { get; set; }
////    }

////    public class CandidateCompetenceDto
////    {
////        [Required(ErrorMessage = "Le champ CompetenceId est requis.")]
////        public Guid CompetenceId { get; set; }

////        [Required(ErrorMessage = "Le champ NiveauPossede est requis.")]
////        [MaxLength(20, ErrorMessage = "Le champ NiveauPossede ne peut pas dépasser 20 caractères.")]
////        public string NiveauPossede { get; set; }
////    }

////    public class CandidatureResponseDto
////    {
////        public Guid IdCandidature { get; set; }
////        public Guid OffreId { get; set; }
////        public string OffreTitre { get; set; }
////        public string MessageMotivation { get; set; }
////        public string Statut { get; set; }
////        public Guid AppUserId { get; set; }
////        public AppUserResponseDto AppUser { get; set; }
////        public List<ExperienceResponseDto> Experiences { get; set; }
////        public List<CandidateCompetenceResponseDto> Competences { get; set; }
////        public DateTime DateSoumission { get; set; }
////    }

////    public class AppUserResponseDto
////    {
////        public string FullName { get; set; }
////        public string Nom { get; set; }
////        public string Prenom { get; set; }
////        public string DateNaissance { get; set; }
////        public string Adresse { get; set; }
////        public string Ville { get; set; }
////        public string Pays { get; set; }
////        public string Phone { get; set; }
////        public string NiveauEtude { get; set; }
////        public string Diplome { get; set; }
////        public string Universite { get; set; }
////        public string Specialite { get; set; }
////        public string Cv { get; set; }
////        public string LinkedIn { get; set; }
////        public string Github { get; set; }
////        public string Portfolio { get; set; }
////        public string Statut { get; set; }
////    }

////    public class ExperienceResponseDto
////    {
////        public string Poste { get; set; }
////        public string NomEntreprise { get; set; }
////        public string Description { get; set; }
////        public string CompetenceAcquise { get; set; }
////        public DateTime? DateDebut { get; set; }
////        public DateTime? DateFin { get; set; }
////        public List<CertificatDto> Certificats { get; set; }
////    }

////    public class CandidateCompetenceResponseDto
////    {
////        public Guid CompetenceId { get; set; }
////        public string CompetenceNom { get; set; }
////        public string NiveauPossede { get; set; }
////    }

////    public class CandidatureFormDto
////    {
////        public Guid IdCandidature { get; set; }
////        public Guid AppUserId { get; set; }
////        public Guid OffreId { get; set; }
////        public string FullName { get; set; }
////        public string Nom { get; set; }
////        public string Prenom { get; set; }
////        public DateTime? DateNaissance { get; set; }
////        public string Adresse { get; set; }
////        public string Ville { get; set; }
////        public string Pays { get; set; }
////        public string Phone { get; set; }
////        public string NiveauEtude { get; set; }
////        public string Diplome { get; set; }
////        public string Universite { get; set; }
////        public string Specialite { get; set; }
////        public string Cv { get; set; }
////        public string LinkedIn { get; set; }
////        public string Github { get; set; }
////        public string Portfolio { get; set; }
////        public string UserStatut { get; set; }
////        public string Statut { get; set; }
////        public string MessageMotivation { get; set; }
////        public List<ExperienceDto> Experiences { get; set; } = new List<ExperienceDto>();
////        public List<CandidateCompetenceDto> Competences { get; set; } = new List<CandidateCompetenceDto>();
////    }
////}

//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Backend_poulina_future_jobs.Models;
//using Microsoft.AspNetCore.Authorization;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Identity;

//namespace Backend_poulina_future_jobs.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class CandidaturesController : ControllerBase
//    {
//        private readonly AppDbContext _context;
//        private readonly UserManager<AppUser> _userManager;

//        public CandidaturesController(AppDbContext context, UserManager<AppUser> userManager)
//        {
//            _context = context;
//            _userManager = userManager;
//        }

//        [HttpGet]
//        [AllowAnonymous]
//        public async Task<ActionResult<IEnumerable<CandidatureResponseDto>>> GetAll()
//        {
//            try
//            {
//                var candidatures = await _context.Candidatures
//                    .Include(c => c.AppUser)
//                    .Include(c => c.Offre)
//                    .ToListAsync();

//                return Ok(candidatures.Select(c => MapToResponseDto(c)));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { error = $"Erreur interne: {ex.Message}" });
//            }
//        }

//        [HttpGet("{id}")]
//        [AllowAnonymous]
//        public async Task<ActionResult<CandidatureResponseDto>> GetById(Guid id)
//        {
//            try
//            {
//                var candidature = await _context.Candidatures
//                    .Include(c => c.AppUser)
//                    .Include(c => c.Offre)
//                    .FirstOrDefaultAsync(c => c.IdCandidature == id);

//                if (candidature == null) return NotFound();

//                return Ok(MapToResponseDto(candidature));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { error = $"Erreur interne: {ex.Message}" });
//            }
//        }

//        [HttpDelete("{id}")]
//        [Authorize(Roles = "Admin,Recruteur")]
//        public async Task<IActionResult> Delete(Guid id)
//        {
//            try
//            {
//                var candidature = await _context.Candidatures.FindAsync(id);
//                if (candidature == null) return NotFound();

//                _context.Candidatures.Remove(candidature);
//                await _context.SaveChangesAsync();

//                return NoContent();
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { error = $"Erreur interne: {ex.Message}" });
//            }
//        }

//        [HttpPatch("{id}/status")]
//        [Authorize(Roles = "Recruteur")]
//        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string statut)
//        {
//            try
//            {
//                var candidature = await _context.Candidatures.FindAsync(id);
//                if (candidature == null) return NotFound();

//                candidature.Statut = statut;
//                await _context.SaveChangesAsync();

//                return NoContent();
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { error = $"Erreur interne: {ex.Message}" });
//            }
//        }

//        [HttpGet("offre/{offreId}")]
//        [AllowAnonymous]
//        public async Task<ActionResult<IEnumerable<CandidatureResponseDto>>> GetByOffre(Guid offreId)
//        {
//            try
//            {
//                var candidatures = await _context.Candidatures
//                    .Where(c => c.OffreId == offreId)
//                    .Include(c => c.AppUser)
//                    .ToListAsync();

//                return Ok(candidatures.Select(c => MapToResponseDto(c)));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { error = $"Erreur interne: {ex.Message}" });
//            }
//        }

//        private CandidatureResponseDto MapToResponseDto(Candidature c)
//        {
//            return new CandidatureResponseDto
//            {
//                IdCandidature = c.IdCandidature,
//                AppUserId = c.AppUserId,
//                OffreId = c.OffreId,
//                Statut = c.Statut,
//                MessageMotivation = c.MessageMotivation,
//                DateSoumission = c.DateSoumission,
//                CV = c.CvFilePath,
//                LettreMotivation = c.LettreMotivation
//            };
//        }
//    }
//}

