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

        public DateTime DateDebut { get; set; } = DateTime.Now;

        public DateTime? DateFin { get; set; }

        public StatutTentative Statut { get; set; } = StatutTentative.EnCours;

        public double Score { get; set; }

        // Navigation properties
        [ForeignKey("AppUserId")]
        public AppUser AppUser { get; set; }

        [ForeignKey("QuizId")]
        public Quiz Quiz { get; set; }

        public virtual ResultatQuiz Resultat { get; set; }

        public virtual ICollection<ReponseUtilisateur> ReponsesUtilisateur { get; set; } = new List<ReponseUtilisateur>();
    }
}