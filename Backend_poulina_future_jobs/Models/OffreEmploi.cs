
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Backend_poulina_future_jobs.Models
{
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
            
       Alternance= 5,
            

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

        public string Specialite { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le champ 'Titre' est obligatoire.")]
        public string Titre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le champ 'Description' est obligatoire.")]
        public string Description { get; set; } = string.Empty;

        public DateTime DatePublication { get; set; } = DateTime.UtcNow;

        public DateTime? DateExpiration { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Le salaire minimum doit être positif.")]
        public decimal SalaireMin { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Le salaire maximum doit être positif.")]
        public decimal SalaireMax { get; set; }

        public string NiveauExperienceRequis { get; set; } = string.Empty;

        public string DiplomeRequis { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le type de contrat est obligatoire.")]
        public TypeContratEnum TypeContrat { get; set; }

        [Required(ErrorMessage = "Le statut est obligatoire.")]
        public StatutOffre Statut { get; set; }

        public ModeTravail ModeTravail { get; set; } = ModeTravail.Presentiel;

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
