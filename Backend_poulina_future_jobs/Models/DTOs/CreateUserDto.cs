using System.ComponentModel.DataAnnotations;

namespace Backend_poulina_future_jobs.Models.DTOs
{
    public class CreateUserModel
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
        public string Role { get; set; } =string.Empty;
        [Required(ErrorMessage = "Le mot de passe est requis")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Le mot de passe doit contenir entre 6 et 100 caractères")]
        public string Password { get; set; } = string.Empty;

        // Champs optionnels
        public string? Photo { get; set; }
        public DateTime? DateNaissance { get; set; }
        public string? Adresse { get; set; }
        public string? Ville { get; set; }
        public string? Pays { get; set; }
        public string? Phone { get; set; }
        public string? NiveauEtude { get; set; }
        public string? Diplome { get; set; }
        public string? Universite { get; set; }
        public string? Specialite { get; set; }
        public string? Cv { get; set; }
        public string? LinkedIn { get; set; }
        public string? Github { get; set; }
        public string? Portfolio { get; set; }
        public string? Entreprise { get; set; }
        public string? Poste { get; set; }
    }
}