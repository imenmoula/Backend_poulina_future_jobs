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
                TitreOffre = offre.TitreOffre,
                DescriptionOffre = offre.DescriptionOffre,
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
                DiplomesRequis = offre.DiplomesRequis.Select(d => new DiplomeDto
                {
                    IdDiplome = d.IdDiplome,
                    NomDiplome = d.NomDiplome,
                    Domaine = d.Domaine,
                    Niveau = d.Niveau
                }).ToList()
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
                TitreOffre = offreEmploi.TitreOffre,
                DescriptionOffre = offreEmploi.DescriptionOffre,
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
                DiplomesRequis = offreEmploi.DiplomesRequis.Select(d => new DiplomeDto
                {
                    IdDiplome = d.IdDiplome,
                    NomDiplome = d.NomDiplome,
                    Domaine = d.Domaine,
                    Niveau = d.Niveau
                }).ToList()
            };

            return Ok(new { success = true, message = "Offre d'emploi récupérée avec succès.", offreEmploi = offreEmploiDto });
        }

        [HttpPost]
        [Authorize(Roles = "Recruteur")] // Ajout du guard d'autorisation
        public async Task<ActionResult<object>> CreateOffreEmploi([FromBody] CreateOffreEmploiRequest request)
        {
            if (request?.Dto == null)
            {
                return BadRequest(new { success = false, message = "Données invalides." });
            }

            var dto = request.Dto;

            // Validation de base
            if (!await _context.Filiales.AnyAsync(f => f.IdFiliale == dto.IdFiliale))
            {
                return BadRequest(new { success = false, message = "La filiale spécifiée n'existe pas." });
            }

            if (!await _context.Departements.AnyAsync(d => d.IdDepartement == dto.IdDepartement))
            {
                return BadRequest(new { success = false, message = "Le département spécifié n'existe pas." });
            }

            if (dto.SalaireMin > dto.SalaireMax)
            {
                return BadRequest(new { success = false, message = "Le salaire minimum ne peut pas être supérieur au salaire maximum." });
            }

            if (dto.DateExpiration <= DateTime.UtcNow)
            {
                return BadRequest(new { success = false, message = "La date d'expiration doit être dans le futur." });
            }

            // Gestion des diplômes - création automatique si nécessaire
            var diplomesToAdd = new List<Diplome>();
            foreach (var diplomeDto in dto.DiplomesRequis)
            {
                var existingDiplome = await _context.Diplomes
                    .FirstOrDefaultAsync(d =>
                        d.NomDiplome == diplomeDto.NomDiplome &&
                        d.Domaine == diplomeDto.Domaine &&
                        d.Niveau == diplomeDto.Niveau);

                if (existingDiplome != null)
                {
                    diplomesToAdd.Add(existingDiplome);
                }
                else
                {
                    var newDiplome = new Diplome
                    {
                        IdDiplome = Guid.NewGuid(),
                        NomDiplome = diplomeDto.NomDiplome,
                        Domaine = diplomeDto.Domaine,
                        Niveau = diplomeDto.Niveau
                    };
                    diplomesToAdd.Add(newDiplome);
                    _context.Diplomes.Add(newDiplome);
                }
            }

            // Création de l'offre
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
                TitreOffre = dto.TitreOffre,
                DescriptionOffre = dto.DescriptionOffre,
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
                OffreCompetences = new List<OffreCompetences>(), // Initialisation vide
                DiplomesRequis = diplomesToAdd
            };

            // Gestion des compétences avec création si nécessaire
            foreach (var competenceDto in dto.OffreCompetences)
            {
                Competence competenceExistante = null;
                if (competenceDto.IdCompetence.HasValue) // Si IdCompetence est fourni
                {
                    competenceExistante = await _context.Competences.FindAsync(competenceDto.IdCompetence);
                }

                if (competenceExistante == null) // Créer une nouvelle compétence si elle n'existe pas
                {
                    competenceExistante = new Competence
                    {
                        Id = Guid.NewGuid(),
                        Nom = competenceDto.Competence?.Nom ?? "Nouvelle compétence",
                        Description = competenceDto.Competence?.Description ?? string.Empty,
                        DateModification = DateTime.UtcNow,
                        estTechnique = competenceDto.Competence?.EstTechnique ?? false,
                        estSoftSkill = competenceDto.Competence?.EstSoftSkill ?? false
                    };
                    _context.Competences.Add(competenceExistante);
                    await _context.SaveChangesAsync(); // Sauvegarde pour générer l'ID
                }

                // Ajouter la relation OffreCompetences
                offreEmploi.OffreCompetences.Add(new OffreCompetences
                {
                    IdOffreEmploi = newId,
                    IdCompetence = competenceExistante.Id,
                    NiveauRequis = competenceDto.NiveauRequis
                });
            }

            _context.OffresEmploi.Add(offreEmploi);
            await _context.SaveChangesAsync();

            // Chargement des relations pour la réponse
            offreEmploi = await _context.OffresEmploi
                .Include(o => o.Postes)
                .Include(o => o.OffreMissions)
                .Include(o => o.OffreLangues)
                .Include(o => o.OffreCompetences).ThenInclude(oc => oc.Competence)
                .Include(o => o.DiplomesRequis)
                .FirstOrDefaultAsync(o => o.IdOffreEmploi == newId);

            var createdOffreEmploiDto = MapToDto(offreEmploi);

            return CreatedAtAction(nameof(GetOffreEmploi), new { id = newId },
                new { success = true, message = "Offre créée avec succès.", offreEmploi = createdOffreEmploiDto });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Recruteur")] // Ajout du guard d'autorisation
        public async Task<ActionResult<object>> UpdateOffreEmploi(Guid id, [FromBody] OffreEmploiDto dto)
        {
            // Validation de base
            if (dto == null || id != dto.IdOffreEmploi)
            {
                return BadRequest(new { success = false, message = "Données invalides ou ID non cohérent." });
            }

            // Démarrer une transaction
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Charger l'offre existante AVEC les relations many-to-many à mettre à jour (Diplomes)
                var offre = await _context.OffresEmploi
                    .Include(o => o.DiplomesRequis)
                    .FirstOrDefaultAsync(o => o.IdOffreEmploi == id);

                if (offre == null)
                {
                    await transaction.RollbackAsync();
                    return NotFound(new { success = false, message = "Offre non trouvée." });
                }

                // --- Mise à jour des propriétés simples ---
                offre.Specialite = dto.Specialite;
                offre.DateExpiration = dto.DateExpiration;
                offre.SalaireMin = dto.SalaireMin;
                offre.SalaireMax = dto.SalaireMax;
                offre.NiveauExperienceRequis = dto.NiveauExperienceRequis;
                offre.TypeContrat = dto.TypeContrat;
                offre.Statut = dto.Statut;
                offre.ModeTravail = dto.ModeTravail;
                offre.estActif = dto.EstActif;
                offre.Avantages = dto.Avantages;
                offre.IdRecruteur = dto.IdRecruteur;
                offre.IdFiliale = dto.IdFiliale;
                offre.IdDepartement = dto.IdDepartement;
                offre.TitreOffre = dto.TitreOffre;
                offre.DescriptionOffre = dto.DescriptionOffre;

                // --- Gestion des collections One-to-Many (Suppression / Ajout) ---
                await _context.Postes.Where(p => p.IdOffreEmploi == id).ExecuteDeleteAsync();
                await _context.OffreMissions.Where(m => m.IdOffreEmploi == id).ExecuteDeleteAsync();
                await _context.OffreLangues.Where(l => l.IdOffreEmploi == id).ExecuteDeleteAsync();
                await _context.OffreCompetences.Where(c => c.IdOffreEmploi == id).ExecuteDeleteAsync();

                // Ajouter les nouveaux Postes
                foreach (var posteDto in dto.Postes)
                {
                    _context.Postes.Add(new Poste
                    {
                        IdOffreEmploi = id,
                        TitrePoste = posteDto.TitrePoste,
                        Description = posteDto.Description,
                        NombrePostes = posteDto.NombrePostes,
                        ExperienceSouhaitee = posteDto.ExperienceSouhaitee,
                        NiveauHierarchique = posteDto.NiveauHierarchique
                    });
                }

                // Ajouter les nouvelles Missions
                foreach (var missionDto in dto.OffreMissions)
                {
                    _context.OffreMissions.Add(new OffreMission
                    {
                        IdOffreEmploi = id,
                        DescriptionMission = missionDto.DescriptionMission,
                        Priorite = missionDto.Priorite
                    });
                }

                // Ajouter les nouvelles Langues
                foreach (var langueDto in dto.OffreLangues)
                {
                    _context.OffreLangues.Add(new OffreLangue
                    {
                        IdOffreEmploi = id,
                        Langue = langueDto.Langue,
                        NiveauRequis = langueDto.NiveauRequis
                    });
                }

                // Ajouter les nouvelles Compétences
                foreach (var competenceDto in dto.OffreCompetences)
                {
                    Competence competenceExistante = null;
                    if (competenceDto.IdCompetence.HasValue)
                    {
                        competenceExistante = await _context.Competences.FindAsync(competenceDto.IdCompetence);
                    }

                    if (competenceExistante == null)
                    {
                        competenceExistante = new Competence
                        {
                            Id = Guid.NewGuid(),
                            Nom = competenceDto.Competence?.Nom ?? "Nouvelle compétence",
                            Description = competenceDto.Competence?.Description ?? string.Empty,
                            DateModification = DateTime.UtcNow,
                            estTechnique = competenceDto.Competence?.EstTechnique ?? false,
                            estSoftSkill = competenceDto.Competence?.EstSoftSkill ?? false
                        };
                        _context.Competences.Add(competenceExistante);
                        await _context.SaveChangesAsync(); // Sauvegarde pour générer l'ID
                    }

                    _context.OffreCompetences.Add(new OffreCompetences
                    {
                        IdOffreEmploi = id,
                        IdCompetence = competenceExistante.Id,
                        NiveauRequis = competenceDto.NiveauRequis
                    });
                }

                // --- Gestion de la collection Many-to-Many (DiplomesRequis) ---
                offre.DiplomesRequis.Clear();
                var nouveauxDiplomeIds = dto.DiplomesRequis.Select(d => d.IdDiplome).Distinct().ToList();
                var diplomesAAttacher = await _context.Diplomes
                    .Where(d => nouveauxDiplomeIds.Contains(d.IdDiplome))
                    .ToListAsync();

                if (diplomesAAttacher.Count != nouveauxDiplomeIds.Count)
                {
                    var idsNonTrouves = nouveauxDiplomeIds.Except(diplomesAAttacher.Select(d => d.IdDiplome));
                    await transaction.RollbackAsync();
                    return BadRequest(new { success = false, message = $"Les diplômes suivants n'ont pas été trouvés : {string.Join(", ", idsNonTrouves)}" });
                }

                foreach (var diplome in diplomesAAttacher)
                {
                    offre.DiplomesRequis.Add(diplome);
                }

                // --- Sauvegarder les changements et commiter la transaction ---
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // --- Préparer la réponse ---
                var updatedOffre = await _context.OffresEmploi
                    .Include(o => o.Postes)
                    .Include(o => o.OffreMissions)
                    .Include(o => o.OffreLangues)
                    .Include(o => o.OffreCompetences).ThenInclude(oc => oc.Competence)
                    .Include(o => o.DiplomesRequis)
                    .FirstOrDefaultAsync(o => o.IdOffreEmploi == id);

                return Ok(new
                {
                    success = true,
                    message = "Offre mise à jour avec succès.",
                    data = MapToDto(updatedOffre)
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Erreur lors de la mise à jour de l'offre {id}: {ex.ToString()}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Une erreur interne est survenue lors de la mise à jour de l'offre."
                });
            }
        }

        private OffreEmploiDto MapToDto(OffreEmploi offre)
        {
            return new OffreEmploiDto
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
                TitreOffre = offre.TitreOffre,
                DescriptionOffre = offre.DescriptionOffre,
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
                DiplomesRequis = offre.DiplomesRequis.Select(d => new DiplomeDto
                {
                    IdDiplome = d.IdDiplome,
                    NomDiplome = d.NomDiplome,
                    Domaine = d.Domaine,
                    Niveau = d.Niveau
                }).ToList()
            };
        }
        private void UpdateRelatedEntities(OffreEmploi offreEmploi, OffreEmploiDto dto, Guid id)
        {
            // Mise à jour des Postes
            offreEmploi.Postes.Clear();
            foreach (var posteDto in dto.Postes)
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

            // Mise à jour des Missions
            offreEmploi.OffreMissions.Clear();
            foreach (var missionDto in dto.OffreMissions)
            {
                offreEmploi.OffreMissions.Add(new OffreMission
                {
                    IdOffreEmploi = id,
                    DescriptionMission = missionDto.DescriptionMission,
                    Priorite = missionDto.Priorite
                });
            }

            // Mise à jour des Langues
            offreEmploi.OffreLangues.Clear();
            foreach (var langueDto in dto.OffreLangues)
            {
                offreEmploi.OffreLangues.Add(new OffreLangue
                {
                    IdOffreEmploi = id,
                    Langue = langueDto.Langue,
                    NiveauRequis = langueDto.NiveauRequis
                });
            }

            // Mise à jour des Compétences
            offreEmploi.OffreCompetences.Clear();
            foreach (var competenceDto in dto.OffreCompetences)
            {
                offreEmploi.OffreCompetences.Add(new OffreCompetences
                {
                    IdOffreEmploi = id,
                    IdCompetence = (Guid)competenceDto.IdCompetence,
                    NiveauRequis = competenceDto.NiveauRequis
                });
            }
        }

       
        [HttpDelete("{id}")]

        [Authorize(Roles = "Recruteur")] // Ajout du guard d'autorisation
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
                    DiplomesRequis = offre.DiplomesRequis.Select(d => new DiplomeDto
                    {
                        IdDiplome = d.IdDiplome,
                        NomDiplome = d.NomDiplome,
                        Domaine = d.Domaine,
                        Niveau = d.Niveau
                    }).ToList()
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
        //[HttpGet("by-recruteur-filiale/{idFiliale}")]
        //[AllowAnonymous]
        //public async Task<ActionResult<object>> GetOffresByRecruteurOfFiliale(Guid idFiliale)
        //{
        //    try
        //    {
        //        // Vérifier si la filiale existe
        //        var filialeExists = await _context.Filiales.AnyAsync(f => f.IdFiliale == idFiliale);
        //        if (!filialeExists)
        //        {
        //            return BadRequest(new { success = false, message = "La filiale spécifiée n'existe pas." });
        //        }

        //        // Récupérer tous les recruteurs appartenant à cette filiale
        //        var recruteursIds = await _userManager.GetUsersInRoleAsync("Recruteur")
        //            .ContinueWith(task => task.Result
        //                .Where(u => u.IdFiliale == idFiliale) // Supposons que AppUser a une propriété IdFiliale
        //                .Select(u => u.Id)
        //                .ToList());

        //        if (!recruteursIds.Any())
        //        {
        //            return NotFound(new { success = false, message = "Aucun recruteur trouvé pour cette filiale." });
        //        }

        //        // Récupérer les offres d'emploi créées par ces recruteurs
        //        var offresEmploi = await _context.OffresEmploi
        //            .Include(o => o.Postes)
        //            .Include(o => o.OffreMissions)
        //            .Include(o => o.OffreLangues)
        //            .Include(o => o.OffreCompetences).ThenInclude(oc => oc.Competence)
        //            .Include(o => o.DiplomesRequis)
        //            .Include(o => o.Filiale)
        //            .Include(o => o.Departement)
        //            .Where(o => recruteursIds.Contains(o.IdRecruteur))
        //            .ToListAsync();

        //        if (!offresEmploi.Any())
        //        {
        //            return NotFound(new { success = false, message = "Aucune offre d'emploi trouvée pour les recruteurs de cette filiale." });
        //        }

        //        // Mapper vers OffreEmploiDto
        //        var offresEmploiDto = offresEmploi.Select(offre => new OffreEmploiDto
        //        {
        //            IdOffreEmploi = offre.IdOffreEmploi,
        //            Specialite = offre.Specialite,
        //            DatePublication = offre.DatePublication,
        //            DateExpiration = offre.DateExpiration,
        //            SalaireMin = offre.SalaireMin,
        //            SalaireMax = offre.SalaireMax,
        //            NiveauExperienceRequis = offre.NiveauExperienceRequis,
        //            TypeContrat = offre.TypeContrat,
        //            Statut = offre.Statut,
        //            ModeTravail = offre.ModeTravail,
        //            EstActif = offre.estActif,
        //            Avantages = offre.Avantages,
        //            IdRecruteur = offre.IdRecruteur,
        //            IdFiliale = offre.IdFiliale,
        //            IdDepartement = offre.IdDepartement,
        //            Postes = offre.Postes.Select(p => new PosteDto
        //            {
        //                TitrePoste = p.TitrePoste,
        //                Description = p.Description,
        //                NombrePostes = p.NombrePostes,
        //                ExperienceSouhaitee = p.ExperienceSouhaitee,
        //                NiveauHierarchique = p.NiveauHierarchique
        //            }).ToList(),
        //            OffreMissions = offre.OffreMissions.Select(m => new OffreMissionDto
        //            {
        //                DescriptionMission = m.DescriptionMission,
        //                Priorite = m.Priorite
        //            }).ToList(),
        //            OffreLangues = offre.OffreLangues.Select(l => new OffreLangueDto
        //            {
        //                Langue = l.Langue,
        //                NiveauRequis = l.NiveauRequis
        //            }).ToList(),
        //            OffreCompetences = offre.OffreCompetences.Select(oc => new OffreCompetenceDto
        //            {
        //                IdOffreEmploi = oc.IdOffreEmploi,
        //                IdCompetence = oc.IdCompetence,
        //                NiveauRequis = oc.NiveauRequis,
        //                Competence = new CompetenceDto
        //                {
        //                    Id = oc.Competence.Id,
        //                    Nom = oc.Competence.Nom,
        //                    Description = oc.Competence.Description,
        //                    DateModification = oc.Competence.DateModification,
        //                    EstTechnique = oc.Competence.estTechnique,
        //                    EstSoftSkill = oc.Competence.estSoftSkill
        //                }
        //            }).ToList(),
        //            DiplomesRequis = offre.DiplomesRequis.Select(d => new DiplomeDto
        //            {
        //                IdDiplome = d.IdDiplome,
        //                NomDiplome = d.NomDiplome,
        //                Domaine = d.Domaine,
        //                Niveau = d.Niveau
        //            }).ToList()
        //        }).ToList();

        //        return Ok(new
        //        {
        //            success = true,
        //            message = "Offres d'emploi récupérées avec succès pour les recruteurs de la filiale.",
        //            offresEmploi = offresEmploiDto
        //        });

        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new
        //        {
        //            success = false,
        //            message = "Une erreur est survenue lors de la récupération des offres.",
        //            detail = ex.Message
        //        });
        //    }
        //}



       


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

        [HttpGet("by-connected-recruteur")]
        [Authorize(Roles = "Recruteur")] // Ajout du guard d'autorisation
        public async Task<ActionResult<object>> GetOffresByConnectedRecruteur()
        {
            try
            {
                // 1. Récupérer l'identité de l'utilisateur
                var identity = HttpContext.User.Identity as ClaimsIdentity;
                if (identity == null)
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Identité utilisateur non trouvée."
                    });
                }

                // 2. Récupérer le claim userId (version robuste)
                var userIdClaim = identity.FindFirst(c =>
                    c.Type == ClaimTypes.NameIdentifier ||
                    c.Type == "userId" ||
                    c.Type == "sub");

                if (userIdClaim == null || string.IsNullOrWhiteSpace(userIdClaim.Value))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Claim userId manquant dans le token."
                    });
                }

                // 3. Vérifier et parser le userId
                if (!Guid.TryParse(userIdClaim.Value, out Guid recruteurGuid))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Format d'ID utilisateur invalide."
                    });
                }

                // 4. Récupérer l'utilisateur depuis la base de données
                var recruiter = await _userManager.FindByIdAsync(recruteurGuid.ToString());
                if (recruiter == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Utilisateur non trouvé."
                    });
                }

                // 5. Vérifier le rôle
                var isRecruteur = await _userManager.IsInRoleAsync(recruiter, "Recruteur");
                if (!isRecruteur)
                {
                    return StatusCode(403, new
                    {
                        success = false,
                        message = "Accès refusé : vous n'êtes pas recruteur."
                    });
                }

                // 6. Récupérer les offres avec toutes les relations
                var offres = await _context.OffresEmploi
                    .Where(o => o.IdRecruteur == recruteurGuid)
                    .Include(o => o.Postes)
                    .Include(o => o.OffreMissions)
                    .Include(o => o.OffreLangues)
                    .Include(o => o.OffreCompetences)
                        .ThenInclude(oc => oc.Competence)
                    .Include(o => o.DiplomesRequis)
                    .Include(o => o.Filiale)
                    .Include(o => o.Departement)
                    .OrderByDescending(o => o.DatePublication)
                    .AsNoTracking()
                    .ToListAsync();

                // 7. Gérer le cas où aucune offre n'est trouvée
                if (!offres.Any())
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Aucune offre trouvée pour ce recruteur.",
                        count = 0,
                        offresEmploi = new List<object>()
                    });
                }

                // 8. Mapper les offres vers le DTO
                var offresDto = offres.Select(offre => new OffreEmploiDto
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
                        Competence = oc.Competence != null ? new CompetenceDto
                        {
                            Id = oc.Competence.Id,
                            Nom = oc.Competence.Nom,
                            Description = oc.Competence.Description,
                            DateModification = oc.Competence.DateModification,
                            EstTechnique = oc.Competence.estTechnique,
                            EstSoftSkill = oc.Competence.estSoftSkill
                        } : null
                    }).ToList(),
                    DiplomesRequis = offre.DiplomesRequis.Select(d => new DiplomeDto
                    {
                        IdDiplome = d.IdDiplome,
                        NomDiplome = d.NomDiplome,
                        Domaine = d.Domaine,
                        Niveau = d.Niveau
                    }).ToList()
                }).ToList();

                // 9. Retourner la réponse
                return Ok(new
                {
                    success = true,
                    message = "Offres récupérées avec succès.",
                    recruteur = new
                    {
                        id = recruiter.Id,
                        email = recruiter.Email,
                        fullName = recruiter.FullName
                    },
                    count = offresDto.Count,
                    offresEmploi = offresDto
                });
            }
            catch (Exception ex)
            {
                // Log l'erreur pour le débogage
                //_logger.LogError(ex, "Erreur lors de la récupération des offres du recruteur");

                return StatusCode(500, new
                {
                    success = false,
                    message = "Erreur serveur lors de la récupération des offres.",
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

