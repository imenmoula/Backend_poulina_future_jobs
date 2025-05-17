using System;
using System.ComponentModel.DataAnnotations;

namespace Backend_poulina_future_jobs.Models.DTOs
{
    public class CreateFilialeDto
    {
        public Guid IdFiliale { get; set; }

        [Required(ErrorMessage = "Le champ 'Nom' est obligatoire.")]
        [MaxLength(255, ErrorMessage = "Le nom ne peut pas dépasser 255 caractères.")]
        public string Nom { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "L'adresse ne peut pas dépasser 500 caractères.")]
        public string Adresse { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères.")]
        public string Description { get; set; } = string.Empty;

        public DateTime DateCreation { get; set; }

        public string? Photo { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Phone { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? Fax { get; set; } = string.Empty;

        [MaxLength(255), EmailAddress]
        public string? Email { get; set; } = string.Empty;

        [MaxLength(255), Url]
        public string? SiteWeb { get; set; } = string.Empty;
    }
}
