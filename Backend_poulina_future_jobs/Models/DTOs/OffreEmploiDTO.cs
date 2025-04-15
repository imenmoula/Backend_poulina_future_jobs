using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Backend_poulina_future_jobs.Models; // Pour accéder aux enums

namespace Backend_poulina_future_jobs.Models.DTOs
{
    public class OffreEmploiDTO
    {
        public Guid IdOffreEmploi { get; set; }

        [Required(ErrorMessage = "Le champ 'Titre' est obligatoire.")]
        public string Titre { get; set; } = string.Empty;

        public string Specialite { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le champ 'Description' est obligatoire.")]
        public string Description { get; set; } = string.Empty;

        public DateTime DatePublication { get; set; }

        public DateTime? DateExpiration { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Le salaire minimum doit être positif.")]
        public decimal SalaireMin { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Le salaire maximum doit être positif.")]
        public decimal SalaireMax { get; set; }

        public string NiveauExperienceRequis { get; set; } = string.Empty;

        public string DiplomeRequis { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le type de contrat est obligatoire.")]
        [EnumDataType(typeof(TypeContratEnum), ErrorMessage = "Type de contrat invalide.")]
        public TypeContratEnum TypeContrat { get; set; } // Enum au lieu de int

        [Required(ErrorMessage = "Le statut est obligatoire.")]
        [EnumDataType(typeof(StatutOffre), ErrorMessage = "Statut invalide.")]
        public StatutOffre Statut { get; set; } // Enum au lieu de int

        [EnumDataType(typeof(ModeTravail), ErrorMessage = "Mode de travail invalide.")]
        public ModeTravail ModeTravail { get; set; } // Enum au lieu de int

        [Range(1, int.MaxValue, ErrorMessage = "Le nombre de postes doit être au moins 1.")]
        public int NombrePostes { get; set; } = 1;

        public string Avantages { get; set; } = string.Empty;

        public Guid IdRecruteur { get; set; }

        public Guid IdFiliale { get; set; }

        public List<OffreCompetenceDTO> OffreCompetences { get; set; } = new List<OffreCompetenceDTO>();
    }
}