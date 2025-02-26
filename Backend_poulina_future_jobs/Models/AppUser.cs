using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_poulina_future_jobs.Models
{
    public class AppUser : IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(150)")]
        public string FullName { get; set; } = string.Empty; // Valeur par défaut ""

        [PersonalData]
        [Column(TypeName = "nvarchar(10)")]
        public string Poste { get; set; } = string.Empty; // Valeur par défaut ""

        [PersonalData]
        public int? filialeId { get; set; } = 0; // Valeur par défaut 0
    }
}
