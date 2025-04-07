using System.ComponentModel.DataAnnotations;

namespace Backend_poulina_future_jobs.Models
{
    public class OffreCompetenceDTO
    {
        internal Guid IdCompetence;

        [Required]
        public Guid IdOffreEmploi { get; set; }
    }
}
