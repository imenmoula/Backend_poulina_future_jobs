using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Backend_poulina_future_jobs.Models;

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
        public List<DiplomeDto> DiplomesRequis { get; set; }
    }

    public class PosteDto
    {
        [Required(ErrorMessage = "Le titre du poste est obligatoire.")]
        public string TitrePoste { get; set; } = string.Empty;
        [Required(ErrorMessage = "La description du poste est obligatoire.")]
        public string Description { get; set; } = string.Empty;
        [Range(1, int.MaxValue, ErrorMessage = "Le nombre de postes doit être au moins 1.")]
        public int NombrePostes { get; set; }
        public string? ExperienceSouhaitee { get; set; }
        [Required(ErrorMessage = "Le niveau hiérarchique est obligatoire.")]
        public string NiveauHierarchique { get; set; } = string.Empty;
    }

    public class OffreMissionDto
    {
        public Guid IdOffreEmploi { get; set; } // Added to align with OffreMissionsController
        [Required(ErrorMessage = "La description de la mission est obligatoire.")]
        public string DescriptionMission { get; set; } = string.Empty;
        public int Priorite { get; set; }
    }

    public class OffreLangueDto
    {
        public Guid IdOffreEmploi { get; set; } // Added to align with OffreLanguesController
        [Required(ErrorMessage = "La langue est obligatoire.")]
        public Langue Langue { get; set; }
        [Required(ErrorMessage = "Le niveau requis est obligatoire.")]
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
    
        public class DiplomeDto
        {
            public Guid IdDiplome { get; set; }
            [Required(ErrorMessage = "Le nom du diplôme est obligatoire.")]
            public string NomDiplome { get; set; } = string.Empty;
            public string Domaine { get; set; } = string.Empty;
            public string Niveau { get; set; } = string.Empty;
        }
    


    public class CreateOffreEmploiRequest
    {
        [Required(ErrorMessage = "Les données de l'offre sont obligatoires.")]
        public OffreEmploiDto Dto { get; set; }
    }
}