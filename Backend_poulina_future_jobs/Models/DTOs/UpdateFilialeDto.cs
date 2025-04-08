namespace Backend_poulina_future_jobs.Models.DTOs
{
    public class UpdateFilialeDto
    {
        public Guid IdFiliale { get; set; }
        public string Nom { get; set; } = string.Empty;
        public string Adresse { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Photo { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Fax { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SiteWeb { get; set; } = string.Empty;

    }
}
