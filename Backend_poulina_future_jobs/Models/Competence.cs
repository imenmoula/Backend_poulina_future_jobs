using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Backend_poulina_future_jobs.Models
{
    public class Competence
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(50)]
        public string Nom { get; set; }

        [Required]
        [MaxLength(200)]
        public string Description { get; set; }


        public DateTime dateAjout { get; set; } = DateTime.UtcNow;

        public DateTime DateModification { get; set; } = DateTime.UtcNow;

        public bool estTechnique { get; set; }
        public bool estSoftSkill { get; set; }


        // Many-to-many relationship with OffreEmploi via OffreCompetences
        public virtual ICollection<OffreCompetences> OffreCompetences { get; set; } = new List<OffreCompetences>();
    }

  
}