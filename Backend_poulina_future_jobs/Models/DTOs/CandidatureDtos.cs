namespace Backend_poulina_future_jobs.Models
{
    public record CandidatureDiplomeDto(
        Guid Id,
        string NomDiplome,
        string Institution,
        DateTime DateObtention,
        string Specialite = null,
        string UrlDocument = null,
        Guid idDiplome = default);

    public record CandidatureExperienceDto(
        Guid Id,
        string Poste,
        string NomEntreprise,
        DateTime DateDebut,
        DateTime? DateFin,
        string Description = null,
        string CompetenceAcquise = null);

    public record CandidatureCertificatDto(
        Guid Id,
        string Nom,
        string Organisme,
        DateTime DateObtention,
        string Description = null,
        string UrlDocument = null);
}