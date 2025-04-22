using System.ComponentModel.DataAnnotations;

namespace Backend_poulina_future_jobs.Models
{
    public class candiadate_competence
    {
        [Key]
        public Guid Id { get; set; }

        public Guid AppUserId { get; set; }
        public Guid CompetenceId { get; set; }

        [Required]
        [MaxLength(20)]
        public string NiveauPossede { get; set; }

        public AppUser AppUser { get; set; }
        public Competence Competence { get; set; }
    }
}
