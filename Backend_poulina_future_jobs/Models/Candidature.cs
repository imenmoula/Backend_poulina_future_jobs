using System.ComponentModel.DataAnnotations;

namespace Backend_poulina_future_jobs.Models
{
    public class Candidature
    {
        [Key]
        public Guid IdCandidature { get; set;}

        public Guid AppUserId { get; set; }
        public Guid OffreId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Statut { get; set; }
        [MaxLength(1000)] // Limite le message à 1000 caractères

        public string MessageMotivation { get; set; }

        [Required]
        public DateTime DateSoumission { get; set; }

        public AppUser AppUser { get; set; }
        public OffreEmploi Offre { get; set; }
    }
    }
