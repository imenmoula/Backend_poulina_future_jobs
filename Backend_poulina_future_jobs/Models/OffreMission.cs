using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_poulina_future_jobs.Models
{
    public class OffreMission
    {
        [Key]
        public Guid IdOffreMission { get; set; } = Guid.NewGuid();

        public Guid IdOffreEmploi { get; set; }
        [ForeignKey("IdOffreEmploi")]
        public virtual OffreEmploi OffreEmploi { get; set; }

        [Required(ErrorMessage = "La description de la mission est obligatoire.")]
        public string DescriptionMission { get; set; } = string.Empty;

        public int Priorite { get; set; } = 1;

    }
}