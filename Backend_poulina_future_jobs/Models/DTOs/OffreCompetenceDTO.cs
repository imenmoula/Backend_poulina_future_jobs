using System;
using System.ComponentModel.DataAnnotations;

namespace Backend_poulina_future_jobs.Models.DTOs
{
    public class OffreCompetenceDTO
    {
        public Guid IdOffreEmploi { get; set; }
        public Guid IdCompetence { get; set; }
        [Required(ErrorMessage = "Le niveau requis est obligatoire.")]
        [EnumDataType(typeof(NiveauRequisType), ErrorMessage = "Le niveau requis doit être Débutant, Intermédiaire, Avancé ou Expert.")]
        public NiveauRequisType NiveauRequis { get; set; }

        public CompetenceCreateDto? Competence { get; set; }
    }
}