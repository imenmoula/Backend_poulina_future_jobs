using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_poulina_future_jobs.Models
{
    public class OffreCompetences
    {

        public Guid IdOffreEmploi { get; set; }
        [ForeignKey("IdOffreEmploi")]
        public virtual OffreEmploi OffreEmploi { get; set; }

        public Guid IdCompetence { get; set; }
        [ForeignKey("IdCompetence")]
        public virtual Competence Competence { get; set; }

        // Propriété pour le niveau requis
        [Required(ErrorMessage = "Le niveau requis est obligatoire.")]
        [EnumDataType(typeof(NiveauRequisType), ErrorMessage = "Le niveau requis doit être Débutant, Intermédiaire ou Avancé.")]
        public NiveauRequisType NiveauRequis { get; set; }
    }

    public enum NiveauRequisType
    {
        Debutant,
        Intermediaire,
        Avance,
        Expert
    }
}

