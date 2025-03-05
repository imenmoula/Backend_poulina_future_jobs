using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
namespace Backend_poulina_future_jobs.Models
{
    

   
        public class Filiale
        {
            [Key]
            public Guid IdFiliale { get; set; } = Guid.NewGuid();
            public string Nom { get; set; } = string.Empty;
            public string Adresse { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Photo { get; set; } = string.Empty;

            // Propriété de navigation pour les départements
            public ICollection<Departement> Departements { get; set; } = new List<Departement>();
        }
    }

