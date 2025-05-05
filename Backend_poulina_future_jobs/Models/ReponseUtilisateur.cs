// ReponseUtilisateur.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_poulina_future_jobs.Models
{
    public class ReponseUtilisateur
    {
        [Key]
        public Guid ReponseUtilisateurId { get; set; } = Guid.NewGuid();

        [Required]
        public Guid TentativeId { get; set; }

        [Required]
        public Guid QuestionId { get; set; }

        public Guid? ReponseId { get; set; }

        [StringLength(1000)]
        public string TexteReponse { get; set; }

        public int? TempsReponse { get; set; }

        public bool EstCorrecte { get; set; }

        [ForeignKey("TentativeId")]
        public virtual TentativeQuiz Tentative { get; set; }

        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }

        [ForeignKey("ReponseId")]
        public virtual Reponse Reponse { get; set; }
    }
}