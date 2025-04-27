using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_poulina_future_jobs.Models;
using Backend_poulina_future_jobs.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Humanizer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

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
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        // GET: api/OffreEmplois
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetAllOffreEmplois()
        {
            try
            {
                var offresEmploi = await _context.OffresEmploi
                    .Include(o => o.OffreCompetences)
                        .ThenInclude(oc => oc.Competence)
                    .ToListAsync();

                if (!offresEmploi.Any())
                {
                    return NotFound(new { message = "Aucune offre d'emploi trouvée." });
                }

                var offresEmploiDTO = offresEmploi.Select(offreEmploi => new OffreEmploiDTO
                {
                    IdOffreEmploi = offreEmploi.IdOffreEmploi,
                    Titre = offreEmploi.Titre,
                    Specialite = offreEmploi.Specialite,
                    Description = offreEmploi.Description,
                    DatePublication = offreEmploi.DatePublication,
                    DateExpiration = offreEmploi.DateExpiration,
                    SalaireMin = offreEmploi.SalaireMin,
                    SalaireMax = offreEmploi.SalaireMax,
                    NiveauExperienceRequis = offreEmploi.NiveauExperienceRequis,
                    DiplomeRequis = offreEmploi.DiplomeRequis,
                    //TypeContrat = offreEmploi.TypeContrat,
                    Statut = offreEmploi.Statut,
                    ModeTravail = offreEmploi.ModeTravail,
                    NombrePostes = offreEmploi.NombrePostes,
                    Avantages = offreEmploi.Avantages,
                    IdRecruteur = offreEmploi.IdRecruteur,
                    IdFiliale = offreEmploi.IdFiliale,
                    OffreCompetences = offreEmploi.OffreCompetences.Select(oc => new OffreCompetenceDTO
                    {
                        IdOffreEmploi = oc.IdOffreEmploi,
                        IdCompetence = oc.IdCompetence,
                        NiveauRequis = oc.NiveauRequis,
                        Competence = new CompetenceCreateDto
                        {
                            Id = oc.Competence.Id,
                            Nom = oc.Competence.Nom,
                            Description = oc.Competence.Description,
                            EstTechnique = oc.Competence.estTechnique,
                            EstSoftSkill = oc.Competence.estSoftSkill
                        }
                    }).ToList()
                }).ToList();

                return Ok(new { message = "Liste des offres d'emploi récupérée avec succès.", offresEmploi = offresEmploiDTO });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue.", detail = ex.Message });
            }
        }

        // GET: api/OffreEmplois/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetOffreEmploi(Guid id)
        {
            try
            {
                var offreEmploi = await _context.OffresEmploi
                    .Include(o => o.OffreCompetences)
                        .ThenInclude(oc => oc.Competence)
                    .FirstOrDefaultAsync(o => o.IdOffreEmploi == id);

                if (offreEmploi == null)
                {
                    return NotFound(new { message = "L'offre d'emploi demandée n'existe pas." });
                }

                var offreEmploiDTO = new OffreEmploiDTO
                {
                    IdOffreEmploi = offreEmploi.IdOffreEmploi,
                    Titre = offreEmploi.Titre,
                    Specialite = offreEmploi.Specialite,
                    Description = offreEmploi.Description,
                    DatePublication = offreEmploi.DatePublication,
                    DateExpiration = offreEmploi.DateExpiration,
                    SalaireMin = offreEmploi.SalaireMin,
                    SalaireMax = offreEmploi.SalaireMax,
                    NiveauExperienceRequis = offreEmploi.NiveauExperienceRequis,
                    DiplomeRequis = offreEmploi.DiplomeRequis,
                    //TypeContrat = offreEmploi.TypeContrat,
                    Statut = offreEmploi.Statut,
                    ModeTravail = offreEmploi.ModeTravail,
                    NombrePostes = offreEmploi.NombrePostes,
                    Avantages = offreEmploi.Avantages,
                    IdRecruteur = offreEmploi.IdRecruteur,
                    IdFiliale = offreEmploi.IdFiliale,
                    OffreCompetences = offreEmploi.OffreCompetences.Select(oc => new OffreCompetenceDTO
                    {
                        IdOffreEmploi = oc.IdOffreEmploi,
                        IdCompetence = oc.IdCompetence,
                        NiveauRequis = oc.NiveauRequis,
                        Competence = new CompetenceCreateDto
                        {
                            Id = oc.Competence.Id,
                            Nom = oc.Competence.Nom,
                            Description = oc.Competence.Description,
                            EstTechnique = oc.Competence.estTechnique,
                            EstSoftSkill = oc.Competence.estSoftSkill
                        }
                    }).ToList()
                };

                return Ok(new { message = "Offre d'emploi récupérée avec succès.", offreEmploi = offreEmploiDTO });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue.", detail = ex.Message });
            }
        }

        
     
        //PUT: api/OffreEmplois/{id}
            [HttpPut("{id}")]
        [AllowAnonymous]

        public async Task<ActionResult<object>> UpdateOffreEmploi(Guid id, [FromBody] OffreEmploiDTO dto)
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

                var offre = await _context.OffresEmploi
                    .Include(o => o.OffreCompetences)
                    .FirstOrDefaultAsync(o => o.IdOffreEmploi == id);

                if (offre == null)
                {
                    return NotFound(new { success = false, message = "Offre non trouvée." });
                }

                // Vérification Recruteur & Role
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

                // Vérification Filiale
                if (!await _context.Filiales.AnyAsync(f => f.IdFiliale == dto.IdFiliale))
                {
                    return BadRequest(new { success = false, message = "La filiale n'existe pas." });
                }

                // Mise à jour des champs de l'offre
                offre.Titre = dto.Titre;
                offre.Specialite = dto.Specialite;
                offre.Description = dto.Description;
                offre.DiplomeRequis = dto.DiplomeRequis;
                offre.NiveauExperienceRequis = dto.NiveauExperienceRequis;
                offre.SalaireMin = dto.SalaireMin;
                offre.SalaireMax = dto.SalaireMax;
                offre.DateExpiration = dto.DateExpiration;
                offre.TypeContrat = dto.TypeContrat;
                offre.Statut = dto.Statut;
                offre.ModeTravail = dto.ModeTravail;
                offre.NombrePostes = dto.NombrePostes;
                offre.Avantages = dto.Avantages;
                offre.IdRecruteur = dto.IdRecruteur;
                offre.IdFiliale = dto.IdFiliale;

                // Suppression des anciennes compétences
                _context.OffreCompetences.RemoveRange(offre.OffreCompetences);
                offre.OffreCompetences.Clear(); // Ensure collection is cleared

                // Ajout des nouvelles compétences
                if (dto.OffreCompetences != null && dto.OffreCompetences.Any())
                {
                    foreach (var oc in dto.OffreCompetences)
                    {
                        if (oc?.Competence == null || string.IsNullOrWhiteSpace(oc.Competence.Nom))
                        {
                            continue;
                        }

                        var existing = await _context.Competences
                            .FirstOrDefaultAsync(c => c.Nom.ToLower() == oc.Competence.Nom.ToLower());

                        Guid competenceId;
                        if (existing != null)
                        {
                            competenceId = existing.Id;
                        }
                        else
                        {
                            var newCompetence = new Competence
                            {
                                Id = Guid.NewGuid(),
                                Nom = oc.Competence.Nom,
                                Description = oc.Competence.Description,
                                estTechnique = oc.Competence.EstTechnique,
                                estSoftSkill = oc.Competence.EstSoftSkill,
                                dateAjout = DateTime.UtcNow,
                                DateModification = DateTime.UtcNow
                            };
                            _context.Competences.Add(newCompetence);
                            competenceId = newCompetence.Id;
                        }

                        offre.OffreCompetences.Add(new OffreCompetences
                        {
                            IdOffreEmploi = offre.IdOffreEmploi,
                            IdCompetence = competenceId,
                            NiveauRequis = oc.NiveauRequis
                        });
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Offre d'emploi mise à jour avec succès.",
                    data = new { id = offre.IdOffreEmploi }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Erreur lors de la mise à jour.", detail = ex.Message });
            }
        }



        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<object>> CreateOffreEmploi([FromBody] OffreEmploiDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new { success = false, message = "Données d'offre invalides." });
                }

                // Vérification Recruteur & Role
                var recruteur = await _userManager.FindByIdAsync(dto.IdRecruteur.ToString());
                if (recruteur == null)
                {
                    return BadRequest(new { success = false, message = "Le recruteur n'existe pas." });
                }

                var isRecruteur = await _userManager.IsInRoleAsync(recruteur, "Recruteur");
                if (!isRecruteur)
                {
                    return StatusCode(403, new { success = false, message = "Seuls les utilisateurs avec le rôle 'Recruteur' peuvent créer des offres d'emploi." });
                }

                // Vérification Filiale
                if (!await _context.Filiales.AnyAsync(f => f.IdFiliale == dto.IdFiliale))
                {
                    return BadRequest(new { success = false, message = "La filiale n'existe pas." });
                }

                var offre = new OffreEmploi
                {
                    IdOffreEmploi = Guid.NewGuid(),
                    Titre = dto.Titre,
                    Specialite = dto.Specialite,
                    Description = dto.Description,
                    DiplomeRequis = dto.DiplomeRequis,
                    NiveauExperienceRequis = dto.NiveauExperienceRequis,
                    SalaireMin = dto.SalaireMin,
                    SalaireMax = dto.SalaireMax,
                    DatePublication = DateTime.UtcNow,
                    DateExpiration = dto.DateExpiration,
                    TypeContrat = dto.TypeContrat,
                    Statut = dto.Statut,
                    ModeTravail = dto.ModeTravail,
                    NombrePostes = dto.NombrePostes,
                    Avantages = dto.Avantages,
                    IdRecruteur = dto.IdRecruteur,
                    IdFiliale = dto.IdFiliale,
                    OffreCompetences = new List<OffreCompetences>()
                };

                // Associer les compétences si présentes
                if (dto.OffreCompetences != null && dto.OffreCompetences.Any())
                {
                    foreach (var oc in dto.OffreCompetences)
                    {
                        if (oc?.Competence == null || string.IsNullOrWhiteSpace(oc.Competence.Nom))
                        {
                            continue;
                        }

                        var existing = await _context.Competences
                            .FirstOrDefaultAsync(c => c.Nom.ToLower() == oc.Competence.Nom.ToLower());

                        Guid competenceId;
                        if (existing != null)
                        {
                            competenceId = existing.Id;
                        }
                        else
                        {
                            var newCompetence = new Competence
                            {
                                Id = Guid.NewGuid(),
                                Nom = oc.Competence.Nom,
                                Description = oc.Competence.Description,
                                estTechnique = oc.Competence.EstTechnique,
                                estSoftSkill = oc.Competence.EstSoftSkill,
                                dateAjout = DateTime.UtcNow,
                                DateModification = DateTime.UtcNow
                            };
                            _context.Competences.Add(newCompetence);
                            competenceId = newCompetence.Id;
                        }

                        offre.OffreCompetences.Add(new OffreCompetences
                        {
                            IdOffreEmploi = offre.IdOffreEmploi,
                            IdCompetence = competenceId,
                            NiveauRequis = oc.NiveauRequis
                        });
                    }
                }

                _context.OffresEmploi.Add(offre);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetOffreEmploi), new { id = offre.IdOffreEmploi }, new
                {
                    success = true,
                    message = "Offre créée avec succès.",
                    data = new { id = offre.IdOffreEmploi }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Erreur lors de la création.", detail = ex.Message });
            }
        }
        // DELETE: api/OffreEmplois/{id}
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> DeleteOffreEmploi(Guid id)
        {
            try
            {
                var offreEmploi = await _context.OffresEmploi
                    .Include(o => o.OffreCompetences)
                    .FirstOrDefaultAsync(o => o.IdOffreEmploi == id);

                if (offreEmploi == null)
                {
                    return NotFound(new { message = "L'offre d'emploi spécifiée n'existe pas." });
                }

                _context.OffreCompetences.RemoveRange(offreEmploi.OffreCompetences);
                _context.OffresEmploi.Remove(offreEmploi);

                await _context.SaveChangesAsync();

                return Ok(new { message = "Offre d'emploi supprimée avec succès." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue.", detail = ex.Message });
            }
        }
        //GET: api/OffreEmplois/Recruteurs
               [HttpGet("Recruteurs")]
               [AllowAnonymous]
                public async Task<ActionResult<object>> GetRecruteurIds()
        {
            try
            {
                // Récupérer tous les utilisateurs avec le rôle "Recruteur"
                var recruteurs = await _userManager.GetUsersInRoleAsync("Recruteur");
                // Extraire uniquement les IDs et les noms complets
                var recruteurData = recruteurs.Select(r => new
                {
                    id = r.Id,
                    fullName = r.FullName // Assurez-vous que FullName est défini dans votre modèle AppUser
                }).ToList();
                return Ok(new
                {
                    success = true,
                    message = "Liste des recruteurs récupérée avec succès.",
                    data = recruteurData
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Une erreur est survenue.", detail = ex.Message });
            }
        }

        /*******************************/
        [HttpGet("enums/typeContrat")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<string>> GetTypeContratEnums()
        {
            var values = Enum.GetNames(typeof(TypeContratEnum)).ToList();
            return Ok(values);
        }

        [HttpGet("enums/statut")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<string>> GetStatutEnums()
        {
            var values = Enum.GetNames(typeof(StatutOffre)).ToList();
            return Ok(values);
        }

        [HttpGet("enums/modeTravail")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<string>> GetModeTravailEnums()
        {
            var values = Enum.GetNames(typeof(ModeTravail)).ToList();
            return Ok(values);
        }
        /*********************************************************/
        // GET: api/OffreEmplois/filiale/{idFiliale}
        [HttpGet("filiale/{idFiliale}")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetOffreEmploisByFiliale(Guid idFiliale)
        {
            try
            {
                var offresEmploi = await _context.OffresEmploi
                    .Include(o => o.OffreCompetences)
                        .ThenInclude(oc => oc.Competence)
                    .Where(o => o.IdFiliale == idFiliale)
                    .ToListAsync();

                if (!offresEmploi.Any())
                {
                    return NotFound(new { message = "Aucune offre d'emploi n'est associée à cette filiale." });
                }

                var offresEmploiDTO = offresEmploi.Select(offreEmploi => new OffreEmploiDTO
                {
                    IdOffreEmploi = offreEmploi.IdOffreEmploi,
                    Titre = offreEmploi.Titre,
                    Specialite = offreEmploi.Specialite,
                    Description = offreEmploi.Description,
                    DatePublication = offreEmploi.DatePublication,
                    DateExpiration = offreEmploi.DateExpiration,
                    SalaireMin = offreEmploi.SalaireMin,
                    SalaireMax = offreEmploi.SalaireMax,
                    NiveauExperienceRequis = offreEmploi.NiveauExperienceRequis,
                    DiplomeRequis = offreEmploi.DiplomeRequis,
                    TypeContrat = offreEmploi.TypeContrat,
                    Statut = offreEmploi.Statut,
                    ModeTravail = offreEmploi.ModeTravail,
                    NombrePostes = offreEmploi.NombrePostes,
                    Avantages = offreEmploi.Avantages,
                    IdRecruteur = offreEmploi.IdRecruteur,
                    IdFiliale = offreEmploi.IdFiliale,
                    OffreCompetences = offreEmploi.OffreCompetences.Select(oc => new OffreCompetenceDTO
                    {
                        IdOffreEmploi = oc.IdOffreEmploi,
                        IdCompetence = oc.IdCompetence,
                        NiveauRequis = oc.NiveauRequis,
                        Competence = new CompetenceCreateDto
                        {
                            Id = oc.Competence.Id,
                            Nom = oc.Competence.Nom,
                            Description = oc.Competence.Description,
                            EstTechnique = oc.Competence.estTechnique,
                            EstSoftSkill = oc.Competence.estSoftSkill
                        }
                    }).ToList()
                }).ToList();

                return Ok(new { message = "Offres d'emploi de la filiale récupérées avec succès.", offresEmploi = offresEmploiDTO });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue.", detail = ex.Message });
            }
        }

        [HttpGet("enums/niveauxRequis")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<string>> GetNiveauxRequisEnums()
        {
            var values = new List<string> { "Debutant", "Intermediaire", "Avance", "Expert" };
            return Ok(values);
        }
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> SearchOffreEmplois(
     [FromQuery] string? titre = null,
     [FromQuery] string? specialite = null, // Add specialite parameter
     [FromQuery] StatutOffre? statut = null,
     [FromQuery] Guid? idFiliale = null,
     [FromQuery] ModeTravail? modeTravail = null,
     [FromQuery] TypeContratEnum? typeContrat = null)
        {
            try
            {
                var query = _context.OffresEmploi
                    .Include(o => o.OffreCompetences)
                        .ThenInclude(oc => oc.Competence)
                    .AsQueryable();

                // Appliquer les filtres si spécifiés
                if (!string.IsNullOrWhiteSpace(titre))
                {
                    query = query.Where(o => o.Titre.Contains(titre));
                }

                if (!string.IsNullOrWhiteSpace(specialite))
                {
                    query = query.Where(o => o.Specialite.Contains(specialite));
                }

                if (statut.HasValue)
                {
                    query = query.Where(o => o.Statut == statut.Value);
                }

                if (idFiliale.HasValue)
                {
                    query = query.Where(o => o.IdFiliale == idFiliale.Value);
                }

                if (modeTravail.HasValue)
                {
                    query = query.Where(o => o.ModeTravail == modeTravail.Value);
                }

                if (typeContrat.HasValue)
                {
                    query = query.Where(o => o.TypeContrat == typeContrat.Value);
                }

                var offresEmploi = await query.ToListAsync();

                if (!offresEmploi.Any())
                {
                    return NotFound(new { message = "Aucune offre d'emploi ne correspond aux critères de recherche." });
                }

                var offresEmploiDTO = offresEmploi.Select(offreEmploi => new OffreEmploiDTO
                {
                    IdOffreEmploi = offreEmploi.IdOffreEmploi,
                    Titre = offreEmploi.Titre,
                    Specialite = offreEmploi.Specialite,
                    Description = offreEmploi.Description,
                    DatePublication = offreEmploi.DatePublication,
                    DateExpiration = offreEmploi.DateExpiration,
                    SalaireMin = offreEmploi.SalaireMin,
                    SalaireMax = offreEmploi.SalaireMax,
                    NiveauExperienceRequis = offreEmploi.NiveauExperienceRequis,
                    DiplomeRequis = offreEmploi.DiplomeRequis,
                    TypeContrat = offreEmploi.TypeContrat,
                    Statut = offreEmploi.Statut,
                    ModeTravail = offreEmploi.ModeTravail,
                    NombrePostes = offreEmploi.NombrePostes,
                    Avantages = offreEmploi.Avantages,
                    IdRecruteur = offreEmploi.IdRecruteur,
                    IdFiliale = offreEmploi.IdFiliale,
                    OffreCompetences = offreEmploi.OffreCompetences.Select(oc => new OffreCompetenceDTO
                    {
                        IdOffreEmploi = oc.IdOffreEmploi,
                        IdCompetence = oc.IdCompetence,
                        NiveauRequis = oc.NiveauRequis,
                        Competence = new CompetenceCreateDto
                        {
                            Id = oc.Competence.Id,
                            Nom = oc.Competence.Nom,
                            Description = oc.Competence.Description,
                            EstTechnique = oc.Competence.estTechnique,
                            EstSoftSkill = oc.Competence.estSoftSkill
                        }
                    }).ToList()
                }).ToList();

                return Ok(new { message = "Résultats de recherche récupérés avec succès.", offresEmploi = offresEmploiDTO });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue lors de la recherche.", detail = ex.Message });
            }
        }


    } }

