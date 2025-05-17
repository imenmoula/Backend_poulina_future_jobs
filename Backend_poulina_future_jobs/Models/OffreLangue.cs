using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_poulina_future_jobs.Models
{
    public enum Langue
    {
        Francais = 0,
        Anglais = 1,
        Espagnol = 2,
        Allemand = 3,
        Arabe = 4,
        Italien = 5
    }

    public class OffreLangue
    {
        [Key]
        public Guid IdOffreLangue { get; set; } = Guid.NewGuid();

        public Guid IdOffreEmploi { get; set; }
        [ForeignKey("IdOffreEmploi")]
        public virtual OffreEmploi OffreEmploi { get; set; }

        [Required(ErrorMessage = "La langue est obligatoire.")]
        public Langue Langue { get; set; }

        public string NiveauRequis { get; set; } = string.Empty;
    }
}