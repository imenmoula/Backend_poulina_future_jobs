// ResultatQuiz.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_poulina_future_jobs.Models
{
    public class ResultatQuiz
    {
        [Key]
        public Guid ResultatId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TentativeId { get; set; }

        public double Score { get; set; }

        public int QuestionsCorrectes { get; set; }

        public int NombreQuestions { get; set; }

        public int TempsTotal { get; set; }

        public bool Reussi { get; set; }

        [StringLength(1000)]
        public string Commentaire { get; set; }

        public DateTime DateResultat { get; set; } = DateTime.Now;

        [ForeignKey("TentativeId")]
        public virtual TentativeQuiz Tentative { get; set; }
        public Guid QuizId { get; internal set; }
        public Guid AppUserId { get; internal set; }
    }
}