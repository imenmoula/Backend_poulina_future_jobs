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
        public string Photo { get; set; } = string.Empty;

       

        [PersonalData]
        public DateTime? DateNaissance { get; set; }

        public string Adresse { get; set; } = string.Empty;
        public string Ville { get; set; } = string.Empty;
        public string Pays { get; set; } = string.Empty;
        public string phone { get; set; } = string.Empty;
        public string NiveauEtude { get; set; } = string.Empty;

        public string Diplome { get; set; } = string.Empty;

        public string Universite { get; set; } = string.Empty;
        public string specialite { get; set; } = string.Empty;

        public string cv { get; set; } = string.Empty;
        public string linkedIn { get; set; } = string.Empty;
        
        public string github { get; set; } = string.Empty;
        public string portfolio { get; set; } = string.Empty;
        public string Entreprise { get; set; } = string.Empty;
        public string  Poste { get; set; } = string.Empty;

        // Relation avec les rôles (User peut avoir plusieurs rôles)
        public virtual ICollection<IdentityUserRole<Guid>> UserRoles { get; set; } = new List<IdentityUserRole<Guid>>();

        // Relation 1:N avec OffreEmploi
        public ICollection<OffreEmploi> OffresEmploi { get; set; } = new List<OffreEmploi>();





    }
}
