//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Backend_poulina_future_jobs.Models;
//using Backend_poulina_future_jobs.DTOs;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using System.ComponentModel.DataAnnotations;
//using System.Security.Claims;

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
//            _context = context;
//            _userManager = userManager;
//        } 


//        // Action pour soumettre une candidature
//        [HttpPost]
//        [Route("Soumettre")]
//        public async Task<IActionResult> SoumettreCandidature([FromBody] CandidatureViewModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            // Vérifiez si l'utilisateur et l'offre existent
//            var user = await _context.Users.FindAsync(model.AppUserId);
//            var offre = await _context.OffresEmploi.FindAsync(model.OffreId);
//            if (user == null || offre == null)
//            {
//                return NotFound("Utilisateur ou offre non trouvés.");
//            }

//            // Créez une nouvelle candidature
//            var candidature = new Models.Candidatures
//            {
//                IdCandidature = Guid.NewGuid(),
//                AppUserId = model.AppUserId,
//                OffreId = model.OffreId,
//                Statut = "En cours",
//                MessageMotivation = model.MessageMotivation,
//                DateSoumission = DateTime.UtcNow
//            };

//            _context.Candidatures.Add(candidature);
//            await _context.SaveChangesAsync();

//            return Ok(new { Message = "Candidature soumise avec succès.", CandidatureId = candidature.IdCandidature });
//        }




//        // POST: api/Candidatures (Apply for a job)
//        [HttpPost]
//        [Authorize(Roles = "Candidat")] // Only candidates can apply
//        public async Task<ActionResult<CandidatureDto>> PostCandidature([FromBody] CandidatureDto candidatureDto)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            // Get the authenticated user's ID
//            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
//            if (candidatureDto.AppUserId != userId)
//            {
//                return Unauthorized("Vous ne pouvez postuler que pour vous-même.");
//            }

//            // Verify Offer exists and is open
//            var offre = await _context.OffresEmploi.FindAsync(candidatureDto.OffreId);
//            if (offre == null || offre.Statut != StatutOffre.Ouvert)
//            {
//                return BadRequest("L'offre n'existe pas ou est clôturée.");
//            }

//            // Verify User exists
//            var user = await _context.Users.FindAsync(candidatureDto.AppUserId);
//            if (user == null)
//            {
//                return BadRequest("L'utilisateur n'existe pas.");
//            }

//            // Check if user already applied
//            var existingCandidature = await _context.Candidatures
//                .AnyAsync(c => c.AppUserId == candidatureDto.AppUserId && c.OffreId == candidatureDto.OffreId);
//            if (existingCandidature)
//            {
//                return BadRequest("Vous avez déjà postulé pour cette offre.");
//            }

//            // Map DTO to Entity
//            var candidature = _mapper.Map<Models.Candidatures>(candidatureDto);
//            candidature.IdCandidature = Guid.NewGuid();
//            candidature.DateSoumission = DateTime.UtcNow;
//            candidature.Statut = "Soumise"; // Initial status

//            _context.Candidatures.Add(candidature);
//            await _context.SaveChangesAsync();

//            // Load related data and map back to DTO
//            var savedCandidature = await _context.Candidatures
//                .Include(c => c.AppUser)
//                    .ThenInclude(u => u.Experiences)
//                    .ThenInclude(e => e.Certificats)
//                .Include(c => c.AppUser)
//                    .ThenInclude(u => u.AppUserCompetences)
//                    .ThenInclude(cc => cc.Competence)
//                .Include(c => c.Offre)
//                .FirstOrDefaultAsync(c => c.IdCandidature == candidature.IdCandidature);

//            var resultDto = _mapper.Map<CandidatureDto>(savedCandidature);
//            return CreatedAtAction(nameof(GetCandidature), new { id = resultDto.IdCandidature }, resultDto);
//        }

//        // PUT: api/Candidatures/{id} (Modify an existing candidature)
//        [HttpPut("{id}")]
//        [Authorize] // Candidates can modify their own, recruiters can modify any
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

//            // Authorization check
//            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
//            var isRecruteur = User.IsInRole("Recruteur");
//            if (existingCandidature.AppUserId != userId && !isRecruteur)
//            {
//                return Unauthorized("Vous n'êtes pas autorisé à modifier cette candidature.");
//            }

//            // Update allowed fields
//            existingCandidature.MessageMotivation = candidatureDto.MessageMotivation;
//            if (isRecruteur)
//            {
//                // Only recruiters can change status
//                existingCandidature.Statut = candidatureDto.Statut;
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
//                throw;
//            }

//            return NoContent();
//        }

//        // GET: api/Candidatures/{id} (Helper method for retrieving a candidature)
//        [HttpGet("{id}")]
//        [Authorize]
//        public async Task<ActionResult<CandidatureDto>> GetCandidature(Guid id)
//        {
//            var candidature = await _context.Candidatures
//                .Include(c => c.AppUser)
//                    .ThenInclude(u => u.Experiences)
//                    .ThenInclude(e => e.Certificats)
//                .Include(c => c.AppUser)
//                    .ThenInclude(u => u.AppUserCompetences)
//                    .ThenInclude(cc => cc.Competence)
//                .Include(c => c.Offre)
//                .FirstOrDefaultAsync(c => c.IdCandidature == id);

//            if (candidature == null)
//            {
//                return NotFound("Candidature non trouvée.");
//            }

//            // Authorization check
//            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
//            var isRecruteur = User.IsInRole("Recruteur");
//            if (candidature.AppUserId != userId && !isRecruteur)
//            {
//                return Unauthorized("Vous n'êtes pas autorisé à voir cette candidature.");
//            }

//            var candidatureDto = _mapper.Map<CandidatureDto>(candidature);
//            return candidatureDto;
//        }

//        private bool CandidatureExists(Guid id)
//        {
//            return _context.Candidatures.Any(e => e.IdCandidature == id);
//        }
//    }

//    // ViewModel pour simplifier l'entrée des données de candidature
//    public class CandidatureViewModel
//    {
//        [Required]
//        public Guid AppUserId { get; set; }

//        [Required]
//        public Guid OffreId { get; set; }

//        [MaxLength(1000)]
//        public string MessageMotivation { get; set; }
//    }


//    public class CandidatureDto
//    {
//        public Guid IdCandidature { get; set; }

//        [Required]
//        public Guid AppUserId { get; set; }

//        [Required]
//        public Guid OffreId { get; set; }

//        [Required]
//        [MaxLength(50)]
//        public string Statut { get; set; }

//        [MaxLength(1000)]
//        public string MessageMotivation { get; set; }

//        [Required]
//        public DateTime DateSoumission { get; set; }

//        public AppUserDto AppUser { get; set; }
//        public OffreEmploiDto Offre { get; set; }
//    }

//    public class AppUserDto
//    {
//        public Guid Id { get; set; }
//        public string FullName { get; set; }
//        public string Nom { get; set; }
//        public string Prenom { get; set; }
//        public string Email { get; set; }
//        public string Phone { get; set; }
//        public string NiveauEtude { get; set; }
//        public string Diplome { get; set; }
//        public string Universite { get; set; }
//        public string Specialite { get; set; }
//        public string Cv { get; set; }
//        public string LinkedIn { get; set; }
//        public string Github { get; set; }
//        public string Portfolio { get; set; }
//        public string Statut { get; set; }
//        public List<ExperienceDto> Experiences { get; set; }
//        public List<CandidateCompetenceDto> AppUserCompetences { get; set; }
//    }

//    public class OffreEmploiDto
//    {
//        public Guid IdOffreEmploi { get; set; }
//        public string Specialite { get; set; }
//        public string Titre { get; set; }
//        public string Description { get; set; }
//        public DateTime DatePublication { get; set; }
//        public DateTime? DateExpiration { get; set; }
//        public decimal SalaireMin { get; set; }
//        public decimal SalaireMax { get; set; }
//        public string NiveauExperienceRequis { get; set; }
//        public string DiplomeRequis { get; set; }
//        public TypeContratEnum TypeContrat { get; set; }
//        public StatutOffre Statut { get; set; }
//        public ModeTravail ModeTravail { get; set; }
//        public int NombrePostes { get; set; }
//        public string Avantages { get; set; }
//    }

//    public class ExperienceDto
//    {
//        public Guid IdExperience { get; set; }
//        public string Poste { get; set; }
//        public string Description { get; set; }
//        public string NomEntreprise { get; set; }
//        public string CompetenceAcquise { get; set; }
//        public DateTime? DateDebut { get; set; }
//        public DateTime? DateFin { get; set; }
//        public List<CertificatDto> Certificats { get; set; }
//    }

//    public class CertificatDto
//    {
//        public Guid IdCertificat { get; set; }
//        public string Nom { get; set; }
//        public DateTime DateObtention { get; set; }
//        public string Organisme { get; set; }
//        public string Description { get; set; }
//        public string UrlDocument { get; set; }
//    }

//    public class CandidateCompetenceDto
//    {
//        public Guid Id { get; set; }
//        public Guid CompetenceId { get; set; }
//        public string NiveauPossede { get; set; }
//        public CompetenceDto Competence { get; set; }
//    }

//    public class CompetenceDto
//    {
//        public Guid Id { get; set; }
//        public string Nom { get; set; }
//        public string Description { get; set; }
//        public bool EstTechnique { get; set; }
//        public bool EstSoftSkill { get; set; }
//    }
//}
    


