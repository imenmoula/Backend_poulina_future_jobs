namespace Backend_poulina_future_jobs.Models.DTOs
{
    public class DepartementDto
    {
        public Guid IdDepartement { get; set; }
        public string Nom { get; set; } // Adjust based on your Departement model
        public string Description { get; set; } = string.Empty;

        public Guid IdFiliale { get; set; }
        public FilialeDto Filiale { get; set; }
    }
}
