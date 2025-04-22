using Backend_poulina_future_jobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend_poulina_future_jobs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CandidatureController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CandidatureController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetAllCandidatures()
        {
            var candidatures = await _context.Candidatures
                .Include(c => c.AppUser)
                .Include(c => c.Offre)
                .Include(c => c.AppUser).ThenInclude(u => u.Experiences).ThenInclude(e => e.Certificats)
                .Include(c => c.AppUser).ThenInclude(u => u.AppUserCompetences).ThenInclude(cc => cc.Competence)
                .ToListAsync();

            var response = candidatures.Select(c => MapToResponseDto(c)).ToList();
            return Ok(response);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCandidature(Guid id)
        {
            var candidature = await _context.Candidatures
                .Include(c => c.AppUser)
                .Include(c => c.Offre)
                .Include(c => c.AppUser).ThenInclude(u => u.Experiences).ThenInclude(e => e.Certificats)
                .Include(c => c.AppUser).ThenInclude(u => u.AppUserCompetences).ThenInclude(cc => cc.Competence)
                .FirstOrDefaultAsync(c => c.IdCandidature == id);

            if (candidature == null)
            {
                return NotFound("Candidature introuvable.");
            }

            return Ok(MapToResponseDto(candidature));
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateCandidature([FromBody] CandidatureDto candidatureDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirstValue("userId");
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                userId = candidatureDto.AppUserId;
            }

            var user = await _userManager.FindByIdAsync(candidatureDto.AppUserId.ToString());
            var offre = await _context.OffresEmploi.FirstOrDefaultAsync(o => o.IdOffreEmploi == candidatureDto.OffreId);

            if (user == null)
            {
                return BadRequest("Utilisateur introuvable.");
            }

            if (offre == null)
            {
                return BadRequest("Offre introuvable.");
            }

            var existingCandidature = await _context.Candidatures
                .AnyAsync(c => c.AppUserId == candidatureDto.AppUserId && c.OffreId == candidatureDto.OffreId);
            if (existingCandidature)
            {
                return BadRequest("Une candidature existe déjà pour cette offre.");
            }

            var offreCompetences = await _context.OffreCompetences
                .Where(oc => oc.IdOffreEmploi == candidatureDto.OffreId)
                .Select(oc => oc.Competence.Nom.ToLower())
                .ToListAsync();

            var candidatCompetenceNames = candidatureDto.Competences
                .Select(c => c.CompetenceNom.ToLower())
                .ToList();

            var missingCompetences = offreCompetences
                .Except(candidatCompetenceNames)
                .ToList();

            if (missingCompetences.Any())
            {
                return BadRequest(new
                {
                    message = "Le candidat ne possède pas toutes les compétences requises.",
                    competencesManquantes = missingCompetences
                });
            }

            user.FullName = candidatureDto.FullName ?? user.FullName;
            user.Nom = candidatureDto.Nom ?? user.Nom;
            user.Prenom = candidatureDto.Prenom ?? user.Prenom;
            user.Photo = candidatureDto.Photo ?? user.Photo;
            user.DateNaissance = candidatureDto.DateNaissance ?? user.DateNaissance;
            user.Adresse = candidatureDto.Adresse ?? user.Adresse;
            user.Ville = candidatureDto.Ville ?? user.Ville;
            user.Pays = candidatureDto.Pays ?? user.Pays;
            user.phone = candidatureDto.Phone ?? user.phone;
            user.NiveauEtude = candidatureDto.NiveauEtude;
            user.Diplome = candidatureDto.Diplome;
            user.Universite = candidatureDto.Universite;
            user.specialite = candidatureDto.Specialite;
            user.cv = candidatureDto.Cv;
            user.linkedIn = candidatureDto.LinkedIn;
            user.github = candidatureDto.Github;
            user.portfolio = candidatureDto.Portfolio;
            user.Entreprise = candidatureDto.Entreprise ?? user.Entreprise;
            user.Poste = candidatureDto.Poste ?? user.Poste;
            user.Statut = candidatureDto.UserStatut ?? user.Statut;

            await _userManager.UpdateAsync(user);

            var candidature = new Candidature
            {
                IdCandidature = Guid.NewGuid(),
                AppUserId = candidatureDto.AppUserId,
                OffreId = candidatureDto.OffreId,
                Statut = candidatureDto.Statut ?? "Soumise",
                MessageMotivation = candidatureDto.MessageMotivation,
                DateSoumission = DateTime.UtcNow
            };

            _context.Candidatures.Add(candidature);

            var existingExperiences = await _context.Experiences
                .Where(e => e.AppUserId == candidatureDto.AppUserId)
                .ToListAsync();

            if (existingExperiences.Any())
            {
                _context.Experiences.RemoveRange(existingExperiences);
            }

            foreach (var expDto in candidatureDto.Experiences)
            {
                var experience = new Experience
                {
                    IdExperience = Guid.NewGuid(),
                    AppUserId = candidatureDto.AppUserId,
                    Poste = expDto.Poste,
                    NomEntreprise = expDto.NomEntreprise,
                    Description = expDto.Description,
                    CompetenceAcquise = expDto.CompetenceAcquise,
                    DateDebut = expDto.DateDebut,
                    DateFin = expDto.DateFin
                };

                foreach (var certDto in expDto.Certificats)
                {
                    var certificat = new Certificat
                    {
                        IdCertificat = Guid.NewGuid(),
                        ExperienceId = experience.IdExperience,
                        Nom = certDto.Nom,
                        DateObtention = certDto.DateObtention,
                        Organisme = certDto.Organisme,
                        Description = certDto.Description,
                        UrlDocument = certDto.UrlDocument
                    };
                    experience.Certificats.Add(certificat);
                }

                _context.Experiences.Add(experience);
            }

            var existingCompetences = await _context.AppUserCompetences
                .Where(cc => cc.AppUserId == candidatureDto.AppUserId)
                .ToListAsync();

            if (existingCompetences.Any())
            {
                _context.AppUserCompetences.RemoveRange(existingCompetences);
            }

            foreach (var compDto in candidatureDto.Competences)
            {
                Console.WriteLine($"CompetenceNom: {compDto.CompetenceNom}, NiveauPossede: {compDto.NiveauPossede ?? "null"}");
                var competence = await _context.Competences
                    .FirstOrDefaultAsync(c => c.Nom == compDto.CompetenceNom);

                if (competence == null)
                {
                    competence = new Competence
                    {
                        Id = Guid.NewGuid(),
                        Nom = compDto.CompetenceNom,
                        Description = "Added during candidature",
                        estTechnique = true,
                        estSoftSkill = false
                    };
                    _context.Competences.Add(competence);
                    await _context.SaveChangesAsync();
                }

                var candidateCompetence = new candiadate_competence
                {
                    Id = Guid.NewGuid(),
                    AppUserId = candidatureDto.AppUserId,
                    CompetenceId = competence.Id,
                    NiveauPossede = compDto.NiveauPossede
                };
                _context.AppUserCompetences.Add(candidateCompetence);
            }

            await _context.SaveChangesAsync();

            var response = MapToResponseDto(candidature);
            return CreatedAtAction(nameof(GetCandidature), new { id = candidature.IdCandidature }, response);
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateCandidature(Guid id, [FromBody] CandidatureDto candidatureDto)
        {
            if (id != candidatureDto.IdCandidature)
            {
                return BadRequest("L'ID de la candidature ne correspond pas.");
            }

            var userIdClaim = User.FindFirstValue("userId");
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                userId = candidatureDto.AppUserId;
            }

            var candidature = await _context.Candidatures
                .Include(c => c.AppUser)
                .Include(c => c.Offre)
                .FirstOrDefaultAsync(c => c.IdCandidature == id);

            if (candidature == null)
            {
                return NotFound("Candidature introuvable.");
            }

            var user = await _userManager.FindByIdAsync(candidatureDto.AppUserId.ToString());
            if (user == null)
            {
                return BadRequest("Utilisateur introuvable.");
            }

            user.FullName = candidatureDto.FullName ?? user.FullName;
            user.Nom = candidatureDto.Nom ?? user.Nom;
            user.Prenom = candidatureDto.Prenom ?? user.Prenom;
            user.Photo = candidatureDto.Photo ?? user.Photo;
            user.DateNaissance = candidatureDto.DateNaissance ?? user.DateNaissance;
            user.Adresse = candidatureDto.Adresse ?? user.Adresse;
            user.Ville = candidatureDto.Ville ?? user.Ville;
            user.Pays = candidatureDto.Pays ?? user.Pays;
            user.phone = candidatureDto.Phone ?? user.phone;
            user.NiveauEtude = candidatureDto.NiveauEtude;
            user.Diplome = candidatureDto.Diplome;
            user.Universite = candidatureDto.Universite;
            user.specialite = candidatureDto.Specialite;
            user.cv = candidatureDto.Cv;
            user.linkedIn = candidatureDto.LinkedIn;
            user.github = candidatureDto.Github;
            user.portfolio = candidatureDto.Portfolio;
            user.Entreprise = candidatureDto.Entreprise ?? user.Entreprise;
            user.Poste = candidatureDto.Poste ?? user.Poste;
            user.Statut = candidatureDto.UserStatut ?? user.Statut;

            await _userManager.UpdateAsync(user);

            candidature.MessageMotivation = candidatureDto.MessageMotivation;
            candidature.Statut = candidatureDto.Statut ?? candidature.Statut;

            var existingExperiences = await _context.Experiences
                .Where(e => e.AppUserId == candidature.AppUserId)
                .ToListAsync();
            _context.Experiences.RemoveRange(existingExperiences);

            foreach (var expDto in candidatureDto.Experiences)
            {
                var experience = new Experience
                {
                    IdExperience = Guid.NewGuid(),
                    AppUserId = candidatureDto.AppUserId,
                    Poste = expDto.Poste,
                    NomEntreprise = expDto.NomEntreprise,
                    Description = expDto.Description,
                    CompetenceAcquise = expDto.CompetenceAcquise,
                    DateDebut = expDto.DateDebut,
                    DateFin = expDto.DateFin
                };

                foreach (var certDto in expDto.Certificats)
                {
                    var certificat = new Certificat
                    {
                        IdCertificat = Guid.NewGuid(),
                        ExperienceId = experience.IdExperience,
                        Nom = certDto.Nom,
                        DateObtention = certDto.DateObtention,
                        Organisme = certDto.Organisme,
                        Description = certDto.Description,
                        UrlDocument = certDto.UrlDocument
                    };
                    experience.Certificats.Add(certificat);
                }

                _context.Experiences.Add(experience);
            }

            var existingCompetences = await _context.AppUserCompetences
                .Where(cc => cc.AppUserId == candidatureDto.AppUserId)
                .ToListAsync();
            _context.AppUserCompetences.RemoveRange(existingCompetences);

            foreach (var compDto in candidatureDto.Competences)
            {
                Console.WriteLine($"CompetenceNom: {compDto.CompetenceNom}, NiveauPossede: {compDto.NiveauPossede ?? "null"}");
                var competence = await _context.Competences
                    .FirstOrDefaultAsync(c => c.Nom == compDto.CompetenceNom);
                if (competence == null)
                {
                    competence = new Competence
                    {
                        Id = Guid.NewGuid(),
                        Nom = compDto.CompetenceNom,
                        Description = "Added during candidature update",
                        estTechnique = true,
                        estSoftSkill = false
                    };
                    _context.Competences.Add(competence);
                    await _context.SaveChangesAsync();
                }
                var candidateCompetence = new candiadate_competence
                {
                    Id = Guid.NewGuid(),
                    AppUserId = candidatureDto.AppUserId,
                    CompetenceId = competence.Id,
                    NiveauPossede = compDto.NiveauPossede
                };
                _context.AppUserCompetences.Add(candidateCompetence);
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> SetCandidatureStatus(Guid id, [FromBody] string statut)
        {
            var candidature = await _context.Candidatures.FindAsync(id);
            if (candidature == null)
            {
                return NotFound("Candidature introuvable.");
            }

            candidature.Statut = statut;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private CandidatureResponseDto MapToResponseDto(Candidature candidature)
        {
            return new CandidatureResponseDto
            {
                IdCandidature = candidature.IdCandidature,
                OffreId = candidature.OffreId,
                OffreTitre = candidature.Offre?.Titre,
                MessageMotivation = candidature.MessageMotivation,
                Statut = candidature.Statut,
                AppUserId = candidature.AppUserId,
                AppUser = new AppUserResponseDto
                {
                    FullName = candidature.AppUser.FullName,
                    Nom = candidature.AppUser.Nom,
                    Prenom = candidature.AppUser.Prenom,
                    Photo = candidature.AppUser.Photo,
                    DateNaissance = candidature.AppUser.DateNaissance?.ToString("yyyy-MM-dd"),
                    Adresse = candidature.AppUser.Adresse,
                    Ville = candidature.AppUser.Ville,
                    Pays = candidature.AppUser.Pays,
                    Phone = candidature.AppUser.phone,
                    NiveauEtude = candidature.AppUser.NiveauEtude,
                    Diplome = candidature.AppUser.Diplome,
                    Universite = candidature.AppUser.Universite,
                    Specialite = candidature.AppUser.specialite,
                    Cv = candidature.AppUser.cv,
                    LinkedIn = candidature.AppUser.linkedIn,
                    Github = candidature.AppUser.github,
                    Portfolio = candidature.AppUser.portfolio,
                    Entreprise = candidature.AppUser.Entreprise,
                    Poste = candidature.AppUser.Poste,
                    Statut = candidature.AppUser.Statut
                },
                Experiences = candidature.AppUser.Experiences.Select(e => new ExperienceResponseDto
                {
                    Poste = e.Poste,
                    NomEntreprise = e.NomEntreprise,
                    Description = e.Description,
                    CompetenceAcquise = e.CompetenceAcquise,
                    DateDebut = e.DateDebut,
                    DateFin = e.DateFin,
                    Certificats = e.Certificats.Select(c => new CertificatDto
                    {
                        Nom = c.Nom,
                        DateObtention = c.DateObtention,
                        Organisme = c.Organisme,
                        Description = c.Description,
                        UrlDocument = c.UrlDocument
                    }).ToList()
                }).ToList(),
                Competences = candidature.AppUser.AppUserCompetences.Select(cc => new CandidateCompetenceResponseDto
                {
                    CompetenceId = cc.CompetenceId,
                    CompetenceNom = cc.Competence.Nom,
                    NiveauPossede = cc.NiveauPossede
                }).ToList(),
                DateSoumission = candidature.DateSoumission
            };
        }
    }

    public class ExperienceDto
    {
        public string Poste { get; set; }
        public string NomEntreprise { get; set; }
        public string Description { get; set; }
        public string CompetenceAcquise { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public List<CertificatDto> Certificats { get; set; } = [];
    }

    public class CertificatDto
    {
        public string Nom { get; set; }
        public DateTime DateObtention { get; set; }
        public string Organisme { get; set; }
        public string Description { get; set; }
        public string UrlDocument { get; set; }
    }

    public class CandidateCompetenceDto
    {
        [Required(ErrorMessage = "Le champ CompetenceNom est requis.")]
        public string CompetenceNom { get; set; }

        [Required(ErrorMessage = "Le champ NiveauPossede est requis.")]
        [MaxLength(20, ErrorMessage = "Le champ NiveauPossede ne peut pas dépasser 20 caractères.")]
        public string NiveauPossede { get; set; }
    }

    public class CandidatureResponseDto
    {
        public Guid IdCandidature { get; set; }
        public Guid OffreId { get; set; }
        public string OffreTitre { get; set; }
        public string MessageMotivation { get; set; }
        public string Statut { get; set; }
        public Guid AppUserId { get; set; }
        public AppUserResponseDto AppUser { get; set; }
        public List<ExperienceResponseDto> Experiences { get; set; }
        public List<CandidateCompetenceResponseDto> Competences { get; set; }
        public DateTime DateSoumission { get; set; }
    }

    public class AppUserResponseDto
    {
        public string FullName { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Photo { get; set; }
        public string DateNaissance { get; set; }
        public string Adresse { get; set; }
        public string Ville { get; set; }
        public string Pays { get; set; }
        public string Phone { get; set; }
        public string NiveauEtude { get; set; }
        public string Diplome { get; set; }
        public string Universite { get; set; }
        public string Specialite { get; set; }
        public string Cv { get; set; }
        public string LinkedIn { get; set; }
        public string Github { get; set; }
        public string Portfolio { get; set; }
        public string Entreprise { get; set; }
        public string Poste { get; set; }
        public string Statut { get; set; }
    }

    public class ExperienceResponseDto
    {
        public string Poste { get; set; }
        public string NomEntreprise { get; set; }
        public string Description { get; set; }
        public string CompetenceAcquise { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public List<CertificatDto> Certificats { get; set; }
    }

    public class CandidateCompetenceResponseDto
    {
        public Guid CompetenceId { get; set; }
        public string CompetenceNom { get; set; }
        public string NiveauPossede { get; set; }
    }

    public class CandidatureDto
    {
        public Guid IdCandidature { get; set; }
        public Guid AppUserId { get; set; }
        public Guid OffreId { get; set; }
        public string FullName { get; set; }
        public string Nom { get; set; }
        public string Prenom { get; set; }
        public string Photo { get; set; }
        public DateTime? DateNaissance { get; set; }
        public string Adresse { get; set; }
        public string Ville { get; set; }
        public string Pays { get; set; }
        public string Phone { get; set; }
        public string NiveauEtude { get; set; }
        public string Diplome { get; set; }
        public string Universite { get; set; }
        public string Specialite { get; set; }
        public string Cv { get; set; }
        public string LinkedIn { get; set; }
        public string Github { get; set; }
        public string Portfolio { get; set; }
        public string Entreprise { get; set; }
        public string Poste { get; set; }
        public string UserStatut { get; set; }
        public string Statut { get; set; }
        public string MessageMotivation { get; set; }
        public List<ExperienceDto> Experiences { get; set; } = [];
        public List<CandidateCompetenceDto> Competences { get; set; } = [];
    }
}