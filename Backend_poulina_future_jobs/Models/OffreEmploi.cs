
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
        [EnumMember(Value = " cloturer")]
        cloturer = 1
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

     

     
        public DateTime DatePublication { get; set; } = DateTime.UtcNow;

        public DateTime? DateExpiration { get; set; }


        [Range(0, double.MaxValue, ErrorMessage = "Le salaire minimum doit être positif.")]
        public decimal SalaireMin { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Le salaire maximum doit être positif.")]
        public decimal SalaireMax { get; set; }


        // Niveau d'expérience requis, par exemple "Débutant", "Intermédiaire", "Senior"
        [Required(ErrorMessage = "Le niveau d'expérience requis est obligatoire.")]
        public string NiveauExperienceRequis { get; set; } = string.Empty;



        [Required(ErrorMessage = "Le type de contrat est obligatoire.")]
        public TypeContratEnum TypeContrat { get; set; }

        [Required(ErrorMessage = "Le statut est obligatoire.")]
        public StatutOffre Statut { get; set; }

        public ModeTravail ModeTravail { get; set; } = ModeTravail.Presentiel;

       public bool estActif { get; set; } = true;

        public string Avantages { get; set; } = string.Empty;

        public Guid IdRecruteur { get; set; }
        [ForeignKey("IdRecruteur")]
        public virtual AppUser Recruteur { get; set; }

        // Relation avec Filiale (décommentée et active)
        public Guid IdFiliale { get; set; }
        [ForeignKey("IdFiliale")]
        public virtual Filiale Filiale { get; set; }


        // Relation avec Departement correctement configurée
        // ✅ Colonne existante dans la base
        public Guid IdDepartement { get; set; }

        // ✅ Relation configurée avec la bonne clé étrangère
        [ForeignKey("IdDepartement")] // Correspond au nom de la colonne
        public virtual Departement Departement { get; set; }




        // Relations avec les nouvelles classes

        public virtual ICollection<Diplome> DiplomesRequis { get; set; } = new List<Diplome>();
        public virtual ICollection<Poste> Postes { get; set; } = new List<Poste>();
        public virtual ICollection<OffreMission> OffreMissions { get; set; } = new List<OffreMission>();
        public virtual ICollection<OffreLangue> OffreLangues { get; set; } = new List<OffreLangue>();
        public virtual ICollection<OffreCompetences> OffreCompetences { get; set; } = new List<OffreCompetences>();
        /*quize*/
        public ICollection<Candidature> Candidatures { get; set; } = new List<Candidature>();
        // Relations
        public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
    }
}
