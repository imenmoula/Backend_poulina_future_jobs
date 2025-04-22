namespace Backend_poulina_future_jobs.DTOs
{
    public class CompetenceDTO
    {
        public Guid Id { get; set; }
        public string Nom { get; set; }
        public string Description { get; set; }
        public bool EstTechnique { get; set; }
        public bool EstSoftSkill { get; set; }
    }
}