using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
namespace Backend_poulina_future_jobs.Models
{
    

   
        public class Filiale
        {
        [Key]
        public Guid IdFiliale { get; set; }

        [Required(ErrorMessage = "Le champ 'Nom' est obligatoire.")]
        [MaxLength(255, ErrorMessage = "Le nom ne peut pas dépasser 255 caractères.")]
            public string Nom { get; set; } = string.Empty;
        [MaxLength(500, ErrorMessage = "L'adresse ne peut pas dépasser 500 caractères.")]
        public string Adresse { get; set; } = string.Empty;

        [MaxLength(1000, ErrorMessage = "La description ne peut pas dépasser 1000 caractères.")]

        public string Description { get; set; } = string.Empty;


             public DateTime DateCreation { get; set; } = DateTime.UtcNow;

            [AllowNull]
            public string Photo { get; set; } = string.Empty;

    }
}

