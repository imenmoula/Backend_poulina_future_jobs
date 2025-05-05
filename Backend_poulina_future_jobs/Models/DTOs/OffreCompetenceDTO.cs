//using System;
//using System.ComponentModel.DataAnnotations;
//using Backend_poulina_future_jobs.Models;

//namespace Backend_poulina_future_jobs.Dtos
//{
//    public class OffreCompetenceDto
//    {
//        public Guid IdOffreEmploi { get; set; }

//        public Guid IdCompetence { get; set; }

//        [Required(ErrorMessage = "Le niveau requis est obligatoire.")]
//        [EnumDataType(typeof(NiveauRequisType), ErrorMessage = "Le niveau requis doit être Débutant, Intermédiaire, Avancé ou Expert.")]
//        public NiveauRequisType NiveauRequis { get; set; }
//    }

//    public class CreateOffreCompetenceRequest
//    {
//        [Required(ErrorMessage = "Les données de la compétence sont obligatoires.")]
//        public OffreCompetenceDto Dto { get; set; }
//    }
//}