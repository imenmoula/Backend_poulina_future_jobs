// Quiz.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_poulina_future_jobs.Models
{
    public class Quiz
    {
        [Key]
        public Guid QuizId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(100)]
        public string Titre { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public DateTime DateCreation { get; set; } = DateTime.Now;

        public bool EstActif { get; set; } = true;

        public int Duree { get; set; }

        public int ScoreMinimum { get; set; } = 60;

        public Guid? OffreEmploiId { get; set; }

        [ForeignKey("OffreEmploiId")]
        public virtual OffreEmploi OffreEmploi { get; set; }

        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
        public virtual ICollection<TentativeQuiz> Tentatives { get; set; } = new List<TentativeQuiz>();
    }
}
