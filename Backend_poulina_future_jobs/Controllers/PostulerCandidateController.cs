//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Backend_poulina_future_jobs.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using System.ComponentModel.DataAnnotations;
//using System.Security.Claims;
//using Microsoft.Data.SqlClient;

//namespace Backend_poulina_future_jobs.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class PostulerCandidateController : ControllerBase
//    {
//        private readonly AppDbContext _context;
//        private readonly UserManager<AppUser> _userManager;

//        public PostulerCandidateController(AppDbContext context, UserManager<AppUser> userManager)
//        {
//            _context = context ?? throw new ArgumentNullException(nameof(context));
//            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
//        }

//        // POST: api/PostulerCandidate/Soumettre
//        [HttpPost]
//        [Route("Soumettre")]
//        // POST: api/PostulerCandidate/Soumettre

//        public async Task<IActionResult> SoumettreCandidature([FromBody] CandidatureInputDto model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            var user = await _context.Users.FindAsync(model.AppUserId);
//            var offre = await _context.OffresEmploi.FindAsync(model.OffreId);
//            if (user == null || offre == null)
//            {
//                return NotFound("Utilisateur ou offre non trouvés.");
//            }

//            // Check for existing candidature
//            var existingCandidature = await _context.Candidatures.AnyAsync(
//                c => c.AppUserId == model.AppUserId && c.OffreId == model.OffreId);
//            if (existingCandidature)
//            {
//                return BadRequest("Vous avez déjà postulé pour cette offre.");
//            }

//            var candidature = new Candidature
//            {
//                IdCandidature = Guid.NewGuid(),
//                AppUserId = model.AppUserId,
//                OffreId = model.OffreId,
//                Statut = "En cours",
//                MessageMotivation = model.MessageMotivation,
//                DateSoumission = DateTime.UtcNow
//            };

//            _context.Candidatures.Add(candidature);
//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateException ex)
//            {
//                if (ex.InnerException is SqlException sqlException && (sqlException.Number == 2601 || sqlException.Number == 2627)) // SQL Server error codes for unique index violation
//                {
//                    return BadRequest("Vous avez déjà postulé pour cette offre.");
//                }
//                else
//                {
//                    // Handle other database errors
//                    return StatusCode(500, "An error occurred while saving the application.");
//                }
//            }

//            return Ok(new { Message = "Candidature soumise avec succès.", CandidatureId = candidature.IdCandidature });
//        }
//        // POST: api/PostulerCandidate (Apply for a job)

//        [HttpPost]
//        [Authorize(Roles = "Candidate")]
//        public async Task<ActionResult<CandidatureDto>> PostCandidature([FromBody] CandidatureInputDto input)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
//            if (string.IsNullOrEmpty(userIdClaim))
//            {
//                return Unauthorized("User ID not found in token.");
//            }

//            if (!Guid.TryParse(userIdClaim, out var userId))
//            {
//                return Unauthorized("Invalid User ID in token.");
//            }

//            if (input.AppUserId != userId)
//            {
//                return Unauthorized("Vous ne pouvez postuler que pour vous-même.");
//            }

//            var offre = await _context.OffresEmploi.FindAsync(input.OffreId);
//            if (offre == null || offre.Statut != StatutOffre.Ouvert)
//            {
//                return BadRequest("L'offre n'existe pas ou est clôturée.");
//            }

//            var user = await _context.Users.FindAsync(input.AppUserId);
//            if (user == null)
//            {
//                return BadRequest("L'utilisateur n'existe pas.");
//            }

//            // Check for existing candidature
//            var existingCandidature = await _context.Candidatures
//                .AnyAsync(c => c.AppUserId == input.AppUserId && c.OffreId == input.OffreId);
//            if (existingCandidature)
//            {
//                return BadRequest("Vous avez déjà postulé pour cette offre.");
//            }

//            // Mettre à jour les propriétés de l'utilisateur si nécessaire
//            _context.Entry(user).State = EntityState.Modified;

//            var candidature = new Candidature
//            {
//                IdCandidature = Guid.NewGuid(),
//                AppUserId = input.AppUserId,
//                OffreId = input.OffreId,
//                Statut = "Soumise",
//                MessageMotivation = input.MessageMotivation,
//                DateSoumission = DateTime.UtcNow
//            };

//            _context.Candidatures.Add(candidature);
//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateException ex)
//            {
//                if (ex.InnerException is SqlException sqlException && (sqlException.Number == 2601 || sqlException.Number == 2627))
//                {
//                    return BadRequest("Vous avez déjà postulé pour cette offre.");
//                }
//                else
//                {
//                    return StatusCode(500, "Une erreur s'est produite lors de l'enregistrement de la candidature.");
//                }
//            }

//            // Récupérer la candidature complète avec toutes les relations
//            var savedCandidature = await _context.Candidatures
//                .Include(c => c.AppUser)
//                    .ThenInclude(u => u.Experiences)
//                .Include(c => c.AppUser)
//                    .ThenInclude(u => u.AppUserCompetences)
//                    .ThenInclude(cc => cc.Competence)
//                .Include(c => c.AppUser)
//                    .ThenInclude(u => u.Certificats)
//                .Include(c => c.AppUser)
//                    .ThenInclude(u => u.DiplomesCandidate)
//                .Include(c => c.Offre)
//                .FirstOrDefaultAsync(c => c.IdCandidature == candidature.IdCandidature);

//            if (savedCandidature == null)
//            {
//                return StatusCode(500, "Impossible de récupérer la candidature enregistrée.");
//            }

//            var resultDto = new CandidatureDto
//            {
//                IdCandidature = savedCandidature.IdCandidature,
//                AppUserId = savedCandidature.AppUserId,
//                OffreId = savedCandidature.OffreId,
//                Statut = savedCandidature.Statut,
//                MessageMotivation = savedCandidature.MessageMotivation,
//                DateSoumission = savedCandidature.DateSoumission,
//                AppUser = savedCandidature.AppUser != null ? new AppUserDto
//                {
//                    Id = savedCandidature.AppUser.Id,
//                    FullName = savedCandidature.AppUser.FullName,
//                    Nom = savedCandidature.AppUser.Nom,
//                    Prenom = savedCandidature.AppUser.Prenom,
//                    Email = savedCandidature.AppUser.Email,
//                    Phone = savedCandidature.AppUser.phone,
//                    NiveauEtude = savedCandidature.AppUser.NiveauEtude,
//                    Diplome = savedCandidature.AppUser.Diplome,
//                    Universite = savedCandidature.AppUser.Universite,
//                    Specialite = savedCandidature.AppUser.specialite,
//                    Cv = savedCandidature.AppUser.cv,
//                    LinkedIn = savedCandidature.AppUser.linkedIn,
//                    Github = savedCandidature.AppUser.github,
//                    Portfolio = savedCandidature.AppUser.portfolio,
//                    Statut = savedCandidature.AppUser.Statut,
//                    LettreMotivation = savedCandidature.AppUser.LettreMotivation,
//                    Experiences = savedCandidature.AppUser.Experiences?.Select(e => new ExperienceDto
//                    {
//                        IdExperience = e.IdExperience,
//                        Poste = e.Poste,
//                        Description = e.Description,
//                        NomEntreprise = e.NomEntreprise,
//                        CompetenceAcquise = e.CompetenceAcquise,
//                        DateDebut = e.DateDebut,
//                        DateFin = e.DateFin
//                    }).ToList() ?? new List<ExperienceDto>(),
//                    AppUserCompetences = savedCandidature.AppUser.AppUserCompetences?.Select(ac => new CandidateCompetenceDto
//                    {
//                        Id = ac.Id,
//                        CompetenceId = ac.CompetenceId,
//                        NiveauPossede = ac.NiveauPossede.ToString(),
//                        Competence = new CompetenceCandidateDto
//                        {
//                            Id = ac.Competence.Id,
//                            Nom = ac.Competence.Nom,
//                            Description = ac.Competence.Description,
//                            EstTechnique = ac.Competence.estTechnique,
//                            EstSoftSkill = ac.Competence.estSoftSkill
//                        }
//                    }).ToList() ?? new List<CandidateCompetenceDto>(),
//                    Certificats = savedCandidature.AppUser.Certificats?.Select(c => new CertificatDto
//                    {
//                        IdCertificat = c.IdCertificat,
//                        Nom = c.Nom,
//                        DateObtention = c.DateObtention,
//                        Organisme = c.Organisme,
//                        Description = c.Description,
//                        UrlDocument = c.UrlDocument
//                    }).ToList() ?? new List<CertificatDto>(),
//                    DiplomesCandidate = savedCandidature.AppUser.DiplomesCandidate?.Select(d => new DiplomeCandidateDto
//                    {
//                        IdDiplome = d.IdDiplome,
//                        AppUserId = d.AppUserId,
//                        NomDiplome = d.NomDiplome,
//                        Institution = d.Institution,
//                        DateObtention = d.DateObtention,
//                        Specialite = d.Specialite,
//                        UrlDocument = d.UrlDocument
//                    }).ToList() ?? new List<DiplomeCandidateDto>()
//                } : null,
//                Offre = savedCandidature.Offre != null ? new OffreEmploicandidateDto
//                {
//                    IdOffreEmploi = savedCandidature.Offre.IdOffreEmploi
//                } : null
//            };

//            return CreatedAtAction(nameof(GetCandidature), new { id = resultDto.IdCandidature }, resultDto);
//        }

//        // PUT: api/PostulerCandidate/{id} (Modify an existing candidature)

//        [HttpPut("{id}")]
//        [Authorize]
//        public async Task<IActionResult> PutCandidature(Guid id, [FromBody] CandidatureDto candidatureDto)
//        {
//            if (id != candidatureDto.IdCandidature)
//            {
//                return BadRequest("L'ID de la candidature ne correspond pas.");
//            }

//            var existingCandidature = await _context.Candidatures
//                .Include(c => c.AppUser)
//                .Include(c => c.Offre)
//                .FirstOrDefaultAsync(c => c.IdCandidature == id);

//            if (existingCandidature == null)
//            {
//                return NotFound("Candidature non trouvée.");
//            }

//            // Vérification de l'utilisateur actuel
//            if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
//            {
//                return Unauthorized("Invalid User ID in token.");
//            }

//            var isRecruteur = User.IsInRole("Recruteur");
//            if (existingCandidature.AppUserId != userId && !isRecruteur)
//            {
//                return Unauthorized("Vous n'êtes pas autorisé à modifier cette candidature.");
//            }

//            // Mise à jour des champs modifiables
//            existingCandidature.MessageMotivation = candidatureDto.MessageMotivation;
//            if (isRecruteur)
//            {
//                existingCandidature.Statut = candidatureDto.Statut;
//            }

//            // Mise à jour de l'utilisateur si c'est le candidat qui modifie sa candidature
//            if (existingCandidature.AppUserId == userId)
//            {
//                var user = await _context.Users.FindAsync(existingCandidature.AppUserId);
//                if (user != null)
//                {
//                    _context.Entry(user).State = EntityState.Modified;
//                }
//            }

//            _context.Entry(existingCandidature).State = EntityState.Modified;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!CandidatureExists(id))
//                {
//                    return NotFound("Candidature non trouvée.");
//                }
//                return StatusCode(500, "Erreur de concurrence lors de la mise à jour.");
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"Une erreur s'est produite : {ex.Message}");
//            }

//            return NoContent();
//        }

//        // GET: api/PostulerCandidate/{id} (Retrieve a candidature)
//        [HttpGet("{id}")]
//        [Authorize]
//        public async Task<ActionResult<CandidatureDto>> GetCandidature(Guid id)
//        {
//            var candidature = await _context.Candidatures
//                .Include(c => c.AppUser)
//                    .ThenInclude(u => u.Experiences)
//                .Include(c => c.AppUser)
//                    .ThenInclude(u => u.AppUserCompetences)
//                    .ThenInclude(cc => cc.Competence)
//                .Include(c => c.AppUser)
//                    .ThenInclude(u => u.Certificats)
//                .Include(c => c.AppUser)
//                    .ThenInclude(u => u.DiplomesCandidate)
//                .Include(c => c.Offre)
//                .FirstOrDefaultAsync(c => c.IdCandidature == id);

//            if (candidature == null)
//            {
//                return NotFound("Candidature non trouvée.");
//            }

//            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
//            var isRecruteur = User.IsInRole("Recruteur");
//            if (candidature.AppUserId != userId && !isRecruteur)
//            {
//                return Unauthorized("Vous n'êtes pas autorisé à voir cette candidature.");
//            }

//            var candidatureDto = new CandidatureDto
//            {
//                IdCandidature = candidature.IdCandidature,
//                AppUserId = candidature.AppUserId,
//                OffreId = candidature.OffreId,
//                Statut = candidature.Statut,
//                MessageMotivation = candidature.MessageMotivation,
//                DateSoumission = candidature.DateSoumission,
//                AppUser = candidature.AppUser != null ? new AppUserDto
//                {
//                    Id = candidature.AppUser.Id,
//                    FullName = candidature.AppUser.FullName,
//                    Nom = candidature.AppUser.Nom,
//                    Prenom = candidature.AppUser.Prenom,
//                    Email = candidature.AppUser.Email,
//                    Phone = candidature.AppUser.phone,
//                    NiveauEtude = candidature.AppUser.NiveauEtude,
//                    Diplome = candidature.AppUser.Diplome,
//                    Universite = candidature.AppUser.Universite,
//                    Specialite = candidature.AppUser.specialite,
//                    Cv = candidature.AppUser.cv,
//                    LinkedIn = candidature.AppUser.linkedIn,
//                    Github = candidature.AppUser.github,
//                    Portfolio = candidature.AppUser.portfolio,
//                    Statut = candidature.AppUser.Statut,
//                    LettreMotivation = candidature.AppUser.LettreMotivation,
//                    Experiences = candidature.AppUser.Experiences?.Select(e => new ExperienceDto
//                    {
//                        IdExperience = e.IdExperience,
//                        Poste = e.Poste,
//                        Description = e.Description,
//                        NomEntreprise = e.NomEntreprise,
//                        CompetenceAcquise = e.CompetenceAcquise,
//                        DateDebut = e.DateDebut,
//                        DateFin = e.DateFin
//                    }).ToList() ?? new List<ExperienceDto>(),
//                    AppUserCompetences = candidature.AppUser.AppUserCompetences?.Select(ac => new CandidateCompetenceDto
//                    {
//                        Id = ac.Id,
//                        CompetenceId = ac.CompetenceId,
//                        NiveauPossede = ac.NiveauPossede.ToString(),
//                        Competence = new CompetenceCandidateDto
//                        {
//                            Id = ac.Competence.Id,
//                            Nom = ac.Competence.Nom,
//                            Description = ac.Competence.Description,
//                            EstTechnique = ac.Competence.estTechnique,
//                            EstSoftSkill = ac.Competence.estSoftSkill
//                        }
//                    }).ToList() ?? new List<CandidateCompetenceDto>(),
//                    Certificats = candidature.AppUser.Certificats?.Select(c => new CertificatDto
//                    {
//                        IdCertificat = c.IdCertificat,
//                        Nom = c.Nom,
//                        DateObtention = c.DateObtention,
//                        Organisme = c.Organisme,
//                        Description = c.Description,
//                        UrlDocument = c.UrlDocument
//                    }).ToList() ?? new List<CertificatDto>(),
//                    DiplomesCandidate = candidature.AppUser.DiplomesCandidate?.Select(d => new DiplomeCandidateDto
//                    {
//                        IdDiplome = d.IdDiplome,
//                        AppUserId = d.AppUserId,
//                        NomDiplome = d.NomDiplome,
//                        Institution = d.Institution,
//                        DateObtention = d.DateObtention,
//                        Specialite = d.Specialite,
//                        UrlDocument = d.UrlDocument
//                    }).ToList() ?? new List<DiplomeCandidateDto>()
//                } : null,
//                Offre = candidature.Offre != null ? new OffreEmploicandidateDto
//                {
//                    IdOffreEmploi = candidature.Offre.IdOffreEmploi
//                } : null
//            };

//            return Ok(candidatureDto);
//        }

//        // GET: api/PostulerCandidate/GetPotentialCandidates/{offreId}
//        [HttpGet("GetPotentialCandidates/{offreId}")]
//        [Authorize(Roles = "Recruteur")]
//        public async Task<ActionResult<List<AppUserDto>>> GetPotentialCandidates(Guid offreId)
//        {
//            try
//            {
//                var candidatureIds = await _context.Candidatures
//                    .Where(c => c.OffreId == offreId)
//                    .Select(c => c.AppUserId)
//                    .ToListAsync();

//                var candidates = await _context.AppUser
//                    .Where(u => candidatureIds.Contains(u.Id))
//                    .Include(u => u.AppUserCompetences)
//                        .ThenInclude(ac => ac.Competence)
//                    .Include(u => u.DiplomesCandidate)
//                    .Include(u => u.Experiences)
//                    .Include(u => u.Certificats)
//                    .ToListAsync();

//                if (candidates == null || candidates.Count == 0)
//                {
//                    return NotFound("No potential candidates found for this job offer.");
//                }

//                var candidateDtos = candidates.Select(u => new AppUserDto
//                {
//                    Id = u.Id,
//                    FullName = u.FullName,
//                    Nom = u.Nom,
//                    Prenom = u.Prenom,
//                    Email = u.Email,
//                    Phone = u.phone,
//                    NiveauEtude = u.NiveauEtude,
//                    Diplome = u.Diplome,
//                    Universite = u.Universite,
//                    Specialite = u.specialite,
//                    Cv = u.cv,
//                    LinkedIn = u.linkedIn,
//                    Github = u.github,
//                    Portfolio = u.portfolio,
//                    Statut = u.Statut,
//                    LettreMotivation = u.LettreMotivation,
//                    Experiences = u.Experiences?.Select(e => new ExperienceDto
//                    {
//                        IdExperience = e.IdExperience,
//                        Poste = e.Poste,
//                        Description = e.Description,
//                        NomEntreprise = e.NomEntreprise,
//                        CompetenceAcquise = e.CompetenceAcquise,
//                        DateDebut = e.DateDebut,
//                        DateFin = e.DateFin
//                    }).ToList() ?? new List<ExperienceDto>(),
//                    AppUserCompetences = u.AppUserCompetences?.Select(ac => new CandidateCompetenceDto
//                    {
//                        Id = ac.Id,
//                        CompetenceId = ac.CompetenceId,
//                        NiveauPossede = ac.NiveauPossede.ToString(),
//                        Competence = new CompetenceCandidateDto
//                        {
//                            Id = ac.Competence.Id,
//                            Nom = ac.Competence.Nom,
//                            Description = ac.Competence.Description,
//                            EstTechnique = ac.Competence.estTechnique,
//                            EstSoftSkill = ac.Competence.estSoftSkill
//                        }
//                    }).ToList() ?? new List<CandidateCompetenceDto>(),
//                    Certificats = u.Certificats?.Select(c => new CertificatDto
//                    {
//                        IdCertificat = c.IdCertificat,
//                        Nom = c.Nom,
//                        DateObtention = c.DateObtention,
//                        Organisme = c.Organisme,
//                        Description = c.Description,
//                        UrlDocument = c.UrlDocument
//                    }).ToList() ?? new List<CertificatDto>(),
//                    DiplomesCandidate = u.DiplomesCandidate?.Select(d => new DiplomeCandidateDto
//                    {
//                        IdDiplome = d.IdDiplome,
//                        AppUserId = d.AppUserId,
//                        NomDiplome = d.NomDiplome,
//                        Institution = d.Institution,
//                        DateObtention = d.DateObtention,
//                        Specialite = d.Specialite,
//                        UrlDocument = d.UrlDocument
//                    }).ToList() ?? new List<DiplomeCandidateDto>()
//                }).ToList();

//                return Ok(candidateDtos);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"An error occurred: {ex.Message}");
//            }
//        }

//        // GET: api/PostulerCandidate/GetCandidatesForOffre/{offreId}
//        [HttpGet("GetCandidatesForOffre/{offreId}")]
//        [Authorize(Roles = "Recruteur")]
//        public async Task<ActionResult<List<CandidatureDto>>> GetCandidatesForOffre(Guid offreId)
//        {
//            try
//            {
//                var candidates = await _context.Candidatures
//                    .Where(c => c.OffreId == offreId)
//                    .Include(c => c.AppUser)
//                    .Include(c => c.Offre)
//                    .ToListAsync();

//                if (candidates == null || candidates.Count == 0)
//                {
//                    return NotFound("No candidates found for this job offer.");
//                }

//                var candidateDtos = candidates.Select(c => new CandidatureDto
//                {
//                    IdCandidature = c.IdCandidature,
//                    AppUserId = c.AppUserId,
//                    OffreId = c.OffreId,
//                    Statut = c.Statut,
//                    MessageMotivation = c.MessageMotivation,
//                    DateSoumission = c.DateSoumission,
//                    AppUser = c.AppUser != null ? new AppUserDto
//                    {
//                        Id = c.AppUser.Id,
//                        FullName = c.AppUser.FullName,
//                        Nom = c.AppUser.Nom,
//                        Prenom = c.AppUser.Prenom,
//                        Email = c.AppUser.Email,
//                        Phone = c.AppUser.phone,
//                        NiveauEtude = c.AppUser.NiveauEtude,
//                        Diplome = c.AppUser.Diplome,
//                        Universite = c.AppUser.Universite,
//                        Specialite = c.AppUser.specialite,
//                        Cv = c.AppUser.cv,
//                        LinkedIn = c.AppUser.linkedIn,
//                        Github = c.AppUser.github,
//                        Portfolio = c.AppUser.portfolio,
//                        Statut = c.AppUser.Statut,
//                        LettreMotivation = c.AppUser.LettreMotivation
//                    } : null,
//                    Offre = c.Offre != null ? new OffreEmploicandidateDto
//                    {
//                        IdOffreEmploi = c.Offre.IdOffreEmploi
//                    } : null
//                }).ToList();

//                return Ok(candidateDtos);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"An error occurred: {ex.Message}");
//            }
//        }

//        // DELETE: api/PostulerCandidate/DeleteCandidate/{candidatureId}
//        [HttpDelete("DeleteCandidate/{candidatureId}")]
//        [Authorize]
//        public async Task<IActionResult> DeleteCandidate(Guid candidatureId)
//        {
//            try
//            {
//                var candidature = await _context.Candidatures
//                    .Include(c => c.AppUser)
//                    .FirstOrDefaultAsync(c => c.IdCandidature == candidatureId);

//                if (candidature == null)
//                {
//                    return NotFound("Candidature not found.");
//                }

//                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
//                var isRecruteur = User.IsInRole("Recruteur");
//                if (candidature.AppUserId != userId && !isRecruteur)
//                {
//                    return Unauthorized("Vous n'êtes pas autorisé à supprimer cette candidature.");
//                }

//                _context.Candidatures.Remove(candidature);
//                await _context.SaveChangesAsync();

//                return NoContent();
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"An error occurred: {ex.Message}");
//            }
//        }

//        // GET: api/PostulerCandidate/GetSortedCandidates/{offreId}?sortBy=DateSoumission&ascending=true
//        [HttpGet("GetSortedCandidates/{offreId}")]
//        [Authorize(Roles = "Recruteur")]
//        public async Task<ActionResult<List<CandidatureDto>>> GetSortedCandidates(Guid offreId, [FromQuery] string sortBy = "DateSoumission", [FromQuery] bool ascending = true)
//        {
//            try
//            {
//                var query = _context.Candidatures
//                    .Where(c => c.OffreId == offreId)
//                    .Include(c => c.AppUser)
//                    .Include(c => c.Offre);

//                if (sortBy.ToLower() == "datesoumission")
//                {
//                    query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Candidature, OffreEmploi>)(ascending ? query.OrderBy(c => c.DateSoumission) : query.OrderByDescending(c => c.DateSoumission));
//                }
//                else if (sortBy.ToLower() == "statut")
//                {
//                    query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Candidature, OffreEmploi>)(ascending ? query.OrderBy(c => c.Statut) : query.OrderByDescending(c => c.Statut));
//                }

//                var sortedCandidates = await query.ToListAsync();
//                if (sortedCandidates == null || sortedCandidates.Count == 0)
//                {
//                    return NotFound("No candidates found for this job offer.");
//                }

//                var candidateDtos = sortedCandidates.Select(c => new CandidatureDto
//                {
//                    IdCandidature = c.IdCandidature,
//                    AppUserId = c.AppUserId,
//                    OffreId = c.OffreId,
//                    Statut = c.Statut,
//                    MessageMotivation = c.MessageMotivation,
//                    DateSoumission = c.DateSoumission,
//                    AppUser = c.AppUser != null ? new AppUserDto
//                    {
//                        Id = c.AppUser.Id,
//                        FullName = c.AppUser.FullName,
//                        Nom = c.AppUser.Nom,
//                        Prenom = c.AppUser.Prenom,
//                        Email = c.AppUser.Email,
//                        Phone = c.AppUser.phone,
//                        NiveauEtude = c.AppUser.NiveauEtude,
//                        Diplome = c.AppUser.Diplome,
//                        Universite = c.AppUser.Universite,
//                        Specialite = c.AppUser.specialite,
//                        Cv = c.AppUser.cv,
//                        LinkedIn = c.AppUser.linkedIn,
//                        Github = c.AppUser.github,
//                        Portfolio = c.AppUser.portfolio,
//                        Statut = c.AppUser.Statut,
//                        LettreMotivation = c.AppUser.LettreMotivation
//                    } : null,
//                    Offre = c.Offre != null ? new OffreEmploicandidateDto
//                    {
//                        IdOffreEmploi = c.Offre.IdOffreEmploi
//                    } : null
//                }).ToList();

//                return Ok(candidateDtos);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"An error occurred: {ex.Message}");
//            }
//        }

//        // PUT: api/PostulerCandidate/UpdateCandidatureStatus/{candidatureId}
//        [HttpPut("UpdateCandidatureStatus/{candidatureId}")]
//        [Authorize(Roles = "Recruteur")]
//        public async Task<IActionResult> UpdateCandidatureStatus(Guid candidatureId, [FromBody] string newStatus)
//        {
//            if (string.IsNullOrEmpty(newStatus) || newStatus.Length > 50)
//            {
//                return BadRequest("Status must be a non-empty string with a maximum length of 50 characters.");
//            }

//            try
//            {
//                var candidature = await _context.Candidatures.FindAsync(candidatureId);
//                if (candidature == null)
//                {
//                    return NotFound("Candidature not found.");
//                }

//                candidature.Statut = newStatus;
//                await _context.SaveChangesAsync();

//                return Ok("Candidate status updated successfully.");
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"An error occurred: {ex.Message}");
//            }
//        }

//        private bool CandidatureExists(Guid id)
//        {
//            return _context.Candidatures.Any(e => e.IdCandidature == id);
//        }
//    }

//    #region DTOs for Candidature

//    public class CandidatureDto
//    {
//        public Guid IdCandidature { get; set; }

//        [Required(ErrorMessage = "AppUserId is required.")]
//        public Guid AppUserId { get; set; }

//        [Required(ErrorMessage = "OffreId is required.")]
//        public Guid OffreId { get; set; }

//        [Required(ErrorMessage = "Statut is required.")]
//        [MaxLength(50)]
//        public string Statut { get; set; }

//        [MaxLength(1000)]
//        public string MessageMotivation { get; set; }

//        [Required]
//        public DateTime DateSoumission { get; set; }

//        public AppUserDto AppUser { get; set; }
//        public OffreEmploicandidateDto Offre { get; set; }
//    }

//    public class AppUserDto
//    {
//        public Guid Id { get; set; }
//        public string FullName { get; set; }
//        public string Nom { get; set; }
//        public string Prenom { get; set; }
//        [EmailAddress] public string Email { get; set; }
//        public string Phone { get; set; }
//        public string NiveauEtude { get; set; }
//        public string Diplome { get; set; }
//        public string Universite { get; set; }
//        public string Specialite { get; set; }
//        public string LettreMotivation { get; set; }
//        [MaxLength(255)] public string Cv { get; set; }
//        [MaxLength(255)] public string LinkedIn { get; set; }
//        [MaxLength(255)] public string Github { get; set; }
//        [MaxLength(255)] public string Portfolio { get; set; }
//        [MaxLength(20)] public string Statut { get; set; }

//        public List<ExperienceDto> Experiences { get; set; }
//        public List<CandidateCompetenceDto> AppUserCompetences { get; set; }
//        public List<CertificatDto> Certificats { get; set; }
//        public List<DiplomeCandidateDto> DiplomesCandidate { get; set; }
//    }

//    public class OffreEmploicandidateDto
//    {
//        public Guid IdOffreEmploi { get; set; }
//    }

//    public class ExperienceDto
//    {
//        public Guid IdExperience { get; set; }
//        [MaxLength(100)] public string Poste { get; set; }
//        [MaxLength(1000)] public string Description { get; set; }
//        [MaxLength(150)] public string NomEntreprise { get; set; }
//        [MaxLength(255)] public string CompetenceAcquise { get; set; }
//        public DateTime? DateDebut { get; set; }
//        public DateTime? DateFin { get; set; }
//    }

//    public class CertificatDto
//    {
//        public Guid IdCertificat { get; set; }
//        [MaxLength(100)] public string Nom { get; set; }
//        public DateTime DateObtention { get; set; }
//        [MaxLength(150)] public string Organisme { get; set; }
//        [MaxLength(1000)] public string Description { get; set; }
//        [MaxLength(255)] public string UrlDocument { get; set; }
//    }

//    public class CandidateCompetenceDto
//    {
//        public Guid Id { get; set; }
//        public Guid CompetenceId { get; set; }
//        public string NiveauPossede { get; set; }
//        public CompetenceCandidateDto Competence { get; set; }
//    }

//    public class CompetenceCandidateDto
//    {
//        public Guid Id { get; set; }
//        public string Nom { get; set; }
//        public string Description { get; set; }
//        public bool EstTechnique { get; set; }
//        public bool EstSoftSkill { get; set; }
//    }

//    public class DiplomeCandidateDto
//    {
//        public Guid IdDiplome { get; set; }
//        public Guid AppUserId { get; set; }
//        [MaxLength(150)] public string NomDiplome { get; set; }
//        [MaxLength(150)] public string Institution { get; set; }
//        public DateTime DateObtention { get; set; }
//        [MaxLength(255)] public string Specialite { get; set; }
//        [MaxLength(255)] public string UrlDocument { get; set; }
//    }

//    public class CandidatureInputDto
//    {
//        [Required(ErrorMessage = "AppUserId is required.")]
//        public Guid AppUserId { get; set; }

//        [Required(ErrorMessage = "OffreId is required.")]
//        public Guid OffreId { get; set; }

//        [MaxLength(1000)]
//        public string MessageMotivation { get; set; }
//    }

//    #endregion
//}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_poulina_future_jobs.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Backend_poulina_future_jobs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostulerCandidateController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PostulerCandidateController> _logger;

        public PostulerCandidateController(AppDbContext context, ILogger<PostulerCandidateController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // 1. Soumettre une candidature (public, non authentifié)
        [HttpPost("Soumettre")]
        public async Task<IActionResult> SoumettreCandidature([FromBody] CandidatureInputDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users.FindAsync(model.AppUserId);
            var offre = await _context.OffresEmploi.FindAsync(model.OffreId);
            if (user == null || offre == null)
                return NotFound("Utilisateur ou offre non trouvé.");

            // Vérifier si déja candidat
            if (await _context.Candidatures.AnyAsync(c => c.AppUserId == model.AppUserId && c.OffreId == model.OffreId))
                return BadRequest("Vous avez déjà postulé pour cette offre.");

            var candidature = new Candidature
            {
                IdCandidature = Guid.NewGuid(),
                AppUserId = model.AppUserId,
                OffreId = model.OffreId,
                Statut = "En cours",
                MessageMotivation = model.MessageMotivation,
                DateSoumission = DateTime.UtcNow
            };

            _context.Candidatures.Add(candidature);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
                    return BadRequest("Vous avez déjà postulé pour cette offre.");
                throw;
            }

            return Ok(new { Message = "Candidature soumise avec succès", Id = candidature.IdCandidature });
        }

        // 2. Postuler (authentifié, poste pour soi)
        [HttpPost]
        [Authorize(Roles = "Candidate")]
       
        public async Task<ActionResult<CandidatureDto>> PostCandidature([FromBody] CandidatureInputDto input)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for candidature submission: {@ModelState}", ModelState);
                return BadRequest(ModelState);
            }

            // Log all claims for debugging
            foreach (var claim in User.Claims)
            {
                _logger.LogInformation("Claim Type: {Type}, Value: {Value}", claim.Type, claim.Value);
            }

            // Try different claim names to extract userId
            var userIdClaim = User.FindFirstValue("userId")
                            ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                            ?? User.FindFirstValue("sub");

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Invalid or missing user ID in token.");
                return Unauthorized("Invalid or missing user ID in token.");
            }

            _logger.LogInformation("Extracted userId from token: {UserId}", userId);

            if (input.AppUserId != userId)
            {
                _logger.LogWarning("Unauthorized access: User ID mismatch. Expected {UserId}, got {InputUserId}", userId, input.AppUserId);
                return Unauthorized("Vous ne pouvez postuler que pour vous-même.");
            }

            var offre = await _context.OffresEmploi.FindAsync(input.OffreId);
            if (offre == null || offre.Statut != StatutOffre.Ouvert)
            {
                _logger.LogWarning("Invalid offer: Offer ID {OffreId} not found or closed.", input.OffreId);
                return BadRequest("L'offre n'existe pas ou est clôturée.");
            }

            var user = await _context.Users.FindAsync(input.AppUserId);
            if (user == null)
            {
                _logger.LogWarning("User not found: User ID {UserId}", input.AppUserId);
                return BadRequest("L'utilisateur n'existe pas.");
            }

            var existingCandidature = await _context.Candidatures
                .AnyAsync(c => c.AppUserId == input.AppUserId && c.OffreId == input.OffreId);
            if (existingCandidature)
            {
                _logger.LogWarning("Duplicate candidature: User {UserId} already applied for Offer {OffreId}", input.AppUserId, input.OffreId);
                return BadRequest("Vous avez déjà postulé pour cette offre.");
            }

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
            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Candidature submitted successfully: {IdCandidature}", candidature.IdCandidature);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlException && (sqlException.Number == 2601 || sqlException.Number == 2627))
                {
                    _logger.LogWarning("Duplicate candidature detected: {UserId} for {OffreId}", input.AppUserId, input.OffreId);
                    return BadRequest("Vous avez déjà postulé pour cette offre.");
                }
                _logger.LogError(ex, "Database error while saving candidature.");
                return StatusCode(500, "Une erreur s'est produite lors de l'enregistrement de la candidature.");
            }

            var savedCandidature = await _context.Candidatures
                .Include(c => c.AppUser)
                    .ThenInclude(u => u.Experiences)
                .Include(c => c.AppUser)
                    .ThenInclude(u => u.AppUserCompetences)
                    .ThenInclude(cc => cc.Competence)
                .Include(c => c.AppUser)
                    .ThenInclude(u => u.Certificats)
                .Include(c => c.AppUser)
                    .ThenInclude(u => u.DiplomesCandidate)
                .Include(c => c.Offre)
                .FirstOrDefaultAsync(c => c.IdCandidature == candidature.IdCandidature);

            if (savedCandidature == null)
            {
                _logger.LogError("Failed to retrieve saved candidature: {IdCandidature}", candidature.IdCandidature);
                return StatusCode(500, "Impossible de récupérer la candidature enregistrée.");
            }

            var resultDto = MapToCandidatureDto(savedCandidature);
            return CreatedAtAction(nameof(GetCandidature), new { id = resultDto.IdCandidature }, resultDto);
        }


        // 3. Modifier une candidature
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutCandidature(Guid id, [FromBody] CandidatureDto dto)
        {
            if (id != dto.IdCandidature)
                return BadRequest("ID mismatch");

            var candidature = await _context.Candidatures
                .Include(c => c.AppUser)
                .Include(c => c.Offre)
                .FirstOrDefaultAsync(c => c.IdCandidature == id);

            if (candidature == null)
                return NotFound("Candidature non trouvée");

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var isRecruteur = User.IsInRole("Recruteur");
            if (candidature.AppUserId != userId && !isRecruteur)
                return Unauthorized("Non autorisé");

            // Mise à jour
            candidature.MessageMotivation = dto.MessageMotivation;
            if (isRecruteur && !string.IsNullOrEmpty(dto.Statut) && dto.Statut.Length <= 50)
            {
                candidature.Statut = dto.Statut;
            }

            _context.Candidatures.Update(candidature);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 4. Récupérer une candidature
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<CandidatureDto>> GetCandidature(Guid id)
        {
            var c = await _context.Candidatures
                .Include(c => c.AppUser)
                    .ThenInclude(u => u.Experiences)
                .Include(c => c.AppUser)
                    .ThenInclude(u => u.AppUserCompetences).ThenInclude(c => c.Competence)
                .Include(c => c.AppUser)
                    .ThenInclude(u => u.Certificats)
                .Include(c => c.AppUser)
                    .ThenInclude(u => u.DiplomesCandidate)
                .Include(c => c.Offre)
                .FirstOrDefaultAsync(c => c.IdCandidature == id);

            if (c == null)
                return NotFound("Candidature non trouvée");

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var isRecruteur = User.IsInRole("Recruteur");
            if (c.AppUserId != userId && !isRecruteur)
                return Unauthorized("Non autorisé");

            return MapToCandidatureDto(c);
        }

        // 5. Supprimer une candidature
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(Guid id)
        {
            var c = await _context.Candidatures
                .Include(c => c.AppUser)
                .FirstOrDefaultAsync(c => c.IdCandidature == id);
            if (c == null)
                return NotFound();

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var isRecruteur = User.IsInRole("Recruteur");
            if (c.AppUserId != userId && !isRecruteur)
                return Unauthorized();

            _context.Candidatures.Remove(c);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // 6. Liste des candidats potentiels pour une offre
        [HttpGet("GetPotentialCandidates/{offreId}")]
        [Authorize(Roles = "Recruteur")]
        public async Task<ActionResult<List<AppUserDto>>> GetPotentialCandidates(Guid offreId)
        {
            try
            {
                var candidatureIds = await _context.Candidatures
                    .Where(c => c.OffreId == offreId)
                    .Select(c => c.AppUserId)
                    .ToListAsync();

                var candidates = await _context.AppUser
                    .Where(u => candidatureIds.Contains(u.Id))
                    .Include(u => u.Experiences)
                    .Include(u => u.AppUserCompetences)
                        .ThenInclude(ac => ac.Competence)
                    .Include(u => u.Certificats)
                    .Include(u => u.DiplomesCandidate)
                    .ToListAsync();

                if (candidates == null || candidates.Count == 0)
                {
                    _logger.LogInformation("No potential candidates found for offer: {OffreId}", offreId);
                    return NotFound("No potential candidates found for this job offer.");
                }

                var candidateDtos = candidates.Select(u => MapToAppUserDto(u)).ToList();
                return Ok(candidateDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving potential candidates for offer: {OffreId}", offreId);
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }


        // 7. Liste des candidats pour une offre
        [HttpGet("GetCandidatesForOffre/{offreId}")]
        [Authorize(Roles = "Recruteur")]
        public async Task<ActionResult<List<CandidatureDto>>> GetCandidatesForOffre(Guid offreId)
        {
            var candidatures = await _context.Candidatures
                .Where(c => c.OffreId == offreId)
                .Include(c => c.AppUser)
                .Include(c => c.Offre)
                .ToListAsync();

            var dtos = candidatures.Select(c => MapToCandidatureDto(c)).ToList();
            return Ok(dtos);
        }

        // 8. Mise à jour du statut d'une candidature
        [HttpPut("UpdateCandidatureStatus/{id}")]
        [Authorize(Roles = "Recruteur")]
        public async Task<IActionResult> UpdateCandidatureStatus(Guid id, [FromBody] string newStatus)
        {
            if (string.IsNullOrEmpty(newStatus) || newStatus.Length > 50)
                return BadRequest("Statut invalide");

            var c = await _context.Candidatures.FindAsync(id);
            if (c == null)
                return NotFound();

            c.Statut = newStatus;
            await _context.SaveChangesAsync();
            return Ok("Statut mis à jour");
        }

        // ### Helper methods for mapping
        private static CandidatureDto MapToCandidatureDto(Candidature c)
        {
            return new CandidatureDto
            {
                IdCandidature = c.IdCandidature,
                AppUserId = c.AppUserId,
                OffreId = c.OffreId,
                Statut = c.Statut,
                MessageMotivation = c.MessageMotivation,
                DateSoumission = c.DateSoumission,
                AppUser = c.AppUser != null ? MapToAppUserDto(c.AppUser) : null,
                Offre = c.Offre != null ? new OffreEmploicandidateDto { IdOffreEmploi = c.Offre.IdOffreEmploi } : null
            };
        }

        private static AppUserDto MapToAppUserDto(AppUser u)
        {
            return new AppUserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Nom = u.Nom,
                Prenom = u.Prenom,
                Email = u.Email,
                Phone = u.phone,
                NiveauEtude = u.NiveauEtude,
                Diplome = u.Diplome,
                Universite = u.Universite,
                Specialite = u.specialite,
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
                    CompetenceAcquise = e.CompetenceAcquise,
                    DateDebut = e.DateDebut,
                    DateFin = e.DateFin
                }).ToList() ?? new List<ExperienceDto>(),
                AppUserCompetences = u.AppUserCompetences?.Select(ac => new CandidateCompetenceDto
                {
                    Id = ac.Id,
                    CompetenceId = ac.CompetenceId,
                    NiveauPossede = ac.NiveauPossede.ToString(),
                    Competence = new CompetenceCandidateDto
                    {
                        Id = ac.Competence.Id,
                        Nom = ac.Competence.Nom,
                        Description = ac.Competence.Description,
                        EstTechnique = ac.Competence.estTechnique,
                        EstSoftSkill = ac.Competence.estSoftSkill
                    }
                }).ToList() ?? new List<CandidateCompetenceDto>(),
                Certificats = u.Certificats?.Select(c => new CertificatDto
                {
                    IdCertificat = c.IdCertificat,
                    Nom = c.Nom,
                    DateObtention = c.DateObtention,
                    Organisme = c.Organisme,
                    Description = c.Description,
                    UrlDocument = c.UrlDocument
                }).ToList() ?? new List<CertificatDto>(),
                DiplomesCandidate = u.DiplomesCandidate?.Select(d => new DiplomeCandidateDto
                {
                    IdDiplome = d.IdDiplome,
                    AppUserId = d.AppUserId,
                    NomDiplome = d.NomDiplome,
                    Institution = d.Institution,
                    DateObtention = d.DateObtention,
                    Specialite = d.Specialite,
                    UrlDocument = d.UrlDocument
                }).ToList() ?? new List<DiplomeCandidateDto>()
            };
        }
    }

    #region DTOs
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

    public class OffreEmploicandidateDto
    {
        public Guid IdOffreEmploi { get; set; }
    }

    public class ExperienceDto
    {
        public Guid IdExperience { get; set; }
        public string Poste { get; set; }
        public string Description { get; set; }
        public string NomEntreprise { get; set; }
        public string CompetenceAcquise { get; set; }
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
    }

    public class CertificatDto
    {
        public Guid IdCertificat { get; set; }
        public string Nom { get; set; }
        public DateTime DateObtention { get; set; }
        public string Organisme { get; set; }
        public string Description { get; set; }
        public string UrlDocument { get; set; }
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
        public string Description { get; set; }
        public bool EstTechnique { get; set; }
        public bool EstSoftSkill { get; set; }
    }

    public class DiplomeCandidateDto
    {
        public Guid IdDiplome { get; set; }
        public Guid AppUserId { get; set; }
        public string NomDiplome { get; set; }
        public string Institution { get; set; }
        public DateTime DateObtention { get; set; }
        public string Specialite { get; set; }
        public string UrlDocument { get; set; }
    }

    public class CandidatureInputDto
    {
        public Guid AppUserId { get; set; }
        public Guid OffreId { get; set; }
        public string MessageMotivation { get; set; }
    }
    #endregion
}