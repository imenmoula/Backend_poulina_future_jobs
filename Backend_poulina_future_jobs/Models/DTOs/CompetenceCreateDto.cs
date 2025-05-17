

using System.ComponentModel.DataAnnotations;

namespace Backend_poulina_future_jobs.Models.DTOs
{
    public class CompetenceCreateDto
    {
        public Guid Id { get; set; }
        [Required]
        public string Nom { get; set; }
        [Required]
        public string Description { get; set; }
        public bool EstTechnique { get; set; }
        public bool EstSoftSkill { get; set; }
    }
}