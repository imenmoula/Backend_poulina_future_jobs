
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
using OffreCompetenceDTO = Backend_poulina_future_jobs.Models.DTOs.OffreCompetenceDTO;

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
                    .ToListAsync();

                if (!offresEmploi.Any())
                {
                    return NotFound(new { message = "Aucune offre d'emploi trouvée." });
                }

                var offresEmploiDTO = offresEmploi.Select(o => new OffreEmploiDTO
                {
                    IdOffreEmploi = o.IdOffreEmploi,
                    Titre = o.Titre,
                    Specialite = o.specialite,
                    Description = o.Description,
                    DatePublication = o.DatePublication,
                    DateExpiration = o.DateExpiration,
                    Salaire = o.Salaire,
                    TypeContrat = o.TypeContrat,
                    NombrePostes = o.NombrePostes,
                    ModeTravail = o.ModeTravail,
                    Avantages = o.Avantages,

                    Statut = o.Statut,
                    NiveauExperienceRequis = o.NiveauExperienceRequis,
                    DiplomeRequis = o.DiplomeRequis,
                    IdRecruteur = o.IdRecruteur,
                    IdFiliale = o.IdFiliale,
                    OffreCompetences = o.OffreCompetences.Select(oc => new OffreCompetenceDTO
                    {
                        IdOffreEmploi = oc.IdOffreEmploi,
                        IdCompetence = oc.IdCompetence,
                        NiveauRequis = oc.NiveauRequis
                    }).ToList()
                });

                return Ok(new { message = "Liste des offres d'emploi récupérée avec succès.", offresEmploi = offresEmploiDTO });
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
                // Validation des entités liées
                if (!await _context.Users.AnyAsync(u => u.Id == offreEmploiDTO.IdRecruteur))
                {
                    return BadRequest(new { message = "Le recruteur spécifié n'existe pas." });
                }
                if (!await _context.Filiales.AnyAsync(f => f.IdFiliale == offreEmploiDTO.IdFiliale))
                {
                    return BadRequest(new { message = "La filiale spécifiée n'existe pas." });
                }

                var newOffreId = Guid.NewGuid();
                var offreEmploi = new OffreEmploi
                {
                    IdOffreEmploi = newOffreId,
                    specialite = offreEmploiDTO.Specialite,
                    Titre = offreEmploiDTO.Titre,
                    Description = offreEmploiDTO.Description,
                    DatePublication = DateTime.UtcNow, // Override with server time
                    DateExpiration = offreEmploiDTO.DateExpiration,
                    Salaire = offreEmploiDTO.Salaire,
                    TypeContrat = offreEmploiDTO.TypeContrat,
                    Statut = offreEmploiDTO.Statut,
                    ModeTravail = offreEmploiDTO.ModeTravail,
                    NombrePostes = offreEmploiDTO.NombrePostes,
                    Avantages = offreEmploiDTO.Avantages,
                    NiveauExperienceRequis = offreEmploiDTO.NiveauExperienceRequis,
                    DiplomeRequis = offreEmploiDTO.DiplomeRequis,
                    IdRecruteur = offreEmploiDTO.IdRecruteur,
                    IdFiliale = offreEmploiDTO.IdFiliale,
                    OffreCompetences = new List<OffreCompetences>()
                };

                // Create new competencies and link them
                foreach (var ocDTO in offreEmploiDTO.OffreCompetences)
                {
                    var newCompetenceId = Guid.NewGuid();
                    var competence = new Competence
                    {
                        Id = newCompetenceId,
                        Nom = ocDTO.Competence.Nom,
                        Description = ocDTO.Competence.Description,
                     //DateModification= ocDTO.Competence.DateModification,
                        HardSkills = ocDTO.Competence.HardSkills,
                        SoftSkills = ocDTO.Competence.SoftSkills
                    };

                    _context.Competences.Add(competence);

                    offreEmploi.OffreCompetences.Add(new OffreCompetences
                    {
                        IdOffreEmploi = newOffreId,
                        IdCompetence = newCompetenceId,
                        NiveauRequis = ocDTO.NiveauRequis
                    });
                }

                _context.OffresEmploi.Add(offreEmploi);
                await _context.SaveChangesAsync();

                // Prepare response DTO
                offreEmploiDTO.IdOffreEmploi = newOffreId;
                foreach (var oc in offreEmploiDTO.OffreCompetences)
                {
                    oc.IdOffreEmploi = newOffreId;
                    oc.Competence.Id = offreEmploi.OffreCompetences.First(c => c.NiveauRequis == oc.NiveauRequis).IdCompetence;
                }

                return CreatedAtAction(nameof(GetOffreEmploi), new { id = newOffreId }, new
                {
                    message = "L'offre d'emploi a été créée avec succès.",
                    offreEmploi = offreEmploiDTO
                });
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
            var offreEmploi = await _context.OffresEmploi
                .Include(o => o.OffreCompetences)
                .ThenInclude(oc => oc.Competence) // Ajouter cette ligne pour charger les détails de Competence
                .FirstOrDefaultAsync(o => o.IdOffreEmploi == id);

            if (offreEmploi == null)
            {
                return NotFound(new { message = "L'offre d'emploi demandée n'existe pas." });
            }

            var offreEmploiDTO = new OffreEmploiDTO
            {
                IdOffreEmploi = offreEmploi.IdOffreEmploi,
                Titre = offreEmploi.Titre,
                Specialite = offreEmploi.specialite,
                Description = offreEmploi.Description,
                DatePublication = offreEmploi.DatePublication,
                DateExpiration = offreEmploi.DateExpiration,
                Salaire = offreEmploi.Salaire,
                TypeContrat = offreEmploi.TypeContrat,
                Statut = offreEmploi.Statut,
                ModeTravail = offreEmploi.ModeTravail,
                NombrePostes = offreEmploi.NombrePostes,
                Avantages = offreEmploi.Avantages,
                NiveauExperienceRequis = offreEmploi.NiveauExperienceRequis,
                DiplomeRequis = offreEmploi.DiplomeRequis,
                IdRecruteur = offreEmploi.IdRecruteur,
                IdFiliale = offreEmploi.IdFiliale,
                OffreCompetences = offreEmploi.OffreCompetences.Select(oc => new OffreCompetenceDTO
                {
                    IdOffreEmploi = oc.IdOffreEmploi,
                    IdCompetence = oc.IdCompetence,
                    NiveauRequis = oc.NiveauRequis,
                    Competence = new CompetenceDTO // Remplir les détails de la compétence
                    {
                        Id = oc.Competence.Id,
                        Nom = oc.Competence.Nom,
                        Description = oc.Competence.Description,
                        DateModification = oc.Competence.DateModification,
                        HardSkills = oc.Competence.HardSkills,
                        SoftSkills = oc.Competence.SoftSkills
                    }
                }).ToList()
            };

            return Ok(new { message = "Offre d'emploi récupérée avec succès.", offreEmploi = offreEmploiDTO });
        }

        // PUT: api/OffreEmplois/{id}
        [HttpPut("{id}")]
        [AllowAnonymous]


        public async Task<ActionResult<object>> UpdateOffreEmploi(Guid id, OffreEmploiDTO offreEmploiDTO)
        {
            try
            {
                var offreEmploi = await _context.OffresEmploi
                    .Include(o => o.OffreCompetences)
                    .FirstOrDefaultAsync(o => o.IdOffreEmploi == id);

                if (offreEmploi == null)
                {
                    return NotFound(new { message = "L'offre d'emploi à modifier n'existe pas." });
                }

                // Mettre à jour les champs principaux
                offreEmploi.specialite = offreEmploiDTO.Specialite;
                offreEmploi.Titre = offreEmploiDTO.Titre;
                offreEmploi.Description = offreEmploiDTO.Description;
                offreEmploi.DateExpiration = offreEmploiDTO.DateExpiration;
                offreEmploi.Salaire = offreEmploiDTO.Salaire;
                offreEmploi.TypeContrat = offreEmploiDTO.TypeContrat;
                offreEmploi.Statut = offreEmploiDTO.Statut;
                offreEmploi.NiveauExperienceRequis = offreEmploiDTO.NiveauExperienceRequis;
                offreEmploi.DiplomeRequis = offreEmploiDTO.DiplomeRequis;
                offreEmploi.IdRecruteur = offreEmploiDTO.IdRecruteur;
                offreEmploi.IdFiliale = offreEmploiDTO.IdFiliale;
                // Ajouter les champs manquants
                offreEmploi.ModeTravail = offreEmploiDTO.ModeTravail;
                offreEmploi.Avantages = offreEmploiDTO.Avantages;
                offreEmploi.NombrePostes = offreEmploiDTO.NombrePostes;

                // Supprimer les anciennes compétences
                _context.OffreCompetences.RemoveRange(offreEmploi.OffreCompetences);

                // Ajouter les nouvelles compétences
                foreach (var oc in offreEmploiDTO.OffreCompetences)
                {
                    offreEmploi.OffreCompetences.Add(new OffreCompetences
                    {
                        IdOffreEmploi = id,
                        IdCompetence = oc.IdCompetence,
                        NiveauRequis = oc.NiveauRequis
                    });
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = "L'offre d'emploi a été modifiée avec succès.", offreEmploi = offreEmploiDTO });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue.", detail = ex.Message });
            }
        }        // DELETE: api/OffreEmplois/{id}
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


        //GET: api/OffreEmplois/search
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> Search(
    [FromQuery] string titre = null,
    [FromQuery] string specialite = null,
    [FromQuery] string typeContrat = null,
    [FromQuery] string statut = null,
    [FromQuery] string modeTravail = null) // Ajout de modeTravail
        {
            try
            {
                if (string.IsNullOrWhiteSpace(titre) &&
                    string.IsNullOrWhiteSpace(specialite) &&
                    string.IsNullOrWhiteSpace(typeContrat) &&
                    string.IsNullOrWhiteSpace(statut) &&
                    string.IsNullOrWhiteSpace(modeTravail))
                {
                    return BadRequest(new { message = "Au moins un critère de recherche (titre, spécialité, type de contrat, statut ou mode de travail) doit être fourni." });
                }

                var query = _context.OffresEmploi
                    .Include(o => o.OffreCompetences)
                    .ThenInclude(oc => oc.Competence)
                    .AsQueryable();

                if (!string.IsNullOrWhiteSpace(titre))
                {
                    query = query.Where(o => o.Titre.ToLower().Contains(titre.ToLower()));
                }

                if (!string.IsNullOrWhiteSpace(specialite))
                {
                    query = query.Where(o => o.specialite.ToLower().Contains(specialite.ToLower()));
                }

                if (!string.IsNullOrWhiteSpace(typeContrat))
                {
                    if (!Enum.TryParse<TypeContratEnum>(typeContrat, true, out var typeContratEnum))
                    {
                        return BadRequest(new { message = "TypeContrat invalide." });
                    }
                    query = query.Where(o => o.TypeContrat == (int)typeContratEnum);
                }

                if (!string.IsNullOrWhiteSpace(statut))
                {
                    if (!Enum.TryParse<StatutOffre>(statut, true, out var statutEnum))
                    {
                        return BadRequest(new { message = "Statut invalide." });
                    }
                    query = query.Where(o => o.Statut == (int)statutEnum);
                }

                if (!string.IsNullOrWhiteSpace(modeTravail))
                {
                    if (!Enum.TryParse<ModeTravail>(modeTravail, true, out var modeTravailEnum))
                    {
                        return BadRequest(new { message = $"ModeTravail invalide. Valeurs acceptées : {string.Join(", ", Enum.GetNames(typeof(ModeTravail)))}" });
                    }
                    query = query.Where(o => o.ModeTravail == (int)modeTravailEnum);
                }

                var offresEmploi = await query.ToListAsync();

                if (!offresEmploi.Any())
                {
                    return NotFound(new { message = "Aucune offre trouvée avec ces critères." });
                }

                var offresEmploiDTO = offresEmploi.Select(offreEmploi => new OffreEmploiDTO
                {
                    IdOffreEmploi = offreEmploi.IdOffreEmploi,
                    Specialite = offreEmploi.specialite,
                    Titre = offreEmploi.Titre,
                    Description = offreEmploi.Description,
                    DatePublication = offreEmploi.DatePublication,
                    DateExpiration = offreEmploi.DateExpiration,
                    Salaire = (decimal)offreEmploi.Salaire,
                    TypeContrat = offreEmploi.TypeContrat,
                    Statut = offreEmploi.Statut,
                    ModeTravail = offreEmploi.ModeTravail,
                    NombrePostes=offreEmploi.NombrePostes,
                    NiveauExperienceRequis = offreEmploi.NiveauExperienceRequis,
                    DiplomeRequis = offreEmploi.DiplomeRequis,
                    IdRecruteur = offreEmploi.IdRecruteur,
                    IdFiliale = offreEmploi.IdFiliale,
                    OffreCompetences = offreEmploi.OffreCompetences?.Select(oc => new OffreCompetenceDTO
                    {
                        IdOffreEmploi = oc.IdOffreEmploi,
                        IdCompetence = oc.IdCompetence,
                        NiveauRequis = oc.NiveauRequis,
                        Competence = new CompetenceDTO
                        {
                            Id = oc.Competence.Id,
                            Nom = oc.Competence.Nom,
                            Description = oc.Competence.Description,
                            DateModification = oc.Competence.DateModification,
                            HardSkills = oc.Competence.HardSkills,
                            SoftSkills = oc.Competence.SoftSkills
                        }
                    }).ToList() ?? new List<OffreCompetenceDTO>()
                }).ToList();

                return Ok(new { message = "Offres d'emploi trouvées.", offresEmploi = offresEmploiDTO });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Une erreur est survenue lors de la recherche.", detail = ex.Message });
            }
        }


        private bool OffreEmploiExists(Guid id)
        {
            return _context.OffresEmploi.Any(e => e.IdOffreEmploi == id);
        }
    }
}




