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
using Mono.TextTemplating;
using System.Security.Claims;



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

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<object>> CreateOffreEmploi([FromBody] CreateOffreEmploiRequest request)
        {
            if (request?.Dto == null)
            {
                return BadRequest(new { success = false, message = "Données invalides." });
            }

            var dto = request.Dto;

            // Validation logic
            if (!await _context.Filiales.AnyAsync(f => f.IdFiliale == dto.IdFiliale))
            {
                return BadRequest(new { success = false, message = "La filiale spécifiée n'existe pas." });
            }
            if (!await _context.Departements.AnyAsync(d => d.IdDepartement == dto.IdDepartement))
            {
                return BadRequest(new { success = false, message = "Le département spécifié n'existe pas." });
            }
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

            if (dto.SalaireMin > dto.SalaireMax)
            {
                return BadRequest(new { success = false, message = "Le salaire minimum ne peut pas être supérieur au salaire maximum." });
            }
            if (dto.DateExpiration <= DateTime.UtcNow)
            {
                return BadRequest(new { success = false, message = "La date d'expiration doit être dans le futur." });
            }

            // Validate that all IdCompetence values exist in the Competences table
            foreach (var competence in dto.OffreCompetences)
            {
                if (!await _context.Competences.AnyAsync(c => c.Id == competence.IdCompetence))
                {
                    return BadRequest(new { success = false, message = $"La compétence avec ID {competence.IdCompetence} n'existe pas." });
                }
            }

            // Validate that all DiplomeIds exist in the Diplomes table
            foreach (var diplomeId in dto.DiplomeIds)
            {
                if (!await _context.Diplomes.AnyAsync(d => d.IdDiplome == diplomeId))
                {
                    return BadRequest(new { success = false, message = $"Le diplôme avec ID {diplomeId} n'existe pas." });
                }
            }

            var newId = Guid.NewGuid(); // Automatically generated IdOffreEmploi
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

            // Reload the entity with related data to avoid NullReferenceException
            offreEmploi = await _context.OffresEmploi
                .Include(o => o.Postes)
                .Include(o => o.OffreMissions)
                .Include(o => o.OffreLangues)
                .Include(o => o.OffreCompetences)
                    .ThenInclude(oc => oc.Competence) // Ensure Competence is loaded
                .Include(o => o.DiplomesRequis)
                .FirstOrDefaultAsync(o => o.IdOffreEmploi == newId);

            if (offreEmploi == null)
            {
                return StatusCode(500, new { success = false, message = "Erreur lors de la récupération de l'offre après création." });
            }

            var createdOffreEmploiDto = new OffreEmploiDto
            {
                IdOffreEmploi = offreEmploi.IdOffreEmploi, // Automatically generated ID
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
                    IdOffreEmploi = m.IdOffreEmploi,
                    DescriptionMission = m.DescriptionMission,
                    Priorite = m.Priorite
                }).ToList(),
                OffreLangues = offreEmploi.OffreLangues.Select(l => new OffreLangueDto
                {
                    IdOffreEmploi = l.IdOffreEmploi,
                    Langue = l.Langue,
                    NiveauRequis = l.NiveauRequis
                }).ToList(),
                OffreCompetences = offreEmploi.OffreCompetences.Select(oc => new OffreCompetenceDto
                {
                    IdOffreEmploi = oc.IdOffreEmploi,
                    IdCompetence = oc.IdCompetence,
                    NiveauRequis = oc.NiveauRequis,
                    Competence = oc.Competence == null ? null : new CompetenceDto
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

            return CreatedAtAction(nameof(GetOffreEmploi), new { id = newId },
                new { success = true, message = "L'offre d'emploi a été créée avec succès.", offreEmploi = createdOffreEmploiDto });
        }

        // PUT: api/OffreEmplois/{id}
        [HttpPut("{id}")]
        [AllowAnonymous]
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
                    .Include(o => o.OffreCompetences).ThenInclude(oc => oc.Competence)
                    .Include(o => o.DiplomesRequis)
                    .FirstOrDefaultAsync(o => o.IdOffreEmploi == id);

                if (offreEmploi == null)
                {
                    return NotFound(new { success = false, message = "Offre non trouvée." });
                }

                // Validation logic (unchanged)...

                // Update main fields
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

                // Update related collections
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

        [HttpDelete("{id}")]

        [AllowAnonymous]
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

        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> Search(
        [FromQuery] string titrePoste = null,
        [FromQuery] string specialite = null,
        [FromQuery] string typeContrat = null,
        [FromQuery] string statut = null,
        [FromQuery] string niveauExperienceRequis = null,
        [FromQuery] int? idFiliale = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(titrePoste) &&
                    string.IsNullOrWhiteSpace(specialite) &&
                    string.IsNullOrWhiteSpace(typeContrat) &&
                    string.IsNullOrWhiteSpace(statut) &&
                    string.IsNullOrWhiteSpace(niveauExperienceRequis) &&
                    !idFiliale.HasValue)
                {
                    return BadRequest(new { success = false, message = "Au moins un critère de recherche (titre, spécialité, type de contrat, statut, niveau d'expérience ou filiale) doit être fourni." });
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
                if (!string.IsNullOrWhiteSpace(niveauExperienceRequis))
                {
                    query = query.Where(o => o.NiveauExperienceRequis.ToLower().Contains(niveauExperienceRequis.ToLower()));
                }
                //if (idFiliale.HasValue)
                //{
                //    query = query.Where(o => o.IdFiliale == idFiliale.Value);
                //}

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


        // GET: api/JobBoard/Statuts
        [HttpGet("Statuts")]
        [AllowAnonymous] // Peut être restreint à certains rôles si nécessaire
        public IActionResult GetStatuts()
        {
            var statuts = Enum.GetValues(typeof(StatutOffre))
                .Cast<StatutOffre>()
                .Select(s => new { Value = (int)s, Name = s.ToString() });
            return Ok(new { Success = true, Message = "Statuts récupérés", Data = statuts });
        }

        // GET: api/JobBoard/TypesContrat
        [HttpGet("TypesContrat")]
        [AllowAnonymous] // Peut être restreint à certains rôles si nécessaire

        public IActionResult GetTypesContrat()
        {
            var types = Enum.GetValues(typeof(TypeContratEnum))
                .Cast<TypeContratEnum>()
                .Select(t => new { Value = (int)t, Name = t.ToString() });
            return Ok(new { Success = true, Message = "Types de contrat récupérés", Data = types });
        }

        // GET: api/JobBoard/ModesTravail
        [HttpGet("ModesTravail")]
        [AllowAnonymous] // Peut être restreint à certains rôles si nécessaire

        public IActionResult GetModesTravail()
        {
            var modes = Enum.GetValues(typeof(ModeTravail))
                .Cast<ModeTravail>()
                .Select(m => new { Value = (int)m, Name = m.ToString() });
            return Ok(new { Success = true, Message = "Modes de travail récupérés", Data = modes });
        }

        // GET: api/JobBoard/NiveauxRequis
        [HttpGet("NiveauxRequis")]
        [AllowAnonymous] // Peut être restreint à certains rôles si nécessaire

        public IActionResult GetNiveauxRequis()
        {
            var niveaux = Enum.GetValues(typeof(NiveauRequisType))
                .Cast<NiveauRequisType>()
                .Select(n => n.ToString());
            return Ok(new { Success = true, Message = "Niveaux requis récupérés", Data = niveaux });
        }

        // GET: api/JobBoard/Competences
        [HttpGet("Competences")]
        [AllowAnonymous] // Peut être restreint à certains rôles si nécessaire

        public async Task<IActionResult> GetCompetences()
        {
            var competences = await _context.Competences.ToListAsync();
            return Ok(new { Success = true, Message = "Compétences récupérées", Data = competences });
        }


        // GET: api/OffreEmplois/recruteurs
        [HttpGet("recruteurs")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRecruteurs()
        {
            var recruteurs = await _userManager.GetUsersInRoleAsync("Recruteur");
            var result = recruteurs.Select(u => new
            {
                id = u.Id,           // camelCase pour Angular
                email = u.Email,      // camelCase
                fullName = u.FullName // camelCase
            }).ToList();

            return Ok(new
            {
                success = true,
                message = "Recruteurs récupérés",
                recruteurs = result
            });
        }
        /********************************************************************/
        [HttpGet("by-recruteur-filiale/{idFiliale}")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetOffresByRecruteurOfFiliale(Guid idFiliale)
        {
            try
            {
                // Vérifier si la filiale existe
                var filialeExists = await _context.Filiales.AnyAsync(f => f.IdFiliale == idFiliale);
                if (!filialeExists)
                {
                    return BadRequest(new { success = false, message = "La filiale spécifiée n'existe pas." });
                }

                // Récupérer tous les recruteurs appartenant à cette filiale
                var recruteursIds = await _userManager.GetUsersInRoleAsync("Recruteur")
                    .ContinueWith(task => task.Result
                        .Where(u => u.IdFiliale == idFiliale) // Supposons que AppUser a une propriété IdFiliale
                        .Select(u => u.Id)
                        .ToList());

                if (!recruteursIds.Any())
                {
                    return NotFound(new { success = false, message = "Aucun recruteur trouvé pour cette filiale." });
                }

                // Récupérer les offres d'emploi créées par ces recruteurs
                var offresEmploi = await _context.OffresEmploi
                    .Include(o => o.Postes)
                    .Include(o => o.OffreMissions)
                    .Include(o => o.OffreLangues)
                    .Include(o => o.OffreCompetences).ThenInclude(oc => oc.Competence)
                    .Include(o => o.DiplomesRequis)
                    .Include(o => o.Filiale)
                    .Include(o => o.Departement)
                    .Where(o => recruteursIds.Contains(o.IdRecruteur))
                    .ToListAsync();

                if (!offresEmploi.Any())
                {
                    return NotFound(new { success = false, message = "Aucune offre d'emploi trouvée pour les recruteurs de cette filiale." });
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
                    message = "Offres d'emploi récupérées avec succès pour les recruteurs de la filiale.",
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
       


        [HttpGet("by-recruteur/{idRecruteur}")]
        [AllowAnonymous] // Peut être restreint à certains rôles si nécessa
        public async Task<ActionResult<object>> GetOffresByRecruteur(string idRecruteur)
        {
            try
            {
                // Vérifier si le recruteur existe
                var recruiter = await _userManager.FindByIdAsync(idRecruteur);
                if (recruiter == null)
                {
                    return BadRequest(new { success = false, message = "Le recruteur spécifié n'existe pas." });
                }

                // Vérifier si le recruteur a le rôle "Recruteur" (optionnel, peut être retiré si non requis)
                var isRecruteur = await _userManager.IsInRoleAsync(recruiter, "Recruteur");
                if (!isRecruteur)
                {
                    return BadRequest(new { success = false, message = "L'utilisateur spécifié n'a pas le rôle Recruteur." });
                }

                // Convertir idRecruteur (string) en Guid (si IdRecruteur est un Guid dans OffreEmploi)
                if (!Guid.TryParse(idRecruteur, out var recruteurGuid))
                {
                    return BadRequest(new { success = false, message = "L'ID du recruteur n'est pas un Guid valide." });
                }

                // Récupérer les offres d'emploi du recruteur
                var offresEmploi = await _context.OffresEmploi
                    .Where(o => o.IdRecruteur == recruteurGuid) // Comparaison avec Guid
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
                    return NotFound(new { success = false, message = "Aucune offre d'emploi trouvée pour ce recruteur." });
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
                    IdRecruteur = offre.IdRecruteur, // Convertir Guid en string pour le DTO
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
                    message = "Offres d'emploi récupérées avec succès pour le recruteur.",
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


        // Dans votre OffreEmploiController.cs ou un nouveau ReferenceController.cs

        // Dans OffreEmploisController.cs

        /// <summary>
        /// Récupère toutes les spécialités distinctes existantes dans les offres d'emploi
        /// </summary>
        [HttpGet("specialites")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetSpecialites()
        {
            try
            {
                var specialites = await _context.OffresEmploi
                    .Where(o => !string.IsNullOrEmpty(o.Specialite))
                    .Select(o => o.Specialite)
                    .Distinct()
                    .OrderBy(s => s)
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    message = "Spécialités récupérées avec succès.",
                    specialites = specialites
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erreur lors de la récupération des spécialités.",
                    error = ex.Message
                });
            }
        }
        [HttpGet("actives")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetOffresActives()
        {
            try
            {
                var offresActives = await _context.OffresEmploi
                    .Where(o => o.estActif && o.DateExpiration >= DateTime.UtcNow)
                    .Include(o => o.Postes)
                    .Include(o => o.Filiale)
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    message = "Offres actives récupérées",
                    offresEmploi = offresActives
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erreur lors de la récupération des offres actives",
                    detail = ex.Message
                });
            }
        }
        [HttpGet("exists/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> OffreExists(Guid id)
        {
            try
            {
                var exists = await _context.OffresEmploi.AnyAsync(o => o.IdOffreEmploi == id);
                return Ok(new
                {
                    success = true,
                    exists = exists
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erreur lors de la vérification",
                    detail = ex.Message
                });
            }
        }

        [HttpGet("statistiques")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetStatistics()
        {
            try
            {
                var total = await _context.OffresEmploi.CountAsync();
                var actives = await _context.OffresEmploi.CountAsync(o => o.estActif);
                var expirees = await _context.OffresEmploi.CountAsync(o => o.DateExpiration < DateTime.UtcNow);

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        total,
                        actives,
                        expirees
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erreur lors de la récupération des statistiques",
                    detail = ex.Message
                });
            }
        }
        [HttpPatch("{id}/toggle-activation")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> ToggleActivation(Guid id)
        {
            try
            {
                var offre = await _context.OffresEmploi.FindAsync(id);
                if (offre == null)
                {
                    return NotFound(new { success = false, message = "Offre non trouvée" });
                }

                offre.estActif = !offre.estActif;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = $"Offre {(offre.estActif ? "activée" : "désactivée")} avec succès",
                    isActive = offre.estActif
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erreur lors de la modification du statut",
                    detail = ex.Message
                });
            }
        }

        /// <summary>
        /// Récupère tous les niveaux d'expérience distincts existants dans les offres d'emploi
        /// </summary>
        [HttpGet("niveaux-experience")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetNiveauxExperience()
        {
            try
            {
                var niveaux = await _context.OffresEmploi
                    .Where(o => !string.IsNullOrEmpty(o.NiveauExperienceRequis))
                    .Select(o => o.NiveauExperienceRequis)
                    .Distinct()
                    .OrderBy(n => n)
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    message = "Niveaux d'expérience récupérés avec succès.",
                    niveauxExperience = niveaux
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Erreur lors de la récupération des niveaux d'expérience.",
                    error = ex.Message
                });
            }
        }
        private ActionResult<object> Forbid(object value)
        {
            throw new NotImplementedException();
        }

        private bool OffreEmploiExists(Guid id)
        {
            return _context.OffresEmploi.Any(e => e.IdOffreEmploi == id);
        }
    }
}

