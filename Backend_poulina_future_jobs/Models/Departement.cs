using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend_poulina_future_jobs.Models
{
    public class Departement
    {
        [Key]

        public Guid IdDepartement { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nom { get; set; } = string.Empty;
        [Required(ErrorMessage = "La description est requise")]
        [StringLength(10000, ErrorMessage = "La description ne peut pas dépasser 500 caractères")]
        public string Description { get; set; }

        
        public DateTime DateCreation { get; set; } = DateTime.UtcNow;


        [Required]
        public Guid IdFiliale { get; set; }

        [ForeignKey("IdFiliale")]
        [JsonIgnore]
        public Filiale Filiale { get; set; }


    }

}
