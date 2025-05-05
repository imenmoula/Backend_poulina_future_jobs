using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_poulina_future_jobs.Models;
using Backend_poulina_future_jobs.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Backend_poulina_future_jobs.Dtos
{
    public class OffreEmploiDto
    {
        public Guid IdOffreEmploi { get; set; }
        [Required(ErrorMessage = "La spécialité est obligatoire.")]
        public string Specialite { get; set; } = string.Empty;
        public DateTime DatePublication { get; set; }
        [Required(ErrorMessage = "La date d'expiration est obligatoire.")]
        public DateTime? DateExpiration { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Le salaire minimum doit être positif.")]
        public decimal SalaireMin { get; set; }
        [Range(0, double.MaxValue, ErrorMessage = "Le salaire maximum doit être positif.")]
        public decimal SalaireMax { get; set; }
        [Required(ErrorMessage = "Le niveau d'expérience requis est obligatoire.")]
        public string NiveauExperienceRequis { get; set; } = string.Empty;
        [Required(ErrorMessage = "Le type de contrat est obligatoire.")]
        public TypeContratEnum TypeContrat { get; set; }
        [Required(ErrorMessage = "Le statut est obligatoire.")]
        public StatutOffre Statut { get; set; }
        public ModeTravail ModeTravail { get; set; }
        [Required(ErrorMessage = "L'ID de la filiale est obligatoire.")]
        public Guid IdFiliale { get; set; }
        [Required(ErrorMessage = "L'ID du département est obligatoire.")]
        public Guid IdDepartement { get; set; }
        public bool EstActif { get; set; }
        public string Avantages { get; set; } = string.Empty;
        [Required(ErrorMessage = "L'ID du recruteur est obligatoire.")]
        public Guid IdRecruteur { get; set; }
        public List<PosteDto> Postes { get; set; } = new List<PosteDto>();
        public List<OffreMissionDto> OffreMissions { get; set; } = new List<OffreMissionDto>();
        public List<OffreLangueDto> OffreLangues { get; set; } = new List<OffreLangueDto>();
        public List<OffreCompetenceDto> OffreCompetences { get; set; } = new List<OffreCompetenceDto>();
        public List<Guid> DiplomeIds { get; set; } = new List<Guid>();
    }

    public class PosteDto
    {
        [Required(ErrorMessage = "Le titre du poste est obligatoire.")]
        public string TitrePoste { get; set; } = string.Empty;
        [Required(ErrorMessage = "La description du poste est obligatoire.")]
        public string Description { get; set; } = string.Empty;
        [Range(1, int.MaxValue, ErrorMessage = "Le nombre de postes doit être au moins 1.")]
        public int NombrePostes { get; set; }
        public string? ExperienceSouhaitee { get; set; } // Nouveau champ
        [Required(ErrorMessage = "Le niveau hiérarchique est obligatoire.")]
        public string NiveauHierarchique { get; set; } = string.Empty; // Nouveau champ
    }

    public class OffreMissionDto
    {
        [Required(ErrorMessage = "La description de la mission est obligatoire.")]
        public string DescriptionMission { get; set; } = string.Empty;
        public int Priorite { get; set; }
    }

    public class OffreLangueDto
    {
        [Required(ErrorMessage = "La langue est obligatoire.")]
        public Langue Langue { get; set; }
        public string NiveauRequis { get; set; } = string.Empty;
    }

    public class OffreCompetenceDto
    {
        public Guid IdOffreEmploi { get; set; }
        [Required(ErrorMessage = "L'ID de la compétence est obligatoire.")]
        public Guid IdCompetence { get; set; }
        [Required(ErrorMessage = "Le niveau requis est obligatoire.")]
        public NiveauRequisType NiveauRequis { get; set; }
        public CompetenceDto? Competence { get; set; }
    }

    public class CompetenceDto
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Le nom de la compétence est obligatoire.")]
        public string Nom { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DateModification { get; set; }
        public bool EstTechnique { get; set; }
        public bool EstSoftSkill { get; set; }
    }

    public class CreateOffreEmploiRequest
    {
        [Required(ErrorMessage = "Les données de l'offre sont obligatoires.")]
        public OffreEmploiDto Dto { get; set; }
    }
}

namespace Backend_poulina_future_jobs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OffreEmploisController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OffreEmploisController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/OffreEmplois
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetAllOffreEmplois()
        {
            var offresEmploi = await _context.OffresEmploi
                .Include(o => o.Postes)
                .Include(o => o.OffreMissions)
                .Include(o => o.OffreLangues)
                .Include(o => o.OffreCompetences).ThenInclude(oc => oc.Competence)
                .Include(o => o.DiplomesRequis)
                .Include(o => o.Filiale)
                .Include(o => o.Departement)
                .ToListAsync();

            if (!offresEmploi.Any())
            {
                return NotFound(new { success = false, message = "Aucune offre d'emploi trouvée." });
            }

            var offresEmploiDto = offresEmploi.Select(offre => new OffreEmploiDto
            {
                IdOffreEmploi = offre.IdOffreEmploi,
                Specialite = offre.Specialite,
                DatePublication = offre.DatePublication,
                DateExpiration = offre.DateExpiration,
                SalaireMin = offre.SalaireMin,
                SalaireMax = offre.SalaireMax,
                NiveauExperienceRequis = offre.NiveauExperienceRequis,
                TypeContrat = offre.TypeContrat,
                Statut = offre.Statut,
                ModeTravail = offre.ModeTravail,
                EstActif = offre.estActif,
                Avantages = offre.Avantages,
                IdRecruteur = offre.IdRecruteur,
                IdFiliale = offre.IdFiliale,
                IdDepartement = offre.IdDepartement,
                Postes = offre.Postes.Select(p => new PosteDto
                {
                    TitrePoste = p.TitrePoste,
                    Description = p.Description,
                    NombrePostes = p.NombrePostes,
                    ExperienceSouhaitee = p.ExperienceSouhaitee,
                    NiveauHierarchique = p.NiveauHierarchique
                }).ToList(),
                OffreMissions = offre.OffreMissions.Select(m => new OffreMissionDto
                {
                    DescriptionMission = m.DescriptionMission,
                    Priorite = m.Priorite
                }).ToList(),
                OffreLangues = offre.OffreLangues.Select(l => new OffreLangueDto
                {
                    Langue = l.Langue,
                    NiveauRequis = l.NiveauRequis
                }).ToList(),
                OffreCompetences = offre.OffreCompetences.Select(oc => new OffreCompetenceDto
                {
                    IdOffreEmploi = oc.IdOffreEmploi,
                    IdCompetence = oc.IdCompetence,
                    NiveauRequis = oc.NiveauRequis,
                    Competence = new CompetenceDto
                    {
                        Id = oc.Competence.Id,
                        Nom = oc.Competence.Nom,
                        Description = oc.Competence.Description,
                        DateModification = oc.Competence.DateModification,
                        EstTechnique = oc.Competence.estTechnique,
                        EstSoftSkill = oc.Competence.estSoftSkill
                    }
                }).ToList(),
                DiplomeIds = offre.DiplomesRequis.Select(d => d.IdDiplome).ToList()
            }).ToList();

            return Ok(new { success = true, message = "Liste des offres d'emploi récupérée avec succès.", offresEmploi = offresEmploiDto });
        }

        // GET: api/OffreEmplois/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetOffreEmploi(Guid id)
        {
            var offreEmploi = await _context.OffresEmploi
                .Include(o => o.Postes)
                .Include(o => o.OffreMissions)
                .Include(o => o.OffreLangues)
                .Include(o => o.OffreCompetences).ThenInclude(oc => oc.Competence)
                .Include(o => o.DiplomesRequis)
                .Include(o => o.Filiale)
                .Include(o => o.Departement)
                .FirstOrDefaultAsync(o => o.IdOffreEmploi == id);

            if (offreEmploi == null)
            {
                return NotFound(new { success = false, message = "L'offre d'emploi demandée n'existe pas." });
            }

            var offreEmploiDto = new OffreEmploiDto
            {
                IdOffreEmploi = offreEmploi.IdOffreEmploi,
                Specialite = offreEmploi.Specialite,
                DatePublication = offreEmploi.DatePublication,
                DateExpiration = offreEmploi.DateExpiration,
                SalaireMin = offreEmploi.SalaireMin,
                SalaireMax = offreEmploi.SalaireMax,
                NiveauExperienceRequis = offreEmploi.NiveauExperienceRequis,
                TypeContrat = offreEmploi.TypeContrat,
                Statut = offreEmploi.Statut,
                ModeTravail = offreEmploi.ModeTravail,
                EstActif = offreEmploi.estActif,
                Avantages = offreEmploi.Avantages,
                IdRecruteur = offreEmploi.IdRecruteur,
                IdFiliale = offreEmploi.IdFiliale,
                IdDepartement = offreEmploi.IdDepartement,
                Postes = offreEmploi.Postes.Select(p => new PosteDto
                {
                    TitrePoste = p.TitrePoste,
                    Description = p.Description,
                    NombrePostes = p.NombrePostes,
                    ExperienceSouhaitee = p.ExperienceSouhaitee,
                    NiveauHierarchique = p.NiveauHierarchique
                }).ToList(),
                OffreMissions = offreEmploi.OffreMissions.Select(m => new OffreMissionDto
                {
                    DescriptionMission = m.DescriptionMission,
                    Priorite = m.Priorite
                }).ToList(),
                OffreLangues = offreEmploi.OffreLangues.Select(l => new OffreLangueDto
                {
                    Langue = l.Langue,
                    NiveauRequis = l.NiveauRequis
                }).ToList(),
                OffreCompetences = offreEmploi.OffreCompetences.Select(oc => new OffreCompetenceDto
                {
                    IdOffreEmploi = oc.IdOffreEmploi,
                    IdCompetence = oc.IdCompetence,
                    NiveauRequis = oc.NiveauRequis,
                    Competence = new CompetenceDto
                    {
                        Id = oc.Competence.Id,
                        Nom = oc.Competence.Nom,
                        Description = oc.Competence.Description,
                        DateModification = oc.Competence.DateModification,
                        EstTechnique = oc.Competence.estTechnique,
                        EstSoftSkill = oc.Competence.estSoftSkill
                    }
                }).ToList(),
                DiplomeIds = offreEmploi.DiplomesRequis.Select(d => d.IdDiplome).ToList()
            };

            return Ok(new { success = true, message = "Offre d'emploi récupérée avec succès.", offreEmploi = offreEmploiDto });
        }

        // POST: api/OffreEmplois
        [HttpPost]
        [Authorize(Roles = "Recruteur")]
        public async Task<ActionResult<object>> CreateOffreEmploi([FromBody] CreateOffreEmploiRequest request)
        {
            if (request?.Dto == null)
            {
                return BadRequest(new { success = false, message = "Données invalides." });
            }

            var dto = request.Dto;

            // Vérification que la filiale existe
            var filialeExists = await _context.Filiales.AnyAsync(f => f.IdFiliale == dto.IdFiliale);
            if (!filialeExists)
            {
                return BadRequest(new { success = false, message = "La filiale spécifiée n'existe pas." });
            }

            // Vérification que le département existe et appartient à la filiale
            var departementExists = await _context.Departements
                .AnyAsync(d => d.IdDepartement == dto.IdDepartement && d.IdFiliale == dto.IdFiliale);
            if (!departementExists)
            {
                return BadRequest(new { success = false, message = "Le département spécifié n'existe pas ou n'appartient pas à la filiale." });
            }

            // Vérification du recruteur
            var userExists = await _context.Users.AnyAsync(u => u.Id == dto.IdRecruteur);
            if (!userExists)
            {
                return BadRequest(new { success = false, message = "Le recruteur spécifié n'existe pas." });
            }

            var hasRecruteurRole = await _context.UserRoles
                .Join(_context.Roles,
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => new { ur.UserId, r.Name })
                .AnyAsync(ur => ur.UserId == dto.IdRecruteur && ur.Name.ToLower() == "recruteur");
            if (!hasRecruteurRole)
            {
                return BadRequest(new { success = false, message = "L'utilisateur spécifié n'a pas le rôle Recruteur." });
            }

            // Validation des compétences et diplômes
            foreach (var competence in dto.OffreCompetences)
            {
                if (!await _context.Competences.AnyAsync(c => c.Id == competence.IdCompetence))
                {
                    return BadRequest(new { success = false, message = $"La compétence avec ID {competence.IdCompetence} n'existe pas." });
                }
            }
            foreach (var diplomeId in dto.DiplomeIds)
            {
                if (!await _context.Diplomes.AnyAsync(d => d.IdDiplome == diplomeId))
                {
                    return BadRequest(new { success = false, message = $"Le diplôme avec ID {diplomeId} n'existe pas." });
                }
            }

            // Validation des règles métier
            if (dto.SalaireMin > dto.SalaireMax)
            {
                return BadRequest(new { success = false, message = "Le salaire minimum doit être inférieur au salaire maximum." });
            }
            if (dto.DateExpiration <= DateTime.UtcNow)
            {
                return BadRequest(new { success = false, message = "La date d'expiration doit être postérieure à la date actuelle." });
            }
            if (!Enum.IsDefined(typeof(TypeContratEnum), dto.TypeContrat))
            {
                return BadRequest(new { success = false, message = "Type de contrat invalide." });
            }
            if (!Enum.IsDefined(typeof(StatutOffre), dto.Statut))
            {
                return BadRequest(new { success = false, message = "Statut invalide." });
            }
            if (!Enum.IsDefined(typeof(ModeTravail), dto.ModeTravail))
            {
                return BadRequest(new { success = false, message = "Mode de travail invalide." });
            }
            foreach (var competence in dto.OffreCompetences)
            {
                if (!Enum.IsDefined(typeof(NiveauRequisType), competence.NiveauRequis))
                {
                    return BadRequest(new { success = false, message = $"Niveau requis '{competence.NiveauRequis}' invalide." });
                }
            }
            foreach (var langue in dto.OffreLangues)
            {
                if (!Enum.IsDefined(typeof(Langue), langue.Langue))
                {
                    return BadRequest(new { success = false, message = $"Langue '{langue.Langue}' invalide." });
                }
            }

            var newId = Guid.NewGuid();
            var offreEmploi = new OffreEmploi
            {
                IdOffreEmploi = newId,
                Specialite = dto.Specialite,
                DatePublication = DateTime.UtcNow,
                DateExpiration = dto.DateExpiration,
                SalaireMin = dto.SalaireMin,
                SalaireMax = dto.SalaireMax,
                NiveauExperienceRequis = dto.NiveauExperienceRequis,
                TypeContrat = dto.TypeContrat,
                Statut = dto.Statut,
                ModeTravail = dto.ModeTravail,
                estActif = dto.EstActif,
                Avantages = dto.Avantages,
                IdRecruteur = dto.IdRecruteur,
                IdFiliale = dto.IdFiliale,
                IdDepartement = dto.IdDepartement,
                Postes = dto.Postes.Select(p => new Poste
                {
                    IdOffreEmploi = newId,
                    TitrePoste = p.TitrePoste,
                    Description = p.Description,
                    NombrePostes = p.NombrePostes,
                    ExperienceSouhaitee = p.ExperienceSouhaitee,
                    NiveauHierarchique = p.NiveauHierarchique
                }).ToList(),
                OffreMissions = dto.OffreMissions.Select(m => new OffreMission
                {
                    IdOffreEmploi = newId,
                    DescriptionMission = m.DescriptionMission,
                    Priorite = m.Priorite
                }).ToList(),
                OffreLangues = dto.OffreLangues.Select(l => new OffreLangue
                {
                    IdOffreEmploi = newId,
                    Langue = l.Langue,
                    NiveauRequis = l.NiveauRequis
                }).ToList(),
                OffreCompetences = dto.OffreCompetences.Select(oc => new OffreCompetences
                {
                    IdOffreEmploi = newId,
                    IdCompetence = oc.IdCompetence,
                    NiveauRequis = oc.NiveauRequis
                }).ToList(),
                DiplomesRequis = await _context.Diplomes
                    .Where(d => dto.DiplomeIds.Contains(d.IdDiplome))
                    .ToListAsync()
            };

            _context.OffresEmploi.Add(offreEmploi);
            await _context.SaveChangesAsync();

            var createdOffreEmploiDto = new OffreEmploiDto
            {
                IdOffreEmploi = newId,
                Specialite = offreEmploi.Specialite,
                DatePublication = offreEmploi.DatePublication,
                DateExpiration = offreEmploi.DateExpiration,
                SalaireMin = offreEmploi.SalaireMin,
                SalaireMax = offreEmploi.SalaireMax,
                NiveauExperienceRequis = offreEmploi.NiveauExperienceRequis,
                TypeContrat = offreEmploi.TypeContrat,
                Statut = offreEmploi.Statut,
                ModeTravail = offreEmploi.ModeTravail,
                EstActif = offreEmploi.estActif,
                Avantages = offreEmploi.Avantages,
                IdRecruteur = offreEmploi.IdRecruteur,
                IdFiliale = offreEmploi.IdFiliale,
                IdDepartement = offreEmploi.IdDepartement,
                Postes = offreEmploi.Postes.Select(p => new PosteDto
                {
                    TitrePoste = p.TitrePoste,
                    Description = p.Description,
                    NombrePostes = p.NombrePostes,
                    ExperienceSouhaitee = p.ExperienceSouhaitee,
                    NiveauHierarchique = p.NiveauHierarchique
                }).ToList(),
                OffreMissions = offreEmploi.OffreMissions.Select(m => new OffreMissionDto
                {
                    DescriptionMission = m.DescriptionMission,
                    Priorite = m.Priorite
                }).ToList(),
                OffreLangues = offreEmploi.OffreLangues.Select(l => new OffreLangueDto
                {
                    Langue = l.Langue,
                    NiveauRequis = l.NiveauRequis
                }).ToList(),
                OffreCompetences = offreEmploi.OffreCompetences.Select(oc => new OffreCompetenceDto
                {
                    IdOffreEmploi = oc.IdOffreEmploi,
                    IdCompetence = oc.IdCompetence,
                    NiveauRequis = oc.NiveauRequis
                }).ToList(),
                DiplomeIds = offreEmploi.DiplomesRequis.Select(d => d.IdDiplome).ToList()
            };

            return CreatedAtAction(nameof(GetOffreEmploi), new { id = newId },
                new { success = true, message = "L'offre d'emploi a été créée avec succès.", offreEmploi = createdOffreEmploiDto });
        }

        // PUT: api/OffreEmplois/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = "Recruteur")]
        public async Task<ActionResult<object>> UpdateOffreEmploi(Guid id, [FromBody] OffreEmploiDto dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new { success = false, message = "Données d'offre invalides." });
                }

                if (id != dto.IdOffreEmploi)
                {
                    return BadRequest(new { success = false, message = "L'identifiant de l'offre ne correspond pas." });
                }

                var offreEmploi = await _context.OffresEmploi
                    .Include(o => o.Postes)
                    .Include(o => o.OffreMissions)
                    .Include(o => o.OffreLangues)
                    .Include(o => o.OffreCompetences)
                    .Include(o => o.DiplomesRequis)
                    .FirstOrDefaultAsync(o => o.IdOffreEmploi == id);

                if (offreEmploi == null)
                {
                    return NotFound(new { success = false, message = "Offre non trouvée." });
                }

                // Vérification du recruteur
                var recruteur = await _userManager.FindByIdAsync(dto.IdRecruteur.ToString());
                if (recruteur == null)
                {
                    return BadRequest(new { success = false, message = "Le recruteur n'existe pas." });
                }

                var isRecruteur = await _userManager.IsInRoleAsync(recruteur, "Recruteur");
                if (!isRecruteur)
                {
                    return StatusCode(403, new { success = false, message = "Seuls les utilisateurs avec le rôle 'Recruteur' peuvent modifier des offres d'emploi." });
                }

                // Vérification que la filiale existe
                var filialeExists = await _context.Filiales.AnyAsync(f => f.IdFiliale == dto.IdFiliale);
                if (!filialeExists)
                {
                    return BadRequest(new { success = false, message = "La filiale spécifiée n'existe pas." });
                }

                // Vérification que le département existe et appartient à la filiale
                var departementExists = await _context.Departements
                    .AnyAsync(d => d.IdDepartement == dto.IdDepartement && d.IdFiliale == dto.IdFiliale);
                if (!departementExists)
                {
                    return BadRequest(new { success = false, message = "Le département spécifié n'existe pas ou n'appartient pas à la filiale." });
                }

                // Validation des règles métier
                if (dto.SalaireMin > dto.SalaireMax)
                {
                    return BadRequest(new { success = false, message = "Le salaire minimum doit être inférieur au salaire maximum." });
                }
                if (dto.DateExpiration <= DateTime.UtcNow)
                {
                    return BadRequest(new { success = false, message = "La date d'expiration doit être postérieure à la date actuelle." });
                }
                if (!Enum.IsDefined(typeof(TypeContratEnum), dto.TypeContrat))
                {
                    return BadRequest(new { success = false, message = "Type de contrat invalide." });
                }
                if (!Enum.IsDefined(typeof(StatutOffre), dto.Statut))
                {
                    return BadRequest(new { success = false, message = "Statut invalide." });
                }
                if (!Enum.IsDefined(typeof(ModeTravail), dto.ModeTravail))
                {
                    return BadRequest(new { success = false, message = "Mode de travail invalide." });
                }
                foreach (var competence in dto.OffreCompetences)
                {
                    if (!Enum.IsDefined(typeof(NiveauRequisType), competence.NiveauRequis))
                    {
                        return BadRequest(new { success = false, message = $"Niveau requis '{competence.NiveauRequis}' invalide." });
                    }
                    if (!await _context.Competences.AnyAsync(c => c.Id == competence.IdCompetence))
                    {
                        return BadRequest(new { success = false, message = $"La compétence avec ID {competence.IdCompetence} n'existe pas." });
                    }
                }
                foreach (var langue in dto.OffreLangues)
                {
                    if (!Enum.IsDefined(typeof(Langue), langue.Langue))
                    {
                        return BadRequest(new { success = false, message = $"Langue '{langue.Langue}' invalide." });
                    }
                }
                foreach (var diplomeId in dto.DiplomeIds)
                {
                    if (!await _context.Diplomes.AnyAsync(d => d.IdDiplome == diplomeId))
                    {
                        return BadRequest(new { success = false, message = $"Le diplôme avec ID {diplomeId} n'existe pas." });
                    }
                }

                // Mise à jour des champs principaux
                offreEmploi.Specialite = dto.Specialite;
                offreEmploi.DateExpiration = dto.DateExpiration;
                offreEmploi.SalaireMin = dto.SalaireMin;
                offreEmploi.SalaireMax = dto.SalaireMax;
                offreEmploi.NiveauExperienceRequis = dto.NiveauExperienceRequis;
                offreEmploi.TypeContrat = dto.TypeContrat;
                offreEmploi.Statut = dto.Statut;
                offreEmploi.ModeTravail = dto.ModeTravail;
                offreEmploi.estActif = dto.EstActif;
                offreEmploi.Avantages = dto.Avantages;
                offreEmploi.IdRecruteur = dto.IdRecruteur;
                offreEmploi.IdFiliale = dto.IdFiliale;
                offreEmploi.IdDepartement = dto.IdDepartement;

                // Mise à jour des collections associées
                // Postes
                var postesToRemove = offreEmploi.Postes
                    .Where(p => !dto.Postes.Any(dp => dp.TitrePoste == p.TitrePoste))
                    .ToList();
                foreach (var poste in postesToRemove)
                {
                    _context.Postes.Remove(poste);
                }
                foreach (var posteDto in dto.Postes)
                {
                    var existingPoste = offreEmploi.Postes
                        .FirstOrDefault(p => p.TitrePoste == posteDto.TitrePoste);
                    if (existingPoste == null)
                    {
                        offreEmploi.Postes.Add(new Poste
                        {
                            IdOffreEmploi = id,
                            TitrePoste = posteDto.TitrePoste,
                            Description = posteDto.Description,
                            NombrePostes = posteDto.NombrePostes,
                            ExperienceSouhaitee = posteDto.ExperienceSouhaitee,
                            NiveauHierarchique = posteDto.NiveauHierarchique
                        });
                    }
                    else
                    {
                        existingPoste.Description = posteDto.Description;
                        existingPoste.NombrePostes = posteDto.NombrePostes;
                        existingPoste.ExperienceSouhaitee = posteDto.ExperienceSouhaitee;
                        existingPoste.NiveauHierarchique = posteDto.NiveauHierarchique;
                    }
                }

                // OffreMissions
                var missionsToRemove = offreEmploi.OffreMissions
                    .Where(m => !dto.OffreMissions.Any(dm => dm.DescriptionMission == m.DescriptionMission))
                    .ToList();
                foreach (var mission in missionsToRemove)
                {
                    _context.OffreMissions.Remove(mission);
                }
                foreach (var missionDto in dto.OffreMissions)
                {
                    var existingMission = offreEmploi.OffreMissions
                        .FirstOrDefault(m => m.DescriptionMission == missionDto.DescriptionMission);
                    if (existingMission == null)
                    {
                        offreEmploi.OffreMissions.Add(new OffreMission
                        {
                            IdOffreEmploi = id,
                            DescriptionMission = missionDto.DescriptionMission,
                            Priorite = missionDto.Priorite
                        });
                    }
                    else
                    {
                        existingMission.Priorite = missionDto.Priorite;
                    }
                }

                // OffreLangues
                var languesToRemove = offreEmploi.OffreLangues
                    .Where(l => !dto.OffreLangues.Any(dl => dl.Langue == l.Langue))
                    .ToList();
                foreach (var langue in languesToRemove)
                {
                    _context.OffreLangues.Remove(langue);
                }
                foreach (var langueDto in dto.OffreLangues)
                {
                    var existingLangue = offreEmploi.OffreLangues
                        .FirstOrDefault(l => l.Langue == langueDto.Langue);
                    if (existingLangue == null)
                    {
                        offreEmploi.OffreLangues.Add(new OffreLangue
                        {
                            IdOffreEmploi = id,
                            Langue = langueDto.Langue,
                            NiveauRequis = langueDto.NiveauRequis
                        });
                    }
                    else
                    {
                        existingLangue.NiveauRequis = langueDto.NiveauRequis;
                    }
                }

                // OffreCompetences
                var competencesToRemove = offreEmploi.OffreCompetences
                    .Where(c => !dto.OffreCompetences.Any(dc => dc.IdCompetence == c.IdCompetence))
                    .ToList();
                foreach (var competence in competencesToRemove)
                {
                    _context.OffreCompetences.Remove(competence);
                }
                foreach (var competenceDto in dto.OffreCompetences)
                {
                    var existingCompetence = offreEmploi.OffreCompetences
                        .FirstOrDefault(c => c.IdCompetence == competenceDto.IdCompetence);
                    if (existingCompetence == null)
                    {
                        offreEmploi.OffreCompetences.Add(new OffreCompetences
                        {
                            IdOffreEmploi = id,
                            IdCompetence = competenceDto.IdCompetence,
                            NiveauRequis = competenceDto.NiveauRequis
                        });
                    }
                    else
                    {
                        existingCompetence.NiveauRequis = competenceDto.NiveauRequis;
                    }
                }

                // DiplomesRequis
                var diplomesToRemove = offreEmploi.DiplomesRequis
                    .Where(d => !dto.DiplomeIds.Contains(d.IdDiplome))
                    .ToList();
                foreach (var diplome in diplomesToRemove)
                {
                    offreEmploi.DiplomesRequis.Remove(diplome);
                }
                var diplomesToAdd = await _context.Diplomes
                    .Where(d => dto.DiplomeIds.Contains(d.IdDiplome))
                    .ToListAsync();
                foreach (var diplome in diplomesToAdd)
                {
                    if (!offreEmploi.DiplomesRequis.Any(d => d.IdDiplome == diplome.IdDiplome))
                    {
                        offreEmploi.DiplomesRequis.Add(diplome);
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Offre d'emploi mise à jour avec succès.",
                    data = new { id = offreEmploi.IdOffreEmploi }
                });
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(409, new { success = false, message = "Conflit de concurrence : l'offre a été modifiée ou supprimée par un autre utilisateur.", detail = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Erreur lors de la mise à jour.", detail = ex.Message });
            }
        }

        // DELETE: api/OffreEmplois/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Recruteur")]
        public async Task<IActionResult> DeleteOffreEmploi(Guid id)
        {
            var offreEmploi = await _context.OffresEmploi.FindAsync(id);
            if (offreEmploi == null)
            {
                return NotFound(new { success = false, message = "L'offre d'emploi spécifiée n'existe pas." });
            }

            _context.OffresEmploi.Remove(offreEmploi);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "L'offre d'emploi a été supprimée avec succès." });
        }

        // GET: api/OffreEmplois/search
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> Search(
            [FromQuery] string titrePoste = null,
            [FromQuery] string specialite = null,
            [FromQuery] string typeContrat = null,
            [FromQuery] string statut = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(titrePoste) &&
                    string.IsNullOrWhiteSpace(specialite) &&
                    string.IsNullOrWhiteSpace(typeContrat) &&
                    string.IsNullOrWhiteSpace(statut))
                {
                    return BadRequest(new { success = false, message = "Au moins un critère de recherche (titre, spécialité, type de contrat ou statut) doit être fourni." });
                }

                var query = _context.OffresEmploi
                    .Include(o => o.Postes)
                    .Include(o => o.OffreMissions)
                    .Include(o => o.OffreLangues)
                    .Include(o => o.OffreCompetences).ThenInclude(oc => oc.Competence)
                    .Include(o => o.DiplomesRequis)
                    .Include(o => o.Filiale)
                    .Include(o => o.Departement)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(titrePoste))
                {
                    query = query.Where(o => o.Postes.Any(p => p.TitrePoste.ToLower().Contains(titrePoste.ToLower())));
                }
                if (!string.IsNullOrWhiteSpace(specialite))
                {
                    query = query.Where(o => o.Specialite.ToLower().Contains(specialite.ToLower()));
                }
                if (!string.IsNullOrWhiteSpace(typeContrat))
                {
                    query = query.Where(o => o.TypeContrat.ToString().ToLower().Contains(typeContrat.ToLower()));
                }
                if (!string.IsNullOrWhiteSpace(statut))
                {
                    query = query.Where(o => o.Statut.ToString().ToLower().Contains(statut.ToLower()));
                }

                var offresEmploi = await query.ToListAsync();

                if (!offresEmploi.Any())
                {
                    return NotFound(new { success = false, message = "Aucune offre trouvée avec ces critères." });
                }

                var offresEmploiDto = offresEmploi.Select(offre => new OffreEmploiDto
                {
                    IdOffreEmploi = offre.IdOffreEmploi,
                    Specialite = offre.Specialite,
                    DatePublication = offre.DatePublication,
                    DateExpiration = offre.DateExpiration,
                    SalaireMin = offre.SalaireMin,
                    SalaireMax = offre.SalaireMax,
                    NiveauExperienceRequis = offre.NiveauExperienceRequis,
                    TypeContrat = offre.TypeContrat,
                    Statut = offre.Statut,
                    ModeTravail = offre.ModeTravail,
                    EstActif = offre.estActif,
                    Avantages = offre.Avantages,
                    IdRecruteur = offre.IdRecruteur,
                    IdFiliale = offre.IdFiliale,
                    IdDepartement = offre.IdDepartement,
                    Postes = offre.Postes.Select(p => new PosteDto
                    {
                        TitrePoste = p.TitrePoste,
                        Description = p.Description,
                        NombrePostes = p.NombrePostes,
                        ExperienceSouhaitee = p.ExperienceSouhaitee,
                        NiveauHierarchique = p.NiveauHierarchique
                    }).ToList(),
                    OffreMissions = offre.OffreMissions.Select(m => new OffreMissionDto
                    {
                        DescriptionMission = m.DescriptionMission,
                        Priorite = m.Priorite
                    }).ToList(),
                    OffreLangues = offre.OffreLangues.Select(l => new OffreLangueDto
                    {
                        Langue = l.Langue,
                        NiveauRequis = l.NiveauRequis
                    }).ToList(),
                    OffreCompetences = offre.OffreCompetences.Select(oc => new OffreCompetenceDto
                    {
                        IdOffreEmploi = oc.IdOffreEmploi,
                        IdCompetence = oc.IdCompetence,
                        NiveauRequis = oc.NiveauRequis,
                        Competence = new CompetenceDto
                        {
                            Id = oc.Competence.Id,
                            Nom = oc.Competence.Nom,
                            Description = oc.Competence.Description,
                            DateModification = oc.Competence.DateModification,
                            EstTechnique = oc.Competence.estTechnique,
                            EstSoftSkill = oc.Competence.estSoftSkill
                        }
                    }).ToList(),
                    DiplomeIds = offre.DiplomesRequis.Select(d => d.IdDiplome).ToList()
                }).ToList();

                return Ok(new { success = true, message = "Offres d'emploi trouvées avec succès.", offresEmploi = offresEmploiDto });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Une erreur est survenue lors de la recherche.", detail = ex.Message });
            }
        }
        // GET: api/OffreEmplois/by-filiale/{idFiliale}
        [HttpGet("by-filiale/{idFiliale}")]
        [AllowAnonymous] // Peut être restreint si nécessaire
        public async Task<ActionResult<object>> GetOffresByFiliale(Guid idFiliale)
        {
            try
            {
                // Vérifier si la filiale existe
                var filialeExists = await _context.Filiales.AnyAsync(f => f.IdFiliale == idFiliale);
                if (!filialeExists)
                {
                    return BadRequest(new { success = false, message = "La filiale spécifiée n'existe pas." });
                }

                // Récupérer les offres pour la filiale spécifiée
                var offresEmploi = await _context.OffresEmploi
                    .Where(o => o.IdFiliale == idFiliale)
                    .Include(o => o.Postes)
                    .Include(o => o.OffreMissions)
                    .Include(o => o.OffreLangues)
                    .Include(o => o.OffreCompetences).ThenInclude(oc => oc.Competence)
                    .Include(o => o.DiplomesRequis)
                    .Include(o => o.Filiale)
                    .Include(o => o.Departement)
                    .ToListAsync();

                if (!offresEmploi.Any())
                {
                    return NotFound(new { success = false, message = "Aucune offre d'emploi trouvée pour cette filiale." });
                }

                // Mapper vers OffreEmploiDto
                var offresEmploiDto = offresEmploi.Select(offre => new OffreEmploiDto
                {
                    IdOffreEmploi = offre.IdOffreEmploi,
                    Specialite = offre.Specialite,
                    DatePublication = offre.DatePublication,
                    DateExpiration = offre.DateExpiration,
                    SalaireMin = offre.SalaireMin,
                    SalaireMax = offre.SalaireMax,
                    NiveauExperienceRequis = offre.NiveauExperienceRequis,
                    TypeContrat = offre.TypeContrat,
                    Statut = offre.Statut,
                    ModeTravail = offre.ModeTravail,
                    EstActif = offre.estActif,
                    Avantages = offre.Avantages,
                    IdRecruteur = offre.IdRecruteur,
                    IdFiliale = offre.IdFiliale,
                    IdDepartement = offre.IdDepartement,
                    Postes = offre.Postes.Select(p => new PosteDto
                    {
                        TitrePoste = p.TitrePoste,
                        Description = p.Description,
                        NombrePostes = p.NombrePostes,
                        ExperienceSouhaitee = p.ExperienceSouhaitee,
                        NiveauHierarchique = p.NiveauHierarchique
                    }).ToList(),
                    OffreMissions = offre.OffreMissions.Select(m => new OffreMissionDto
                    {
                        DescriptionMission = m.DescriptionMission,
                        Priorite = m.Priorite
                    }).ToList(),
                    OffreLangues = offre.OffreLangues.Select(l => new OffreLangueDto
                    {
                        Langue = l.Langue,
                        NiveauRequis = l.NiveauRequis
                    }).ToList(),
                    OffreCompetences = offre.OffreCompetences.Select(oc => new OffreCompetenceDto
                    {
                        IdOffreEmploi = oc.IdOffreEmploi,
                        IdCompetence = oc.IdCompetence,
                        NiveauRequis = oc.NiveauRequis,
                        Competence = new CompetenceDto
                        {
                            Id = oc.Competence.Id,
                            Nom = oc.Competence.Nom,
                            Description = oc.Competence.Description,
                            DateModification = oc.Competence.DateModification,
                            EstTechnique = oc.Competence.estTechnique,
                            EstSoftSkill = oc.Competence.estSoftSkill
                        }
                    }).ToList(),
                    DiplomeIds = offre.DiplomesRequis.Select(d => d.IdDiplome).ToList()
                }).ToList();

                return Ok(new
                {
                    success = true,
                    message = "Offres d'emploi récupérées avec succès pour la filiale.",
                    offresEmploi = offresEmploiDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Une erreur est survenue lors de la récupération des offres.",
                    detail = ex.Message
                });
            }
        }

        // GET: api/OffreEmplois/recruteurs
        [HttpGet("recruteurs")]
        [AllowAnonymous] // Peut être restreint à certains rôles si nécessaire
     
        public async Task<IActionResult> GetRecruteurs()
        {
            var recruteurs = await _userManager.GetUsersInRoleAsync("Recruteur");
            var result = recruteurs.Select(u => new { u.Id, u.UserName, u.Email }).ToList();
            return Ok(new { Success = true, Message = "Recruteurs récupérés", Recruteurs = result });
        }
        private bool OffreEmploiExists(Guid id)
        {
            return _context.OffresEmploi.Any(e => e.IdOffreEmploi == id);
        }
    }
}