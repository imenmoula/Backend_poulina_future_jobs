//// DTOs spécifiques pour les entrées du PostulerCandidateController
//// Ces DTOs sont conçus pour éviter les conflits de noms et inclure les champs nécessaires,
//// notamment pour DiplomeCandidate.

//using Microsoft.AspNetCore.Http;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;

//namespace Backend_poulina_future_jobs.Dtos.PostulerCandidate // Suggestion de sous-namespace
//{
//    // DTO principal pour la soumission/mise à jour de candidature
//    public class CandidatureInputDto
//    {
//        // Pour la mise à jour, non requis à la création
//        public Guid? IdCandidature { get; set; }

//        [Required]
//        public Guid AppUserId { get; set; }
//        [Required]
//        public Guid OffreId { get; set; }

//        [MaxLength(1000)]
//        public string MessageMotivation { get; set; }

//        // Fichiers
//        public IFormFile CvFile { get; set; } // Fichier CV
//        public IFormFile LettreMotivationFile { get; set; } // Fichier Lettre de Motivation

//        // Liens
//        [Url(ErrorMessage = "URL LinkedIn invalide")]
//        public string LinkedIn { get; set; } = string.Empty;
//        [Url(ErrorMessage = "URL GitHub invalide")]
//        public string Github { get; set; } = string.Empty;
//        [Url(ErrorMessage = "URL Portfolio invalide")]
//        public string Portfolio { get; set; } = string.Empty;

//        [Required]
//        [MaxLength(20)]
//        public string StatutCandidate { get; set; } = "Debutant";

//        // Informations utilisateur de base (si mises à jour via ce formulaire)
//        public string Nom { get; set; }
//        public string Prenom { get; set; }
//        // Ajoutez d'autres champs AppUser si nécessaire

//        // Listes des entités liées (utilisant les DTOs d'input spécifiques ci-dessous)
//        public List<DiplomeCandidatureInputDto> Diplomes { get; set; } = new List<DiplomeCandidatureInputDto>();
//        public List<ExperienceInputDto> Experiences { get; set; } = new List<ExperienceInputDto>();
//        public List<CompetenceInputDto> Competences { get; set; } = new List<CompetenceInputDto>();
//        public List<CertificatInputDto> Certificats { get; set; } = new List<CertificatInputDto>();
//    }

//    // DTO spécifique pour les informations de diplôme du candidat
//    public class DiplomeCandidatureInputDto
//    {
//        // Pas besoin d'ID ici, car on gère la création/suppression complète à chaque fois
//        // public Guid? IdDiplome { get; set; } // Optionnel si on veut gérer la MàJ individuelle

//        [Required]
//        [MaxLength(150)]
//        public string NomDiplome { get; set; }

//        [Required]
//        [MaxLength(150)]
//        public string Institution { get; set; }

//        [Required]
//        public DateTime DateObtention { get; set; }

//        [MaxLength(255)]
//        public string Specialite { get; set; } = string.Empty;

//        [MaxLength(255)]
//        [Url(ErrorMessage = "URL Document invalide")]
//        public string UrlDocument { get; set; } = string.Empty; // Champ spécifique à DiplomeCandidate
//    }

//    // DTO spécifique pour les informations d'expérience
//    public class ExperienceInputDto
//    {
//        // Ajoutez les propriétés nécessaires pour créer/mettre à jour une Experience
//        // Exemple :
//        [Required]
//        public string TitrePoste { get; set; }
//        [Required]
//        public string NomEntreprise { get; set; }
//        public string Description { get; set; }
//        [Required]
//        public DateTime DateDebut { get; set; }
//        public DateTime? DateFin { get; set; } // Nullable si poste actuel
//        // ... autres champs pertinents
//    }

//    // DTO spécifique pour les compétences (souvent juste l'ID et le niveau)
//    public class CompetenceInputDto
//    {
//        [Required]
//        public Guid CompetenceId { get; set; } // ID de la compétence existante

//        // Ajoutez le niveau si vous gérez le niveau possédé par le candidat
//        // public NiveauCompetence NiveauPossede { get; set; } // Assurez-vous que l'enum NiveauCompetence existe
//    }

//    // DTO spécifique pour les informations de certificat
//    public class CertificatInputDto
//    {
//        // Ajoutez les propriétés nécessaires pour créer/mettre à jour un Certificat
//        // Exemple :
//        [Required]
//        public string NomCertificat { get; set; }
//        [Required]
//        public string Organisation { get; set; }
//        [Required]
//        public DateTime DateObtention { get; set; }
//        public DateTime? DateExpiration { get; set; } // Nullable si pas d'expiration
//        public string UrlCertificat { get; set; } // Optionnel
//        // ... autres champs pertinents
//    }
//}

