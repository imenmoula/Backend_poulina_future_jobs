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


        public string CvFilePath { get; set; }  // Chemin d'accès physique
        public string LettreMotivation { get; set; } = string.Empty;


        [Url(ErrorMessage = "URL LinkedIn invalide")]
        public string LinkedIn { get; set; } = string.Empty;

        [Url(ErrorMessage = "URL GitHub invalide")]
        public string Github { get; set; } = string.Empty;

        [Url(ErrorMessage = "URL Portfolio invalide")]
        public string Portfolio { get; set; } = string.Empty;

       

        [Required]
        [MaxLength(20)]
        public string StatutCandidate { get; set; } = "Debutant";

        public AppUser AppUser { get; set; }
        public OffreEmploi Offre { get; set; }
        // Nouvelle propriété de navigation pour la relation avec TentativeQuiz
        public virtual ICollection<TentativeQuiz> Tentatives { get; set; } = new List<TentativeQuiz>();
    }
    }
