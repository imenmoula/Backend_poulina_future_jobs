using System.ComponentModel.DataAnnotations;
using Backend_poulina_future_jobs.Models.DTOs;

namespace Backend_poulina_future_jobs.Models
{
    public class OffreCompetenceDTO
    {
        internal Guid IdCompetence;
        internal CompetenceCreateDto Competence;

        [Required]
        public Guid IdOffreEmploi { get; set; }
        public NiveauRequisType NiveauRequis { get; internal set; }
    }
}
