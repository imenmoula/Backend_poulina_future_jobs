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
    }
}
