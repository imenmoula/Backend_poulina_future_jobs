using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Backend_poulina_future_jobs.Models.DTOs
{
    public class OffreEmploiDTO
    {
        public Guid IdOffreEmploi { get; set; }

        [Required(ErrorMessage = "Le titre est obligatoire.")]
        [MaxLength(200, ErrorMessage = "Le titre ne peut pas dépasser 200 caractères.")]
        public string Titre { get; set; }

        [Required(ErrorMessage = "La spécialité est obligatoire.")]
        [MaxLength(100, ErrorMessage = "La spécialité ne peut pas dépasser 100 caractères.")]
        public string Specialite { get; set; }

        [Required(ErrorMessage = "La description est obligatoire.")]
        [MaxLength(2000, ErrorMessage = "La description ne peut pas dépasser 2000 caractères.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Le diplôme requis est obligatoire.")]
        [MaxLength(100, ErrorMessage = "Le diplôme requis ne peut pas dépasser 100 caractères.")]
        public string DiplomeRequis { get; set; }

        [Required(ErrorMessage = "L'expérience requise est obligatoire.")]
        [MaxLength(50, ErrorMessage = "L'expérience requise ne peut pas dépasser 50 caractères.")]
        public string NiveauExperienceRequis { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Le salaire doit être positif.")]
        public decimal Salaire { get; set; }

        public DateTime DatePublication { get; set; } = DateTime.UtcNow;

        public DateTime? DateExpiration { get; set; }

        [Required(ErrorMessage = "Le type de contrat est obligatoire.")]
        [Range(1, 4, ErrorMessage = "La valeur de 'TypeContrat' doit être entre 1 et 4.")]
        public int TypeContrat { get; set; }

        [Required(ErrorMessage = "Le statut est obligatoire.")]
        [Range(0, 1, ErrorMessage = "La valeur de 'Statut' doit être 0 (Ouvert) ou 1 (Ferme).")]
        public int Statut { get; set; }

        [Range(0, 2, ErrorMessage = "La valeur de 'ModeTravail' doit être entre 0 et 2.")]
        public int ModeTravail { get; set; } = 0; // Default to Presentiel (0)

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Le nombre de postes doit être au moins 1.")]
        public int NombrePostes { get; set; }

        public string Avantages { get; set; } = string.Empty;

        [Required(ErrorMessage = "L'ID du recruteur est obligatoire.")]
        public Guid IdRecruteur { get; set; }

        [Required(ErrorMessage = "L'ID de la filiale est obligatoire.")]
        public Guid IdFiliale { get; set; }

        public List<OffreCompetenceDTO> OffreCompetences { get; set; } = new List<OffreCompetenceDTO>();
    }
}