//using Backend_poulina_future_jobs.Models;
//using System.ComponentModel.DataAnnotations;

//namespace Backend_poulina_future_jobs.DTOs
//{
//    // DTO for Candidature
//    public class CandidatureDto
//    {
//        public Guid IdCandidature { get; set; }

//        [Required(ErrorMessage = "AppUserId is required.")]
//        public Guid AppUserId { get; set; }

//        [Required(ErrorMessage = "OffreId is required.")]
//        public Guid OffreId { get; set; }

//        [Required(ErrorMessage = "Statut is required.")]
//        [MaxLength(50, ErrorMessage = "Statut cannot exceed 50 characters.")]
//        public string Statut { get; set; }

//        [MaxLength(1000, ErrorMessage = "MessageMotivation cannot exceed 1000 characters.")]
//        public string MessageMotivation { get; set; }

//        [Required(ErrorMessage = "DateSoumission is required.")]
//        public DateTime DateSoumission { get; set; }

//        public AppUserDto AppUser { get; set; }
//        public OffreEmploiDto Offre { get; set; }
//    }

//    // DTO for AppUser
//    public class AppUserDto
//    {
//        public Guid Id { get; set; }

//        [MaxLength(150)]
//        public string FullName { get; set; }

//        [MaxLength(150)]
//        public string Nom { get; set; }

//        [MaxLength(150)]
//        public string Prenom { get; set; }

//        [EmailAddress]
//        public string Email { get; set; }

//        public string Phone { get; set; }

//        public string NiveauEtude { get; set; }

//        public string Diplome { get; set; }

//        public string Universite { get; set; }

//        public string Specialite { get; set; }

//        [MaxLength(255)]
//        public string Cv { get; set; }

//        [MaxLength(255)]
//        public string LinkedIn { get; set; }

//        [MaxLength(255)]
//        public string Github { get; set; }

//        [MaxLength(255)]
//        public string Portfolio { get; set; }

//        [MaxLength(20)]
//        public string Statut { get; set; }

//        public List<ExperienceDto> Experiences { get; set; }
//        public List<CandidateCompetenceDto> AppUserCompetences { get; set; }
//        public List<CertificatDto> Certificats { get; set; }
//        public List<DiplomeCandidateDto> DiplomesCandidate { get; set; }
//    }

//    // DTO for OffreEmploi
//    public class OffreEmploiDto
//    {
//        public Guid IdOffreEmploi { get; set; }

//        public string Specialite { get; set; }

//        public string Titre { get; set; }

//        public string Description { get; set; }

//        public DateTime DatePublication { get; set; }

//        public DateTime? DateExpiration { get; set; }

//        [Range(0, double.MaxValue)]
//        public decimal SalaireMin { get; set; }

//        [Range(0, double.MaxValue)]
//        public decimal SalaireMax { get; set; }

//        public string NiveauExperienceRequis { get; set; }

//        public string DiplomeRequis { get; set; }

//        public TypeContratEnum TypeContrat { get; set; }

//        public StatutOffre Statut { get; set; }

//        public ModeTravail ModeTravail { get; set; }

//        public int NombrePostes { get; set; }

//        public string Avantages { get; set; }
//    }

//    // DTO for Experience
//    public class ExperienceDto
//    {
//        public Guid IdExperience { get; set; }

//        [MaxLength(100)]
//        public string Poste { get; set; }

//        [MaxLength(1000)]
//        public string Description { get; set; }

//        [MaxLength(150)]
//        public string NomEntreprise { get; set; }

//        [MaxLength(255)]
//        public string CompetenceAcquise { get; set; }

//        public DateTime? DateDebut { get; set; }

//        public DateTime? DateFin { get; set; }

//    }

//    // DTO for Certificat
//    public class CertificatDto
//    {
//        public Guid IdCertificat { get; set; }

//        [MaxLength(100)]
//        public string Nom { get; set; }

//        public DateTime DateObtention { get; set; }

//        [MaxLength(150)]
//        public string Organisme { get; set; }

//        [MaxLength(1000)]
//        public string Description { get; set; }

//        [MaxLength(255)]
//        public string UrlDocument { get; set; }
//    }

//    // DTO for AppUserCompetence
//    public class CandidateCompetenceDto
//    {
//        public Guid Id { get; set; }

//        public Guid CompetenceId { get; set; }

//        public string NiveauPossede { get; set; }

//        public CompetenceDto Competence { get; set; }
//    }

//    // DTO for Competence
//    public class CompetenceDto
//    {
//        public Guid Id { get; set; }

//        public string Nom { get; set; }

//        public string Description { get; set; }

//        public bool EstTechnique { get; set; }

//        public bool EstSoftSkill { get; set; }
//    }

//    // DTO for DiplomeCandidate
//    public class DiplomeCandidateDto
//    {
//        public Guid IdDiplome { get; set; }

//        public Guid AppUserId { get; set; }

//        [MaxLength(150)]
//        public string NomDiplome { get; set; }

//        [MaxLength(150)]
//        public string Institution { get; set; }

//        public DateTime DateObtention { get; set; }

//        [MaxLength(255)]
//        public string Specialite { get; set; }

//        [MaxLength(255)]
//        public string UrlDocument { get; set; }
//    }

//    // ViewModel for submitting a candidature
//    public class CandidatureViewModel
//    {
//        [Required(ErrorMessage = "AppUserId is required.")]
//        public Guid AppUserId { get; set; }

//        [Required(ErrorMessage = "OffreId is required.")]
//        public Guid OffreId { get; set; }

//        [MaxLength(1000, ErrorMessage = "MessageMotivation cannot exceed 1000 characters.")]
//        public string MessageMotivation { get; set; }
//    }
//}