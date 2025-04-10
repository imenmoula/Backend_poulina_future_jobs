using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Backend_poulina_future_jobs.Models;

namespace Backend_poulina_future_jobs.Models.DTOs
{
    public class CompetenceCreateDto
    {
        [Required(ErrorMessage = "Le nom de la compétence est obligatoire")]
        [MaxLength(50, ErrorMessage = "Le nom ne doit pas dépasser 50 caractères")]
        public string Nom { get; set; }

        [Required(ErrorMessage = "La description de la compétence est obligatoire")]
        [MaxLength(200, ErrorMessage = "La description ne doit pas dépasser 200 caractères")]
        public string Description { get; set; }

        // Type de compétence
        [Required(ErrorMessage = "Veuillez préciser s'il s'agit d'une compétence technique")]
        public bool EstTechnique { get; set; }

        [Required(ErrorMessage = "Veuillez préciser s'il s'agit d'une soft skill")]
        public bool EstSoftSkill { get; set; }

    }
}