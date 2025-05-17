//using Backend_poulina_future_jobs.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Security.Claims;
//using System.Threading.Tasks;

//namespace Backend_poulina_future_jobs.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class TentativeQuizController : ControllerBase
//    {
//        private readonly AppDbContext _context;
//        private readonly ILogger<TentativeQuizController> _logger;

//        public TentativeQuizController(AppDbContext context, ILogger<TentativeQuizController> logger)
//        {
//            _context = context;
//            _logger = logger;
//        }

//        // POST: api/TentativeQuiz
//        [Authorize]
//        [HttpPost]
//        [ProducesResponseType(typeof(TentativeQuizDto), 201)]
//        [ProducesResponseType(400)]
//        [ProducesResponseType(401)]
//        public async Task<ActionResult<TentativeQuizDto>> CreateTentative([FromBody] CreateTentativeQuizDto dto)
//        {
//            _logger.LogInformation("Creating new tentative for quiz {QuizId}", dto.QuizId);

//            var userIdString = User.FindFirst("userId")?.Value;
//            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
//            {
//                _logger.LogWarning("User ID not found in token");
//                return Unauthorized("ID utilisateur non trouvé dans le token");
//            }

//            var quiz = await _context.Quizzes.FindAsync(dto.QuizId);
//            if (quiz == null || !quiz.EstActif)
//            {
//                _logger.LogWarning("Quiz {QuizId} not found or inactive", dto.QuizId);
//                return BadRequest("Quiz non trouvé ou inactif");
//            }

//            var tentative = new TentativeQuiz
//            {
//                TentativeId = Guid.NewGuid(),
//                QuizId = dto.QuizId,
//                AppUserId = userId,
//                Statut = (Models.StatutTentative)StatutTentative.EnCours,
//                DateDebut = DateTime.UtcNow,
//                DateFin = null,
//                Score = 0
//            };

//            _context.TentativesQuiz.Add(tentative);
//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Failed to create tentative for quiz {QuizId}", dto.QuizId);
//                return StatusCode(500, "Erreur lors de la création de la tentative.");
//            }

//            var tentativeDto = new TentativeQuizDto
//            {
//                TentativeId = tentative.TentativeId,
//                QuizId = tentative.QuizId,
//                AppUserId = tentative.AppUserId.ToString(),
//                Statut = tentative.Statut.ToString(),
//                DateDebut = tentative.DateDebut.ToString("o"),
//                DateFin = tentative.DateFin.HasValue ? tentative.DateFin.Value.ToString("o") : null,
//                Score = (int?)tentative.Score
//            };

//            _logger.LogInformation("Tentative {TentativeId} created successfully", tentative.TentativeId);
//            return CreatedAtAction(nameof(GetTentative), new { id = tentative.TentativeId }, tentativeDto);
//        }

//        // GET: api/TentativeQuiz/{id}
//        [Authorize]
//        [HttpGet("{id}")]
//        [ProducesResponseType(typeof(TentativeQuizDto), 200)]
//        [ProducesResponseType(401)]
//        [ProducesResponseType(404)]
//        public async Task<ActionResult<TentativeQuizDto>> GetTentative(Guid id)
//        {
//            _logger.LogInformation("Retrieving tentative {TentativeId}", id);

//            var userIdString = User.FindFirst("userId")?.Value;
//            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
//            {
//                _logger.LogWarning("User ID not found in token for tentative {TentativeId}", id);
//                return Unauthorized("ID utilisateur non trouvé dans le token");
//            }

//            var tentative = await _context.TentativesQuiz
//                .FirstOrDefaultAsync(t => t.TentativeId == id && t.AppUserId == userId);

//            if (tentative == null)
//            {
//                _logger.LogWarning("Tentative {TentativeId} not found for user {UserId}", id, userId);
//                return NotFound("Tentative non trouvée ou accès non autorisé");
//            }

//            var tentativeDto = new TentativeQuizDto
//            {
//                TentativeId = tentative.TentativeId,
//                QuizId = tentative.QuizId,
//                AppUserId = tentative.AppUserId.ToString(),
//                Statut = tentative.Statut.ToString(),
//                DateDebut = tentative.DateDebut.ToString("o"),
//                DateFin = tentative.DateFin.HasValue ? tentative.DateFin.Value.ToString("o") : null,
//                Score = (int?)tentative.Score
//            };

//            return Ok(tentativeDto);
//        }

//        // GET: api/TentativeQuiz/{id}/Resultat
//        [Authorize]
//        [HttpGet("{id}/Resultat")]
//        public async Task<ActionResult<ResultatQuizDto>> GetResultat(Guid id)
//        {
//            _logger.LogInformation("Retrieving result for tentative {TentativeId}", id);

//            var userIdString = User.FindFirst("userId")?.Value;
//            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
//            {
//                _logger.LogWarning("User ID not found in token for tentative {TentativeId}", id);
//                return Unauthorized("ID utilisateur non trouvé dans le token");
//            }

//            var tentative = await _context.TentativesQuiz
//                .Include(t => t.Quiz)
//                .FirstOrDefaultAsync(t => t.TentativeId == id && t.AppUserId == userId);

//            if (tentative == null)
//            {
//                _logger.LogWarning("Tentative {TentativeId} not found for user {UserId}", id, userId);
//                return NotFound("Tentative non trouvée");
//            }

//            if (tentative.Statut == StatutTentative.Terminee)
//            {
//                _logger.LogWarning("Tentative {TentativeId} is not completed", id);
//                return BadRequest("La tentative n'est pas terminée");
//            }

//            var resultat = await _context.ResultatsQuiz
//                .FirstOrDefaultAsync(r => r.TentativeId == id);

//            if (resultat == null)
//            {
//                _logger.LogWarning("Result not found for tentative {TentativeId}", id);
//                return NotFound("Résultat non trouvé");
//            }

//            return Ok(new ResultatQuizDto
//            {
//                ResultatId = resultat.ResultatId,
//                TentativeId = resultat.TentativeId,
//                AppUserId = tentative.AppUserId.ToString(),
//                QuizId = tentative.QuizId,
//                Score = (int)resultat.Score,
//                DateResultat = resultat.DateResultat.ToString("o"),
//                AReussi = resultat.Score >= tentative.Quiz.ScoreMinimum
//            });
//        }

//        // POST: api/TentativeQuiz/{id}/Soumettre
//        [Authorize]
//        [HttpPost("{id}/Soumettre")]
//        public async Task<ActionResult<ResultatQuizDto>> SoumettreTentative(Guid id, SoumettreTentativeDto dto)
//        {
//            _logger.LogInformation("Submitting tentative {TentativeId}", id);

//            var userIdString = User.FindFirst("userId")?.Value;
//            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
//            {
//                _logger.LogWarning("User ID not found in token for tentative {TentativeId}", id);
//                return Unauthorized("ID utilisateur non trouvé dans le token");
//            }

//            var tentative = await _context.TentativesQuiz
//                .Include(t => t.Quiz)
//                .ThenInclude(q => q.Questions)
//                .ThenInclude(q => q.Reponses)
//                .FirstOrDefaultAsync(t => t.TentativeId == id && t.AppUserId == userId);

//            if (tentative == null)
//            {
//                _logger.LogWarning("Tentative {TentativeId} not found for user {UserId}", id, userId);
//                return NotFound("Tentative non trouvée");
//            }

//            if (tentative.Statut != StatutTentative.EnCours)
//            {
//                _logger.LogWarning("Tentative {TentativeId} is not in progress", id);
//                return BadRequest("La tentative n'est pas en cours");
//            }

//            var quiz = tentative.Quiz;
//            if (quiz == null || !quiz.EstActif)
//            {
//                _logger.LogWarning("Quiz for tentative {TentativeId} is not found or inactive", id);
//                return BadRequest("Quiz non trouvé ou inactif");
//            }

//            if (dto.Reponses == null || !dto.Reponses.Any())
//            {
//                _logger.LogWarning("No responses provided for tentative {TentativeId}", id);
//                return BadRequest("Aucune réponse fournie");
//            }

//            var questions = quiz.Questions.ToList();
//            if (questions.Count == 0)
//            {
//                _logger.LogWarning("Quiz {QuizId} has no questions for tentative {TentativeId}", quiz.QuizId, id);
//                return BadRequest("Le quiz ne contient aucune question");
//            }

//            int score = 0;
//            int questionsCorrectes = 0;
//            int nombreQuestions = questions.Count;
//            int tempsTotal = 0;
//            var resultatDetails = new List<ResultatDetail>();

//            foreach (var reponse in dto.Reponses)
//            {
//                var question = questions.FirstOrDefault(q => q.QuestionId == reponse.QuestionId);
//                if (question == null)
//                {
//                    _logger.LogWarning("Question {QuestionId} not found for tentative {TentativeId}", reponse.QuestionId, id);
//                    return BadRequest($"Question {reponse.QuestionId} non trouvée");
//                }

//                bool estCorrecte = false;

//                if (question.Type == TypeQuestion.ReponseTexte)
//                {
//                    if (string.IsNullOrEmpty(reponse.TexteReponse))
//                    {
//                        _logger.LogWarning("Text response missing for ReponseTexte question {QuestionId}", question.QuestionId);
//                        return BadRequest($"Réponse texte manquante pour la question {question.QuestionId}");
//                    }
//                    // Pour ReponseTexte, on stocke la réponse pour une évaluation manuelle
//                    estCorrecte = false; // Points attribués manuellement plus tard
//                }
//                else
//                {
//                    if (!reponse.ReponseId.HasValue)
//                    {
//                        _logger.LogWarning("ReponseId missing for non-ReponseTexte question {QuestionId}", question.QuestionId);
//                        return BadRequest($"Réponse ID manquante pour la question {question.QuestionId}");
//                    }

//                    var selectedReponse = question.Reponses.FirstOrDefault(r => r.ReponseId == reponse.ReponseId);
//                    if (selectedReponse == null)
//                    {
//                        _logger.LogWarning("Reponse {ReponseId} not found for question {QuestionId}", reponse.ReponseId, question.QuestionId);
//                        return BadRequest($"Réponse {reponse.ReponseId} non trouvée");
//                    }

//                    estCorrecte = selectedReponse.EstCorrecte;

//                    if (question.Type == TypeQuestion.ChoixMultiple)
//                    {
//                        var correctReponses = question.Reponses.Where(r => r.EstCorrecte).Select(r => r.ReponseId).ToList();
//                        var selectedReponses = dto.Reponses
//                            .Where(r => r.QuestionId == question.QuestionId && r.ReponseId.HasValue)
//                            .Select(r => r.ReponseId.Value)
//                            .ToList();

//                        if (selectedReponses.Count == 0)
//                        {
//                            _logger.LogWarning("No responses provided for ChoixMultiple question {QuestionId}", question.QuestionId);
//                            estCorrecte = false;
//                        }
//                        else
//                        {
//                            estCorrecte = correctReponses.All(cr => selectedReponses.Contains(cr)) &&
//                                         selectedReponses.All(sr => correctReponses.Contains(sr));
//                        }
//                    }
//                    else if (question.Type == TypeQuestion.VraiFaux || question.Type == TypeQuestion.ChoixUnique)
//                    {
//                        // Vérifier qu'une seule réponse a été soumise
//                        var responsesForQuestion = dto.Reponses.Count(r => r.QuestionId == question.QuestionId);
//                        if (responsesForQuestion != 1)
//                        {
//                            _logger.LogWarning("Invalid number of responses ({Count}) for question {QuestionId} of type {Type}", responsesForQuestion, question.QuestionId, question.Type);
//                            return BadRequest($"La question {question.QuestionId} de type {question.Type} doit avoir exactement une réponse.");
//                        }
//                    }
//                }

//                if (estCorrecte)
//                {
//                    score += question.Points;
//                    questionsCorrectes++;
//                }

//                tempsTotal += reponse.TempsReponse;

//                resultatDetails.Add(new ResultatDetail
//                {
//                    ResultatDetailId = Guid.NewGuid(),
//                    QuestionId = question.QuestionId,
//                    ReponseId = reponse.ReponseId,
//                    TexteReponse = reponse.TexteReponse,
//                    EstCorrecte = estCorrecte,
//                    PointsObtenus = estCorrecte ? question.Points : 0,
//                    TempsReponse = reponse.TempsReponse
//                });
//            }

//            tentative.Statut = (Models.StatutTentative)StatutTentative.Terminee;
//            tentative.DateFin = DateTime.UtcNow;
//            tentative.Score = score;

//            var resultat = new ResultatQuiz
//            {
//                ResultatId = Guid.NewGuid(),
//                TentativeId = tentative.TentativeId,
//                AppUserId = userId,
//                QuizId = quiz.QuizId,
//                Score = score,
//                QuestionsCorrectes = questionsCorrectes,
//                NombreQuestions = nombreQuestions,
//                TempsTotal = tempsTotal,
//                Reussi = score >= quiz.ScoreMinimum,
//                Commentaire = null,
//                DateResultat = DateTime.UtcNow,
//                ResultatDetails = resultatDetails
//            };

//            _context.ResultatsQuiz.Add(resultat);
//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Failed to submit tentative {TentativeId}", id);
//                return StatusCode(500, "Erreur lors de la soumission de la tentative.");
//            }

//            _logger.LogInformation("Tentative {TentativeId} submitted successfully with score {Score}", id, score);

//            return Ok(new ResultatQuizDto
//            {
//                ResultatId = resultat.ResultatId,
//                TentativeId = resultat.TentativeId,
//                AppUserId = resultat.AppUserId.ToString(),
//                QuizId = resultat.QuizId,
//                Score = resultat.Score,
//                DateResultat = resultat.DateResultat.ToString("o"),
//                AReussi = resultat.Reussi,
//                QuestionsCorrectes = resultat.QuestionsCorrectes,
//                NombreQuestions = resultat.NombreQuestions,
//                TempsTotal = resultat.TempsTotal
//            });
//        }

//        // POST: api/TentativeQuiz/{id}/Abandonner
//        [Authorize]
//        [HttpPost("{id}/Abandonner")]
//        [ProducesResponseType(204)]
//        [ProducesResponseType(400)]
//        [ProducesResponseType(401)]
//        [ProducesResponseType(404)]
//        public async Task<IActionResult> AbandonnerTentative(Guid id)
//        {
//            _logger.LogInformation("Abandoning tentative {TentativeId}", id);

//            var userIdString = User.FindFirst("userId")?.Value;
//            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
//            {
//                _logger.LogWarning("User ID not found in token for tentative {TentativeId}", id);
//                return Unauthorized("ID utilisateur non trouvé dans le token");
//            }

//            var tentative = await _context.TentativesQuiz
//                .FirstOrDefaultAsync(t => t.TentativeId == id && t.AppUserId == userId);

//            if (tentative == null)
//            {
//                _logger.LogWarning("Tentative {TentativeId} not found or unauthorized", id);
//                return NotFound("Tentative non trouvée ou accès non autorisé");
//            }

//            if (tentative.Statut != StatutTentative.EnCours)
//            {
//                _logger.LogWarning("Tentative {TentativeId} is not in progress", id);
//                return BadRequest("La tentative n'est plus en cours");
//            }

//            tentative.Statut = (Models.StatutTentative)StatutTentative.Abandonnee;
//            tentative.DateFin = DateTime.UtcNow;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Failed to abandon tentative {TentativeId}", id);
//                return StatusCode(500, "Erreur lors de l'abandon de la tentative.");
//            }

//            _logger.LogInformation("Tentative {TentativeId} abandoned successfully", id);
//            return NoContent();
//        }

//        #region DTOs and Models

//        public enum StatutTentative
//        {
//            EnCours,
//            Terminee,
//            Abandonnee
//        }

//        public class TentativeQuizDto
//        {
//            public Guid TentativeId { get; set; }
//            public Guid QuizId { get; set; }
//            public string AppUserId { get; set; }
//            public string Statut { get; set; }
//            public string DateDebut { get; set; }
//            public string DateFin { get; set; }
//            public int? Score { get; set; }
//        }

//        public class CreateTentativeQuizDto
//        {
//            [Required]
//            public Guid QuizId { get; set; }
//        }

//        public class SoumettreTentativeDto
//        {
//            [Required]
//            public List<ReponseSoumiseDto> Reponses { get; set; }
//        }

//        public class ReponseSoumiseDto
//        {
//            [Required]
//            public Guid QuestionId { get; set; }
//            public Guid? ReponseId { get; set; }
//            public string TexteReponse { get; set; }
//            public int TempsReponse { get; set; }
//        }

//        public class ResultatDetail
//        {
//            public Guid ResultatDetailId { get; set; }
//            public Guid QuestionId { get; set; }
//            public Guid? ReponseId { get; set; }
//            public string TexteReponse { get; set; }
//            public bool EstCorrecte { get; set; }
//            public int PointsObtenus { get; set; }
//            public int TempsReponse { get; set; }
//        }

//        public class ResultatQuiz
//        {
//            public Guid ResultatId { get; set; }
//            public Guid TentativeId { get; set; }
//            public Guid AppUserId { get; set; }
//            public Guid QuizId { get; set; }
//            public int Score { get; set; }
//            public int QuestionsCorrectes { get; set; }
//            public int NombreQuestions { get; set; }
//            public int TempsTotal { get; set; }
//            public bool Reussi { get; set; }
//            public string Commentaire { get; set; }
//            public DateTime DateResultat { get; set; }
//            public List<ResultatDetail> ResultatDetails { get; set; }
//            public virtual TentativeQuiz Tentative { get; set; }
//        }

//        public class ResultatQuizDto
//        {
//            public Guid ResultatId { get; set; }
//            public Guid TentativeId { get; set; }
//            public string AppUserId { get; set; }
//            public Guid QuizId { get; set; }
//            public int Score { get; set; }
//            public string DateResultat { get; set; }
//            public bool AReussi { get; set; }
//            public int QuestionsCorrectes { get; set; }
//            public int NombreQuestions { get; set; }
//            public int TempsTotal { get; set; }
//        }

//        #endregion
//    }
//}