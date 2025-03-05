using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend_poulina_future_jobs.Models
{
    public class Departement
    {
        [Key]
        public Guid IdDepartement { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DateCreation { get; set; }
        public Guid IdFiliale { get; set; }

        [ForeignKey("IdFiliale")]
        public Filiale Filiale { get; set; }
    }
}

