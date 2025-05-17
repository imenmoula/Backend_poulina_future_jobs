using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Backend_poulina_future_jobs.Models
{
    public class DiplomeCandidate
    {
        [Key]
        public Guid IdDiplome { get; set; }

        public Guid AppUserId { get; set; }

        [Required]
        [MaxLength(150)]
        public string NomDiplome { get; set; }

        [Required]
        [MaxLength(150)]
        public string Institution { get; set; }

        [Required]
        public DateTime DateObtention { get; set; }

        [MaxLength(255)]
        public string Specialite { get; set; } = string.Empty;

        [MaxLength(255)]
        public string UrlDocument { get; set; } = string.Empty;

        [ForeignKey("AppUserId")]
        public AppUser AppUser { get; set; }

    }
}
