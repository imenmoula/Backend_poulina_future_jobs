using System.ComponentModel.DataAnnotations;

namespace Backend_poulina_future_jobs.Models
{
    public class Experience
    {
        [Key]
        public Guid IdExperience { get; set; }

        public Guid AppUserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Poste { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string NomEntreprise { get; set; }

        [MaxLength(255)]
        public string CompetenceAcquise { get; set; } = string.Empty;

        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }

        public AppUser AppUser { get; set; }
        public ICollection<Certificat> Certificats { get; set; } = new List<Certificat>();
    }
}
