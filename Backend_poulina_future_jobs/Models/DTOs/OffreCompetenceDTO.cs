using System.ComponentModel.DataAnnotations;

namespace Backend_poulina_future_jobs.Models.DTOs
{
    public class OffreCompetenceDTO
    {


        public Guid? IdOffreEmploi { get; set; }
        public Guid? IdCompetence { get; set; }
        public CompetenceCreateDto? Competence { get; set; } // Not nullable
        public NiveauRequisType NiveauRequis { get; set; }

    }
}
