using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Backend_poulina_future_jobs.Models;

namespace Backend_poulina_future_jobs.Models.DTOs
{
    public class CompetenceDTO
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nom { get; set; }

        [Required]
        [MaxLength(200)]
        public string Description { get; set; }

        public DateTime DateModification { get; set; }

        [Required(ErrorMessage = "Les HardSkills sont obligatoires.")]
        public List<HardSkillType> HardSkills { get; set; } = new List<HardSkillType>();

        [Required(ErrorMessage = "Les SoftSkills sont obligatoires.")]
        public List<SoftSkillType> SoftSkills { get; set; } = new List<SoftSkillType>();
    }
}