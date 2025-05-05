using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Backend_poulina_future_jobs.Models
{
    public class Poste 
    {
        [Key]
        public Guid IdPoste { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Le titre du poste est obligatoire.")]
        public string TitrePoste { get; set; } = string.Empty;

        [Required(ErrorMessage = "La description du poste est obligatoire.")]
        public string Description { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Le nombre de postes doit être au moins 1.")]
        public int NombrePostes { get; set; } = 1;


        // Expérience souhaitée (optionnelle)
        public string? ExperienceSouhaitee { get; set; }

        // Exemple : "Junior", "Senior", "Chef d'équipe"
        [Required(ErrorMessage = "Le niveau hiérarchique est obligatoire.")]
        public string NiveauHierarchique { get; set; } = string.Empty;
        public Guid IdOffreEmploi { get; set; }
        [ForeignKey("IdOffreEmploi")]
        public virtual OffreEmploi OffreEmploi { get; set; }
         
         
        


        
    }
}