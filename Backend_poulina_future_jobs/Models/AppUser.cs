using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_poulina_future_jobs.Models
{
    public class AppUser : IdentityUser<Guid> // Utilisation de Guid comme clé
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(150)")]
        public string FullName { get; set; } = string.Empty;

        [PersonalData]
        [Column(TypeName = "nvarchar(150)")]
        public string Nom { get; set; } = string.Empty;

        [PersonalData]
        [Column(TypeName = "nvarchar(150)")]
        public string Prenom { get; set; } = string.Empty;

      
        [PersonalData]
        [Column(TypeName = "nvarchar(255)")]
        public string ?Photo { get; set; } = string.Empty;

       

        [PersonalData]
        public DateTime? DateNaissance { get; set; }

         public string Adresse { get; set; }
        public string Ville { get; set; }
        public string Pays { get; set; }
        public string phone { get; set; }
        public string? Entreprise { get; set; }
        public string? Poste { get; set; }




        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

     

        // Nouvelle relation avec Filiale
        public Guid? IdFiliale { get; set; }
        [ForeignKey("IdFiliale")]
        public Filiale? Filiale { get; set; }

        // Relation avec les rôles (User peut avoir plusieurs rôles)
        public virtual ICollection<IdentityUserRole<Guid>> UserRoles { get; set; } = new List<IdentityUserRole<Guid>>();

        // Relation 1:N avec OffreEmploi
        public ICollection<OffreEmploi> OffresEmploi { get; set; } = new List<OffreEmploi>();

        public ICollection<Experience> Experiences { get; set; } = new List<Experience>(); // Pour un candidat
        public ICollection<Candidature> Candidatures { get; set; } = new List<Candidature>(); // Pour un candidat
        public ICollection<AppUserCompetence> AppUserCompetences { get; set; } = new List<AppUserCompetence>(); // Nouvelle relation
        public List<Quiz> QuizzesTentés { get; set; } = new List<Quiz>();
        // Dans la classe AppUser


        public virtual ICollection<TentativeQuiz> Tentatives { get; set; } = new List<TentativeQuiz>();
        // New relationship with Diplome
        public ICollection<DiplomeCandidate> DiplomesCandidate { get; set; } = new List<DiplomeCandidate>();


        // New relationship with Certificat
        public ICollection<Certificat> Certificats { get; set; } = new List<Certificat>();


    }
}
