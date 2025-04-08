using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Backend_poulina_future_jobs.Models
{
    // Keep the enums for reference, but they won't be used directly in the model anymore
    public enum StatutOffre
    {
        [EnumMember(Value = "Ouvert")]
        Ouvert = 0,

        [EnumMember(Value = "Ferme")]
        Ferme = 1
    }

    public enum TypeContratEnum
    {
        CDI = 1,
        CDD = 2,
        Freelance = 3,
        Stage = 4,
    }

    public enum ModeTravail
    {
        Presentiel = 0,
        Hybride = 1,
        Teletravail = 2
    }

    public class OffreEmploi
    {
        [Key]
        public Guid IdOffreEmploi { get; set; } = Guid.NewGuid();

        public string specialite { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le champ 'Titre' est obligatoire.")]
        [MaxLength(200, ErrorMessage = "Le titre ne peut pas dépasser 200 caractères.")]
        public string Titre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le champ 'Description' est obligatoire.")]
        [MaxLength(2000, ErrorMessage = "La description ne peut pas dépasser 2000 caractères.")]
        public string Description { get; set; } = string.Empty;

        public DateTime DatePublication { get; set; } = DateTime.UtcNow;

        public DateTime? DateExpiration { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Le salaire doit être positif.")]
        public decimal Salaire { get; set; }

        [MaxLength(50, ErrorMessage = "L'expérience requise ne peut pas dépasser 50 caractères.")]
        public string NiveauExperienceRequis { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "Le diplôme requis ne peut pas dépasser 100 caractères.")]
        public string DiplomeRequis { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le type de contrat est obligatoire.")]
        [Range(1, 4, ErrorMessage = "La valeur de 'TypeContrat' doit être entre 1 et 4.")]
        public int TypeContrat { get; set; }

        [Required(ErrorMessage = "Le statut est obligatoire.")]
        [Range(0, 1, ErrorMessage = "La valeur de 'Statut' doit être 0 (Ouvert) ou 1 (Ferme).")]
        public int Statut { get; set; }

        [Range(0, 2, ErrorMessage = "La valeur de 'ModeTravail' doit être entre 0 et 2.")]
        public int ModeTravail { get; set; } = 0; // Default to Presentiel (0)

        [Range(1, int.MaxValue, ErrorMessage = "Le nombre de postes doit être au moins 1.")]
        public int NombrePostes { get; set; } = 1;

        public string Avantages { get; set; } = string.Empty;

        public Guid IdRecruteur { get; set; }
        [ForeignKey("IdRecruteur")]
        public virtual AppUser Recruteur { get; set; }

        public Guid IdFiliale { get; set; }
        [ForeignKey("IdFiliale")]
        public virtual Filiale Filiale { get; set; }

        public virtual ICollection<OffreCompetences> OffreCompetences { get; set; } = new List<OffreCompetences>();
    }
}


