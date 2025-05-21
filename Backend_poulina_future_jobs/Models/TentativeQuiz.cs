// TentativeQuiz.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_poulina_future_jobs.Models
{
    public enum StatutTentative
    {

        EnCours = 0,
        Terminee = 1,
        Abandonnee = 2,
        Expiree = 3

    }

    public class TentativeQuiz
    {
        [Key]
        public Guid TentativeId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid QuizId { get; set; }

        [Required]
        public Guid AppUserId { get; set; }
        
        public Guid? CandidatureId { get; set; } // Note the ? making it nullable
        public DateTime? DateDebut { get; set; } // Correction: nullable

        public DateTime? DateFin { get; set; }

        public StatutTentative Statut { get; set; } = StatutTentative.Expiree;

        public double? Score { get; set; } // Correction: nullable

        public string Token { get; set; } // Pour sécuriser le lien


        // Navigation properties
        [ForeignKey("AppUserId")]
        public AppUser AppUser { get; set; }

        [ForeignKey("QuizId")]
        public Quiz Quiz { get; set; }
        // Propriété de navigation pour la relation avec Candidature
        [ForeignKey("CandidatureId")]
        public Candidature Candidature { get; set; }

        public virtual ResultatQuiz Resultat { get; set; }

        public virtual ICollection<ReponseUtilisateur> ReponsesUtilisateur { get; set; } = new List<ReponseUtilisateur>();
    }
}