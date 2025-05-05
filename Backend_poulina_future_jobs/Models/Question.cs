// Question.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_poulina_future_jobs.Models
{
    public enum TypeQuestion
    {
        ChoixUnique = 0,
        ChoixMultiple = 1,
        VraiFaux = 2,
        ReponseTexte = 3
    }

    public class Question
    {
        [Key]
        public Guid QuestionId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(500)]
        public string Texte { get; set; }

        public TypeQuestion Type { get; set; } = TypeQuestion.ChoixUnique;

        public int Points { get; set; } = 1;

        public int Ordre { get; set; }

        public int? TempsRecommande { get; set; }

        [Required]
        public Guid QuizId { get; set; }

        [ForeignKey("QuizId")]
        public virtual Quiz Quiz { get; set; }

        public virtual ICollection<Reponse> Reponses { get; set; } = new List<Reponse>();
        public virtual ICollection<ReponseUtilisateur> ReponsesUtilisateur { get; set; } = new List<ReponseUtilisateur>();
    }
}
