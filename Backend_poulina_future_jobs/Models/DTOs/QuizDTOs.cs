//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using Backend_poulina_future_jobs.Models;

//namespace Backend_poulina_future_jobs.DTOs
//{
//    // DTOs pour la création et mise à jour
//    public class QuizCreateDto
//    {
//        [Required]
//        [StringLength(100)]
//        public string Titre { get; set; } = string.Empty;

//        [StringLength(500)]
//        public string Description { get; set; }

//        public int Duree { get; set; } // En minutes  
//        public int ScoreMinimum { get; set; } = 60;
//        public Guid? OffreEmploiId { get; set; }
//    }

//    public class QuizUpdateDto
//    {
//        [StringLength(100)]
//        public string Titre { get; set; }

//        [StringLength(500)]
//        public string Description { get; set; }

//        public int? Duree { get; set; }
//        public int? ScoreMinimum { get; set; }
//        public Guid? OffreEmploiId { get; set; }
//        public bool? EstActif { get; set; }
//    }

//    // DTOs pour les réponses
//    public class QuizResponseDto
//    {
//        public Guid QuizId { get; set; }
//        public string Titre { get; set; }
//        public string Description { get; set; }
//        public DateTime DateCreation { get; set; }
//        public bool EstActif { get; set; }
//        public int Duree { get; set; }
//        public int ScoreMinimum { get; set; }
//        public Guid? OffreEmploiId { get; set; }
//    }

//    public class QuizFullResponseDto
//    {
//        public Guid QuizId { get; set; }
//        public string Titre { get; set; }
//        public string Description { get; set; }
//        public DateTime DateCreation { get; set; }
//        public bool EstActif { get; set; }
//        public int Duree { get; set; }
//        public int ScoreMinimum { get; set; }
//        public Guid? OffreEmploiId { get; set; }
//        public List<QuestionResponseDto> Questions { get; set; } = new List<QuestionResponseDto>();
//    }

//    public class QuestionResponseDto
//    {
//        public Guid QuestionId { get; set; }
//        public string Texte { get; set; }
//        public TypeQuestion Type { get; set; }
//        public int Points { get; set; }
//        public int Ordre { get; set; }
//        public int? TempsRecommande { get; set; }
//        public Guid QuizId { get; set; }
//        public List<ReponseResponseDto> Reponses { get; set; } = new List<ReponseResponseDto>();
//    }

//    public class ReponseResponseDto
//    {
//        public Guid ReponseId { get; set; }
//        public string Texte { get; set; }
//        public bool EstCorrecte { get; set; }
//        public int Ordre { get; set; }
//        public string Explication { get; set; }
//        public Guid QuestionId { get; set; }
//    }

//    // DTOs pour la création complète
//    public class CreateFullQuizDto
//    {
//        [Required]
//        [StringLength(100)]
//        public string Titre { get; set; }

//        [StringLength(500)]
//        public string Description { get; set; }

//        [Required]
//        [Range(1, 180)]
//        public int Duree { get; set; } // En minutes

//        [Required]
//        [Range(0, 100)]
//        public int ScoreMinimum { get; set; } = 60;

//        public Guid? OffreEmploiId { get; set; }

//        [Required]
//        public List<CreateQuestionDto> Questions { get; set; } = new List<CreateQuestionDto>();
//    }

//    public class CreateQuestionDto
//    {
//        [Required]
//        [StringLength(500)]
//        public string Texte { get; set; }

//        [Required]
//        public TypeQuestion Type { get; set; }

//        [Required]
//        [Range(1, 100)]
//        public int Points { get; set; } = 1;

//        [Required]
//        public int Ordre { get; set; }

//        public int? TempsRecommande { get; set; }

//        public List<CreateReponseDto> Reponses { get; set; } = new List<CreateReponseDto>();
//    }

//    public class CreateReponseDto
//    {
//        [Required]
//        [StringLength(500)]
//        public string Texte { get; set; }

//        [Required]
//        public bool EstCorrecte { get; set; }

//        [Required]
//        public int Ordre { get; set; }

//        [StringLength(500)]
//        public string Explication { get; set; }
//    }

//    // DTOs pour l'envoi de convocation
//    public class EnvoyerConvocationDto
//    {
//        [Required]
//        public Guid CandidatureId { get; set; }

//        [Required]
//        public Guid QuizId { get; set; }

//        [Required]
//        [Url]
//        public string BaseUrl { get; set; }
//    }

//    // DTOs pour la soumission et les résultats
//    public class SoumettreQuizDto
//    {
//        [Required]
//        public Guid TentativeId { get; set; }

//        [Required]
//        public List<ReponseUtilisateurDto> Reponses { get; set; } = new List<ReponseUtilisateurDto>();
//    }

//    public class ReponseUtilisateurDto
//    {
//        [Required]
//        public Guid QuestionId { get; set; }

//        public List<Guid> ReponseIds { get; set; } = new List<Guid>();

//        public string TexteReponse { get; set; }

//        public int? TempsReponse { get; set; }
//    }
//    public class ConvocationQuizDto
//    {
//        [Required]
//        public Guid CandidatureId { get; set; }

//        [Required]
//        public Guid QuizId { get; set; }

//        [Required]
//        [Url]
//        public string BaseUrl { get; set; }
//    }

//    public class ResultatQuizResponseDto
//    {
//        public Guid ResultatId { get; set; }
//        public Guid TentativeId { get; set; }
//        public double Score { get; set; }
//        public int QuestionsCorrectes { get; set; }
//        public int NombreQuestions { get; set; }
//        public int TempsTotal { get; set; }
//        public bool Reussi { get; set; }
//        public DateTime DateResultat { get; set; }
//    }

//    public class ResultatDetailResponseDto
//    {
//        public Guid ResultatId { get; set; }
//        public Guid TentativeId { get; set; }
//        public Guid QuizId { get; set; }
//        public string QuizTitre { get; set; }
//        public double Score { get; set; }
//        public int QuestionsCorrectes { get; set; }
//        public int NombreQuestions { get; set; }
//        public int TempsTotal { get; set; }
//        public bool Reussi { get; set; }
//        public DateTime DateResultat { get; set; }
//        public List<ReponseDetailResponseDto> ReponsesDetail { get; set; } = new List<ReponseDetailResponseDto>();
//    }

//    public class ReponseDetailResponseDto
//    {
//        public Guid QuestionId { get; set; }
//        public string QuestionTexte { get; set; }
//        public TypeQuestion QuestionType { get; set; }
//        public bool EstCorrecte { get; set; }
//        public int? TempsReponse { get; set; }
//        public string TexteReponse { get; set; }
//        public Guid? ReponseId { get; set; }
//        public string ReponseTexte { get; set; }
//        public string Explication { get; set; }
//    }

//}

/*****************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Backend_poulina_future_jobs.Models;

namespace Backend_poulina_future_jobs.DTOs
{
    // DTOs pour Quiz

    /// <summary>
    /// DTO pour la création d'un quiz simple
    /// </summary>
    public class QuizCreateDto
    {
        [Required(ErrorMessage = "Le titre est obligatoire")]
        [StringLength(100, ErrorMessage = "Le titre ne peut pas dépasser 100 caractères")]
        public string Titre { get; set; }

        [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères")]
        public string Description { get; set; }

        [Required(ErrorMessage = "La durée est obligatoire")]
        [Range(1, int.MaxValue, ErrorMessage = "La durée doit être positive")]
        public int Duree { get; set; }

        [Required(ErrorMessage = "Le score minimum est obligatoire")]
        [Range(0, 100, ErrorMessage = "Le score minimum doit être compris entre 0 et 100")]
        public int ScoreMinimum { get; set; } = 60;

        public Guid? OffreEmploiId { get; set; }
    }

    /// <summary>
    /// DTO pour la mise à jour d'un quiz
    /// </summary>
    public class QuizUpdateDto
    {
        [StringLength(100, ErrorMessage = "Le titre ne peut pas dépasser 100 caractères")]
        public string Titre { get; set; }

        [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères")]
        public string Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La durée doit être positive")]
        public int? Duree { get; set; }

        [Range(0, 100, ErrorMessage = "Le score minimum doit être compris entre 0 et 100")]
        public int? ScoreMinimum { get; set; }

        public Guid? OffreEmploiId { get; set; }

        public bool? EstActif { get; set; }
    }

    /// <summary>
    /// DTO pour la réponse d'un quiz simple
    /// </summary>
    public class QuizResponseDto
    {
        public Guid QuizId { get; set; }
        public string Titre { get; set; }
        public string Description { get; set; }
        public DateTime DateCreation { get; set; }
        public bool EstActif { get; set; }
        public int Duree { get; set; }
        public int ScoreMinimum { get; set; }
        public Guid? OffreEmploiId { get; set; }
    }

    /// <summary>
    /// DTO pour la réponse d'un quiz complet avec questions et réponses
    /// </summary>
    public class QuizFullResponseDto
    {
        public Guid QuizId { get; set; }
        public string Titre { get; set; }
        public string Description { get; set; }
        public DateTime DateCreation { get; set; }
        public bool EstActif { get; set; }
        public int Duree { get; set; }
        public int ScoreMinimum { get; set; }
        public Guid? OffreEmploiId { get; set; }
        public List<QuestionsResponseDtos> Questions { get; set; }
    }

    /// <summary>
    /// DTO pour la création d'un quiz complet avec questions et réponses
    /// </summary>
    public class CreateFullQuizDto
    {
        [Required(ErrorMessage = "Le titre est obligatoire")]
        [StringLength(100, ErrorMessage = "Le titre ne peut pas dépasser 100 caractères")]
        public string Titre { get; set; }

        [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères")]
        public string Description { get; set; }

        [Required(ErrorMessage = "La durée est obligatoire")]
        [Range(1, int.MaxValue, ErrorMessage = "La durée doit être positive")]
        public int Duree { get; set; }

        [Required(ErrorMessage = "Le score minimum est obligatoire")]
        [Range(0, 100, ErrorMessage = "Le score minimum doit être compris entre 0 et 100")]
        public int ScoreMinimum { get; set; } = 60;

        public Guid? OffreEmploiId { get; set; }

        [Required(ErrorMessage = "Au moins une question est requise")]
        public List<QuestionCreateDto> Questions { get; set; }
    }

    /// <summary>
    /// DTO pour les statistiques d'un quiz
    /// </summary>
    public class QuizStatistiquesDto
    {
        public Guid QuizId { get; set; }
        public string QuizTitre { get; set; }
        public int NombreTentatives { get; set; }
        public int NombreReussites { get; set; }
        public double TauxReussite { get; set; }
        public double ScoreMoyen { get; set; }
        public int TempsCompletionMoyen { get; set; }
    }

    // DTOs pour Question

    /// <summary>
    /// DTO pour la création d'une question
    /// </summary>
    public class QuestionCreateDto
    {
        [Required(ErrorMessage = "Le texte de la question est obligatoire")]
        [StringLength(500, ErrorMessage = "Le texte ne peut pas dépasser 500 caractères")]
        public string Texte { get; set; }

        [Required(ErrorMessage = "Le type de question est obligatoire")]
        public TypeQuestion Type { get; set; } = TypeQuestion.ChoixUnique;

        [Range(1, int.MaxValue, ErrorMessage = "Les points doivent être positifs")]
        public int Points { get; set; } = 1;

        [Required(ErrorMessage = "L'ordre est obligatoire")]
        public int Ordre { get; set; }

        public int? TempsRecommande { get; set; }
        public Guid QuizId { get; set; } // Added property to fix CS1061  


        public List<ReponseCreateDto> Reponses { get; set; }
    }

    /// <summary>
    /// DTO pour la réponse d'une question
    /// </summary>
    public class QuestionsResponseDtos
    {
        public Guid QuestionId { get; set; }
        public string Texte { get; set; }
        public TypeQuestion Type { get; set; }
        public int Points { get; set; }
        public int Ordre { get; set; }
        public int? TempsRecommande { get; set; }
        public Guid QuizId { get; set; }
        public List<ReponseResponseDtos> Reponses { get; set; }
    }

    // DTOs pour Reponse

    /// <summary>
    /// DTO pour la création d'une réponse
    /// </summary>
    public class ReponseCreateDto
    {
        [Required(ErrorMessage = "Le texte de la réponse est obligatoire")]
        [StringLength(500, ErrorMessage = "Le texte ne peut pas dépasser 500 caractères")]
        public string Texte { get; set; }

        public bool EstCorrecte { get; set; } = false;

        public int Ordre { get; set; }

        [StringLength(500, ErrorMessage = "L'explication ne peut pas dépasser 500 caractères")]
        public string Explication { get; set; }
    }

    /// <summary>
    /// DTO pour la réponse d'une réponse
    /// </summary>
    public class ReponseResponseDtos
    {
        public Guid ReponseId { get; set; }
        public string Texte { get; set; }
        public bool EstCorrecte { get; set; }
        public int Ordre { get; set; }
        public string Explication { get; set; }
        public Guid QuestionId { get; set; }
    }

    // DTOs pour TentativeQuiz

    /// <summary>
    /// DTO pour l'envoi d'une convocation pour un quiz
    /// </summary>
    public class ConvocationQuizDto
    {
        [Required(ErrorMessage = "L'ID de candidature est obligatoire")]
        public Guid CandidatureId { get; set; }

        [Required(ErrorMessage = "L'ID de quiz est obligatoire")]
        public Guid QuizId { get; set; }

        [Required(ErrorMessage = "L'URL de base est obligatoire")]
        public string BaseUrl { get; set; }

        [Required(ErrorMessage = "La date d'expiration est obligatoire")]
        public DateTime DateExpiration { get; set; }

        public string Message { get; set; }
    }

    /// <summary>
    /// DTO pour démarrer une tentative de quiz
    /// </summary>
    public class StartTentativeDto
    {
        [Required(ErrorMessage = "L'ID de quiz est obligatoire")]
        public Guid QuizId { get; set; }

        public Guid? CandidatureId { get; set; }

        public string Token { get; set; }
    }

    /// <summary>
    /// DTO pour soumettre une tentative de quiz
    /// </summary>
    public class SubmitTentativeDto
    {
        [Required(ErrorMessage = "Les réponses sont obligatoires")]
        public List<ReponseUtilisateurCreateDto> Reponses { get; set; }
    }

    /// <summary>
    /// DTO pour la création d'une réponse utilisateur
    /// </summary>
    public class ReponseUtilisateurCreateDto
    {
        public Guid TentativeId { get; set; } // Added this property to fix CS1061  

        [Required(ErrorMessage = "L'ID de question est obligatoire")]
        public Guid QuestionId { get; set; }

        public List<Guid> ReponseIds { get; set; }

        public string TexteReponse { get; set; }

        public int? TempsReponse { get; set; }
    }

    /// <summary>
    /// DTO pour la réponse d'une tentative de quiz
    /// </summary>
    public class TentativeQuizResponseDto
    {
        public Guid TentativeId { get; set; }
        public Guid QuizId { get; set; }
        public Guid AppUserId { get; set; }
        public Guid? CandidatureId { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public StatutTentative Statut { get; set; }
        public double Score { get; set; }
        public string QuizTitre { get; set; }
        public int QuizDuree { get; set; }
    }

    /// <summary>
    /// DTO pour la réponse détaillée d'une tentative de quiz
    /// </summary>
    public class TentativeQuizDetailDto
    {
        public Guid TentativeId { get; set; }
        public Guid QuizId { get; set; }
        public string QuizTitre { get; set; }
        public string QuizDescription { get; set; }
        public int QuizDuree { get; set; }
        public int QuizScoreMinimum { get; set; }
        public Guid AppUserId { get; set; }
        public Guid? CandidatureId { get; set; }
        public DateTime DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public StatutTentative Statut { get; set; }
        public double Score { get; set; }
        public List<ReponseUtilisateurDto> ReponsesUtilisateur { get; set; }
        public ResultatQuizDto Resultat { get; set; }
    }

    // DTOs pour ResultatQuiz

    /// <summary>
    /// DTO pour la réponse d'un résultat de quiz
    /// </summary>
    public class ResultatQuizDto
    {
        public Guid ResultatId { get; set; }
        public Guid TentativeId { get; set; }
        public double Score { get; set; }
        public int QuestionsCorrectes { get; set; }
        public int NombreQuestions { get; set; }
        public int TempsTotal { get; set; }
        public bool Reussi { get; set; }
        public string Commentaire { get; set; }
        public DateTime DateResultat { get; set; }
        public Guid QuizId { get; set; }
        public string QuizTitre { get; set; }
        public Guid AppUserId { get; set; }
        public string AppUserNom { get; set; }
        public string AppUserPrenom { get; set; }
        public string AppUserEmail { get; set; }
    }

    /// <summary>
    /// DTO pour la réponse détaillée d'un résultat de quiz
    /// </summary>
    public class ResultatQuizDetailDto
    {
        public Guid ResultatId { get; set; }
        public Guid TentativeId { get; set; }
        public double Score { get; set; }
        public int QuestionsCorrectes { get; set; }
        public int NombreQuestions { get; set; }
        public int TempsTotal { get; set; }
        public bool Reussi { get; set; }
        public string Commentaire { get; set; }
        public DateTime DateResultat { get; set; }
        public Guid QuizId { get; set; }
        public string QuizTitre { get; set; }
        public string QuizDescription { get; set; }
        public int QuizScoreMinimum { get; set; }
        public Guid AppUserId { get; set; }
        public string AppUserNom { get; set; }
        public string AppUserPrenom { get; set; }
        public string AppUserEmail { get; set; }
        public List<ReponseUtilisateurDto> ReponsesUtilisateur { get; set; }
    }

    /// <summary>
    /// DTO pour la réponse d'une réponse utilisateur
    /// </summary>
    public class ReponseUtilisateurDto
    {
        public Guid ReponseUtilisateurId { get; set; }
        public Guid QuestionId { get; set; }
        public string QuestionTexte { get; set; }
        public TypeQuestion QuestionType { get; set; }
        public int QuestionPoints { get; set; }
        public Guid? ReponseId { get; set; }
        public string ReponseTexte { get; set; }
        public string TexteReponse { get; set; }
        public int? TempsReponse { get; set; }
        public bool EstCorrecte { get; set; }
    }

    /// <summary>
    /// DTO pour la mise à jour du commentaire d'un résultat
    /// </summary>
    public class CommentaireDto
    {
        [Required(ErrorMessage = "Le commentaire est obligatoire")]
        [StringLength(1000, ErrorMessage = "Le commentaire ne peut pas dépasser 1000 caractères")]
        public string Commentaire { get; set; }
    }
    public class SoumettreQuizDto
    {
        [Required]
        public Guid TentativeId { get; set; }

        [Required]
        public List<ReponseSoumiseDto> Reponses { get; set; }
    }

    public class ReponseSoumiseDto
    {
        [Required]
        public Guid QuestionId { get; set; }

        public List<Guid>? ReponseIds { get; set; }
        public string? TexteReponse { get; set; }
        public int TempsReponse { get; set; }
    }

    public class ResultatQuizResponseDto
    {
        public Guid ResultatId { get; set; }
        public Guid TentativeId { get; set; }
        public double Score { get; set; }
        public int QuestionsCorrectes { get; set; }
        public int NombreQuestions { get; set; }
        public int TempsTotal { get; set; }
        public bool Reussi { get; set; }
        public DateTime DateResultat { get; set; }
    }
    public class ResultatDetailResponseDto
    {
        public Guid ResultatId { get; set; }
        public Guid TentativeId { get; set; }
        public Guid QuizId { get; set; }
        public string QuizTitre { get; set; }
        public double Score { get; set; }
        public int QuestionsCorrectes { get; set; }
        public int NombreQuestions { get; set; }
        public int TempsTotal { get; set; }
        public bool Reussi { get; set; }
        public DateTime DateResultat { get; set; }
        public List<ReponseDetailResponseDto> ReponsesDetail { get; set; } = new List<ReponseDetailResponseDto>();
    }

    public class ReponseDetailResponseDto
    {
        public Guid QuestionId { get; set; }
        public string QuestionTexte { get; set; }
        public TypeQuestion QuestionType { get; set; }
        public bool EstCorrecte { get; set; }
        public int? TempsReponse { get; set; }
        public string TexteReponse { get; set; }
        public Guid? ReponseId { get; set; }
        public string ReponseTexte { get; set; }
        public string Explication { get; set; }
    }
    public class UpdateFullQuizDto
    {
        public string? Titre { get; set; }
        public string? Description { get; set; }
        public int? Duree { get; set; }
        public int? ScoreMinimum { get; set; }
        public Guid? OffreEmploiId { get; set; }
        public List<UpdateQuestionDto> Questions { get; set; } = new();
    }

    public class UpdateQuestionDto
    {
        public string Texte { get; set; }
        public TypeQuestion Type { get; set; }
        public int Points { get; set; }
        public int Ordre { get; set; }
        public int? TempsRecommande { get; set; }
        public List<UpdateReponseDto> Reponses { get; set; } = new();
    }

    public class UpdateReponseDto
    {
        public string Texte { get; set; }
        public bool EstCorrecte { get; set; }
        public int Ordre { get; set; }
        public string? Explication { get; set; }
    }
   
}
