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

namespace Backend_poulina_future_jobs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OffreEmploisController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OffreEmploisController(AppDbContext context)
        {
            _context = context;
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

        // POST: api/OffreEmplois
        // PUT: api/OffreEmplois/{id}
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> UpdateOffreEmploi(Guid id, [FromBody] OffreEmploiDTO offreEmploiDTO)
        {
            try
            {
                if (id != offreEmploiDTO.IdOffreEmploi || !ModelState.IsValid)
                {
                    return BadRequest(new { message = "Données ou Guid invalides.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
                }

                var offreEmploi = await _context.OffresEmploi
                    .Include(o => o.OffreCompetences)
                    .FirstOrDefaultAsync(o => o.IdOffreEmploi == id);

                if (offreEmploi == null)
                {
                    return NotFound(new { message = "L'offre d'emploi à modifier n'existe pas." });
                }

                // Valider les entités liées
                if (!await _context.Users.AnyAsync(u => u.Id == offreEmploiDTO.IdRecruteur))
                {
                    return BadRequest(new { message = "Le recruteur spécifié n'existe pas." });
                }
                if (!await _context.Filiales.AnyAsync(f => f.IdFiliale == offreEmploiDTO.IdFiliale))
                {
                    return BadRequest(new { message = "La filiale spécifiée n'existe pas." });
                }

                // Valider les compétences
                var competenceIds = offreEmploiDTO.OffreCompetences.Select(oc => oc.IdCompetence).ToList();
                var competencesExist = await _context.Competences
                    .Where(c => competenceIds.Contains(c.Id))
                    .CountAsync() == competenceIds.Count;

                if (!competencesExist)
                {
                    return BadRequest(new { message = "Une ou plusieurs compétences spécifiées n'existent pas." });
                }

                // Mise à jour des champs
                offreEmploi.Titre = offreEmploiDTO.Titre;
                offreEmploi.Specialite = offreEmploiDTO.Specialite;
                offreEmploi.Description = offreEmploiDTO.Description;
                offreEmploi.DatePublication = offreEmploiDTO.DatePublication;
                offreEmploi.DateExpiration = offreEmploiDTO.DateExpiration;
                offreEmploi.SalaireMin = offreEmploiDTO.SalaireMin;
                offreEmploi.SalaireMax = offreEmploiDTO.SalaireMax;
                offreEmploi.NiveauExperienceRequis = offreEmploiDTO.NiveauExperienceRequis;
                offreEmploi.DiplomeRequis = offreEmploiDTO.DiplomeRequis;
                offreEmploi.TypeContrat = offreEmploiDTO.TypeContrat;
                offreEmploi.Statut = offreEmploiDTO.Statut;
                offreEmploi.ModeTravail = offreEmploiDTO.ModeTravail;
                offreEmploi.NombrePostes = offreEmploiDTO.NombrePostes;
                offreEmploi.Avantages = offreEmploiDTO.Avantages;
                offreEmploi.IdRecruteur = offreEmploiDTO.IdRecruteur;
                offreEmploi.IdFiliale = offreEmploiDTO.IdFiliale;

                // Ajouter champ DateModification

                // Supprimer les compétences existantes
                _context.OffreCompetences.RemoveRange(offreEmploi.OffreCompetences);

                // Ajouter les nouvelles compétences avec ID auto-généré et date de modification
                offreEmploi.OffreCompetences = new List<OffreCompetences>();

                foreach (var oc in offreEmploiDTO.OffreCompetences)
                {
                    var offreCompetence = new OffreCompetences
                    {
                        IdOffreEmploi = id,
                        IdCompetence = oc.IdCompetence,
                        NiveauRequis = oc.NiveauRequis,
                     
                    };

                    offreEmploi.OffreCompetences.Add(offreCompetence);
                }

                await _context.SaveChangesAsync();

                // Charger les détails pour la réponse
                var updatedOffreEmploi = await _context.OffresEmploi
                    .Include(o => o.OffreCompetences)
                        .ThenInclude(oc => oc.Competence)
                    .FirstOrDefaultAsync(o => o.IdOffreEmploi == id);

                var updatedOffreEmploiDTO = new OffreEmploiDTO
                {
                    IdOffreEmploi = updatedOffreEmploi.IdOffreEmploi,
                    Titre = updatedOffreEmploi.Titre,
                    Specialite = updatedOffreEmploi.Specialite,
                    Description = updatedOffreEmploi.Description,
                    DatePublication = updatedOffreEmploi.DatePublication,
                    DateExpiration = updatedOffreEmploi.DateExpiration,
                    SalaireMin = updatedOffreEmploi.SalaireMin,
                    SalaireMax = updatedOffreEmploi.SalaireMax,
                    NiveauExperienceRequis = updatedOffreEmploi.NiveauExperienceRequis,
                    DiplomeRequis = updatedOffreEmploi.DiplomeRequis,
                    TypeContrat = updatedOffreEmploi.TypeContrat,
                    Statut = updatedOffreEmploi.Statut,
                    ModeTravail = updatedOffreEmploi.ModeTravail,
                    NombrePostes = updatedOffreEmploi.NombrePostes,
                    Avantages = updatedOffreEmploi.Avantages,
                    IdRecruteur = updatedOffreEmploi.IdRecruteur,
                    IdFiliale = updatedOffreEmploi.IdFiliale,
                    OffreCompetences = updatedOffreEmploi.OffreCompetences.Select(oc => new OffreCompetenceDTO
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

                return Ok(new { message = "L'offre d'emploi a été modifiée avec succès.", offreEmploi = updatedOffreEmploiDTO });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue.", detail = ex.Message });
            }
        }
        

        // POST: api/OffreEmplois
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<object>> CreateOffreEmploi([FromBody] OffreEmploiDTO offreEmploiDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new { message = "Données invalides.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
                }

                // Valider les entités liées
                if (!await _context.Users.AnyAsync(u => u.Id == offreEmploiDTO.IdRecruteur))
                {
                    return BadRequest(new { message = "Le recruteur spécifié n'existe pas." });
                }
                if (!await _context.Filiales.AnyAsync(f => f.IdFiliale == offreEmploiDTO.IdFiliale))
                {
                    return BadRequest(new { message = "La filiale spécifiée n'existe pas." });
                }

                // Valider les compétences
                var competenceIds = offreEmploiDTO.OffreCompetences.Select(oc => oc.IdCompetence).ToList();
                var competencesExist = await _context.Competences
                    .Where(c => competenceIds.Contains(c.Id))
                    .CountAsync() == competenceIds.Count;

                if (!competencesExist)
                {
                    return BadRequest(new { message = "Une ou plusieurs compétences spécifiées n'existent pas." });
                }

                // Création avec ID automatique et DatePublication = maintenant
                var offreEmploi = new OffreEmploi
                {
                    IdOffreEmploi = Guid.NewGuid(),
                    Titre = offreEmploiDTO.Titre,
                    Specialite = offreEmploiDTO.Specialite,
                    Description = offreEmploiDTO.Description,
                    DatePublication = DateTime.UtcNow,  // Date automatique à la création
                    DateExpiration = offreEmploiDTO.DateExpiration,
                    SalaireMin = offreEmploiDTO.SalaireMin,
                    SalaireMax = offreEmploiDTO.SalaireMax,
                    NiveauExperienceRequis = offreEmploiDTO.NiveauExperienceRequis,
                    DiplomeRequis = offreEmploiDTO.DiplomeRequis,
                    TypeContrat = offreEmploiDTO.TypeContrat,
                    Statut = offreEmploiDTO.Statut,
                    ModeTravail = offreEmploiDTO.ModeTravail,
                    NombrePostes = offreEmploiDTO.NombrePostes,
                    Avantages = offreEmploiDTO.Avantages,
                    IdRecruteur = offreEmploiDTO.IdRecruteur,
                    IdFiliale = offreEmploiDTO.IdFiliale,
                    OffreCompetences = new List<OffreCompetences>()
                };

                // Ajouter les compétences avec des IDs automatiques
                foreach (var oc in offreEmploiDTO.OffreCompetences)
                {
                    var offreCompetence = new OffreCompetences
                    {
                        // ID généré automatiquement pour la relation OffreCompetences
                        IdOffreEmploi = offreEmploi.IdOffreEmploi,
                        IdCompetence = oc.IdCompetence,
                        NiveauRequis = oc.NiveauRequis,
                    };

                    offreEmploi.OffreCompetences.Add(offreCompetence);
                }

                _context.OffresEmploi.Add(offreEmploi);
                await _context.SaveChangesAsync();

                // Charger les détails complets pour la réponse
                var createdOffreEmploi = await _context.OffresEmploi
                    .Include(o => o.OffreCompetences)
                        .ThenInclude(oc => oc.Competence)
                    .FirstOrDefaultAsync(o => o.IdOffreEmploi == offreEmploi.IdOffreEmploi);

                var responseDTO = new OffreEmploiDTO
                {
                    IdOffreEmploi = createdOffreEmploi.IdOffreEmploi,
                    Titre = createdOffreEmploi.Titre,
                    Specialite = createdOffreEmploi.Specialite,
                    Description = createdOffreEmploi.Description,
                    DatePublication = createdOffreEmploi.DatePublication,
                    DateExpiration = createdOffreEmploi.DateExpiration,
                    SalaireMin = createdOffreEmploi.SalaireMin,
                    SalaireMax = createdOffreEmploi.SalaireMax,
                    NiveauExperienceRequis = createdOffreEmploi.NiveauExperienceRequis,
                    DiplomeRequis = createdOffreEmploi.DiplomeRequis,
                    TypeContrat = createdOffreEmploi.TypeContrat,
                    Statut = createdOffreEmploi.Statut,
                    ModeTravail = createdOffreEmploi.ModeTravail,
                    NombrePostes = createdOffreEmploi.NombrePostes,
                    Avantages = createdOffreEmploi.Avantages,
                    IdRecruteur = createdOffreEmploi.IdRecruteur,
                    IdFiliale = createdOffreEmploi.IdFiliale,
                    OffreCompetences = createdOffreEmploi.OffreCompetences.Select(oc => new OffreCompetenceDTO
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

                return CreatedAtAction(nameof(GetOffreEmploi), new { id = offreEmploi.IdOffreEmploi }, new
                {
                    message = "L'offre d'emploi a été créée avec succès.",
                    offreEmploi = responseDTO
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue.", detail = ex.Message });
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

        /*******************************/
        // GET: api/OffreEmplois/statut/{statut}
        [HttpGet("statut/{statut}")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetOffreEmploisByStatut(StatutOffre statut)
        {
            try
            {
                var offresEmploi = await _context.OffresEmploi
                    .Include(o => o.OffreCompetences)
                        .ThenInclude(oc => oc.Competence)
                    .Where(o => o.Statut == statut)
                    .ToListAsync();

                if (!offresEmploi.Any())
                {
                    return NotFound(new { message = $"Aucune offre d'emploi avec le statut '{statut}' n'a été trouvée." });
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

                return Ok(new { message = $"Offres d'emploi avec statut '{statut}' récupérées avec succès.", offresEmploi = offresEmploiDTO });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue.", detail = ex.Message });
            }
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
        // GET: api/OffreEmplois/search
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> SearchOffreEmplois(
            [FromQuery] string? titre = null,
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

