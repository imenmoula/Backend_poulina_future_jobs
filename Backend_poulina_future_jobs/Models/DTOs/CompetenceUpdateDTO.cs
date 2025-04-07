using System.ComponentModel.DataAnnotations;

namespace Backend_poulina_future_jobs.Models.DTOs
{
    public class CompetenceUpdateDTO
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Nom { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string TypeCompetence { get; set; } // "HardSkill" ou "SoftSkill"

        [Required]
        [Range(1, 5, ErrorMessage = "Le niveau doit être entre 1 (Débutant) et 5 (Expert).")]
        public int NiveauRequis { get; set; }

        public DateTime DateModification { get; set; }

    }

}
