using System.ComponentModel.DataAnnotations;

namespace Backend_poulina_future_jobs.Models.DTOs
{
    public class UpdateDepartementDTO
    {
        [Required]
        public string Nom { get; set; }

        public string Description { get; set; }


        [Required]
        public Guid IdFiliale { get; set; }
    }
}
