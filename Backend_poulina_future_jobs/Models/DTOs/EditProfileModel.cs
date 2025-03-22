using System.ComponentModel.DataAnnotations;

namespace Backend_poulina_future_jobs.Models.DTOs
{
    public class EditProfileModel
    {
        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Email invalide")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom complet est requis")]
        [StringLength(150, ErrorMessage = "Le nom complet doit contenir au maximum 150 caractères")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom est requis")]
        [StringLength(150, ErrorMessage = "Le nom doit contenir au maximum 150 caractères")]
        public string Nom { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le prénom est requis")]
        [StringLength(150, ErrorMessage = "Le prénom doit contenir au maximum 150 caractères")]
        public string Prenom { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Le chemin de la photo doit contenir au maximum 255 caractères")]
        public string Photo { get; set; } = string.Empty;
    }
}
