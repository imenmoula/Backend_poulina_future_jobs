// Reponse.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_poulina_future_jobs.Models
{
    public class Reponse
    {
        [Key]
        public Guid ReponseId { get; set; } = Guid.NewGuid();

        [Required]
        [StringLength(500)]
        public string Texte { get; set; }

        public bool EstCorrecte { get; set; } = false;

        public int Ordre { get; set; }

        [StringLength(500)]
        public string Explication { get; set; }

        [Required]
        public Guid QuestionId { get; set; }

        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }

        public virtual ICollection<ReponseUtilisateur> ReponsesUtilisateur { get; set; } = new List<ReponseUtilisateur>();
    }
}
