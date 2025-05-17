namespace Backend_poulina_future_jobs.Models.DTOs
{
    public class CreateDepartementDTO
    {
        public string Nom { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public Guid IdFiliale { get; set; }

    }
}
