using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_poulina_future_jobs.Models
{
    public class Certificat
    {
        [Key]
        public Guid IdCertificat { get; set; }

        public Guid AppUserId { get; set; }
        [Required]
        [MaxLength(100)]
        public string Nom { get; set; }

        [Required]
        public DateTime DateObtention { get; set; }

        [Required]
        [MaxLength(150)]
        public string Organisme { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(255)]
        public string UrlDocument { get; set; } = string.Empty;

        [ForeignKey("AppUserId")]
        public AppUser AppUser { get; set; }
    }
}
