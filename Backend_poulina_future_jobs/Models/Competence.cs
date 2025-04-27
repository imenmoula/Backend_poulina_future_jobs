
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
        public string Nom { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime dateAjout { get; set; } = DateTime.UtcNow;

        public DateTime DateModification { get; set; } = DateTime.UtcNow;

        public bool estTechnique { get; set; }
        public bool estSoftSkill { get; set; }
        public ICollection<AppUserCompetence> AppUserCompetences { get; set; } = new List<AppUserCompetence>();
        public virtual ICollection<OffreCompetences> OffreCompetences { get; set; } = new List<OffreCompetences>();
    }
}