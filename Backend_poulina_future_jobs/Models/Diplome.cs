using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Backend_poulina_future_jobs.Models
{
    public class Diplome
    {
        [Key]
        public Guid IdDiplome { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Le nom du diplôme est obligatoire.")]
        public string NomDiplome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le niveau du diplôme est obligatoire.")]
        public string Niveau { get; set; } = string.Empty;

        public string Domaine { get; set; } = string.Empty;

        public string Institution { get; set; } = string.Empty;

        public virtual ICollection<OffreEmploi> OffresEmploi { get; set; } = new List<OffreEmploi>();
    }
}