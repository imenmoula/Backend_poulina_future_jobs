//using Backend_poulina_future_jobs.DTOs;
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

//        // GET: api/TentativeQuiz/User/{userId}
//        [HttpGet("User/{userId}")]
//        [Authorize]
//        public async Task<ActionResult<IEnumerable<TentativeQuizResponseDto>>> GetTentativesByUser(Guid userId)
//        {
//            // Vérifier que l'utilisateur actuel est autorisé à voir ces tentatives
//            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            if (currentUserId != userId.ToString() && !User.IsInRole("Admin") && !User.IsInRole("Recruteur"))
//                return Forbid();

//            var tentatives = await _context.TentativesQuiz
//                .Include(t => t.Quiz)
//                .Where(t => t.AppUserId == userId)
//                .Select(t => new TentativeQuizResponseDto
//                {
//                    TentativeId = t.TentativeId,
//                    QuizId = t.QuizId,
//                    AppUserId = t.AppUserId,
//                    CandidatureId = t.CandidatureId,
//                    DateDebut = t.DateDebut,
//                    DateFin = t.DateFin,
//                    Statut = t.Statut,
//                    Score = t.Score,
//                    QuizTitre = t.Quiz.Titre,
//                    QuizDuree = t.Quiz.Duree
//                })
//                .OrderByDescending(t => t.DateDebut)
//                .ToListAsync();

//            return Ok(tentatives);
//        }

//        // GET: api/TentativeQuiz/{id}
//        [HttpGet("{id}")]
//        [Authorize]
//        public async Task<ActionResult<TentativeQuizDetailDto>> GetTentative(Guid id)
//        {
//            var tentative = await _context.TentativesQuiz
//                .Include(t => t.Quiz)
//                .Include(t => t.ReponsesUtilisateur)
//                    .ThenInclude(r => r.Question)
//                .Include(t => t.ReponsesUtilisateur)
//                    .ThenInclude(r => r.Reponse)
//                .Include(t => t.Resultat)
//                .FirstOrDefaultAsync(t => t.TentativeId == id);

//            if (tentative == null)
//                return NotFound();

//            // Vérifier que l'utilisateur actuel est autorisé à voir cette tentative
//            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            if (currentUserId != tentative.AppUserId.ToString() && !User.IsInRole("Admin") && !User.IsInRole("Recruteur"))
//                return Forbid();

//            var tentativeDto = new TentativeQuizDetailDto
//            {
//                TentativeId = tentative.TentativeId,
//                QuizId = tentative.QuizId,
//                QuizTitre = tentative.Quiz.Titre,
//                QuizDescription = tentative.Quiz.Description,
//                QuizDuree = tentative.Quiz.Duree,
//                QuizScoreMinimum = tentative.Quiz.ScoreMinimum,
//                AppUserId = tentative.AppUserId,
//                CandidatureId = tentative.CandidatureId,
//                DateDebut = tentative.DateDebut,
//                DateFin = tentative.DateFin,
//                Statut = tentative.Statut,
//                Score = tentative.Score,
//                ReponsesUtilisateur = tentative.ReponsesUtilisateur.Select(r => new ReponseUtilisateurDto
//                {
//                    ReponseUtilisateurId = r.ReponseUtilisateurId,
//                    QuestionId = r.QuestionId,
//                    QuestionTexte = r.Question.Texte,
//                    QuestionType = r.Question.Type,
//                    QuestionPoints = r.Question.Points,
//                    ReponseId = r.ReponseId,
//                    ReponseTexte = r.Reponse?.Texte,
//                    TexteReponse = r.TexteReponse,
//                    TempsReponse = r.TempsReponse,
//                    EstCorrecte = r.EstCorrecte
//                }).ToList(),
//                Resultat = tentative.Resultat != null ? new ResultatQuizDto
//                {
//                    ResultatId = tentative.Resultat.ResultatId,
//                    Score = tentative.Resultat.Score,
//                    QuestionsCorrectes = tentative.Resultat.QuestionsCorrectes,
//                    NombreQuestions = tentative.Resultat.NombreQuestions,
//                    TempsTotal = tentative.Resultat.TempsTotal,
//                    Reussi = tentative.Resultat.Reussi,
//                    Commentaire = tentative.Resultat.Commentaire,
//                    DateResultat = tentative.Resultat.DateResultat
//                } : null
//            };

//            return Ok(tentativeDto);
//        }

//        // POST: api/TentativeQuiz/Start
//        [HttpPost("Start")]
//        [Authorize]
//        public async Task<ActionResult<TentativeQuizResponseDto>> StartTentative(StartTentativeDto dto)
//        {
//            // Récupérer l'ID de l'utilisateur actuel
//            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            if (string.IsNullOrEmpty(currentUserId))
//                return Unauthorized();

//            var userId = Guid.Parse(currentUserId);

//            // Vérifier si le quiz existe
//            var quiz = await _context.Quizzes.FindAsync(dto.QuizId);
//            if (quiz == null)
//                return BadRequest("Le quiz spécifié n'existe pas.");

//            // Vérifier si le quiz est actif
//            if (!quiz.EstActif)
//                return BadRequest("Ce quiz n'est pas disponible actuellement.");

//            // Vérifier si l'utilisateur a déjà une tentative en cours pour ce quiz
//            var tentativeEnCours = await _context.TentativesQuiz
//                .AnyAsync(t => t.QuizId == dto.QuizId && t.AppUserId == userId && t.Statut == StatutTentative.EnCours);

//            if (tentativeEnCours)
//                return BadRequest("Vous avez déjà une tentative en cours pour ce quiz.");

//            // Vérifier le token si une candidature est spécifiée
//            if (dto.CandidatureId.HasValue)
//            {
//                // Logique de vérification du token (à implémenter selon votre système)
//                // ...
//            }

//            // Créer la tentative
//            var tentative = new TentativeQuiz
//            {
//                TentativeId = Guid.NewGuid(),
//                QuizId = dto.QuizId,
//                AppUserId = userId,
//                CandidatureId = dto.CandidatureId,
//                DateDebut = DateTime.UtcNow,
//                Statut = StatutTentative.EnCours
//            };

//            _context.TentativesQuiz.Add(tentative);
//            await _context.SaveChangesAsync();

//            // Construire la réponse
//            var tentativeDto = new TentativeQuizResponseDto
//            {
//                TentativeId = tentative.TentativeId,
//                QuizId = tentative.QuizId,
//                AppUserId = tentative.AppUserId,
//                CandidatureId = tentative.CandidatureId,
//                DateDebut = tentative.DateDebut,
//                Statut = tentative.Statut,
//                QuizTitre = quiz.Titre,
//                QuizDuree = quiz.Duree
//            };

//            return CreatedAtAction(nameof(GetTentative), new { id = tentative.TentativeId }, tentativeDto);
//        }

//        // POST: api/TentativeQuiz/{id}/Submit
//        [HttpPost("{id}/Submit")]
//        [Authorize]
//        public async Task<ActionResult<ResultatQuizDto>> SubmitTentative(Guid id, SubmitTentativeDto dto)
//        {
//            // Récupérer l'ID de l'utilisateur actuel
//            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            if (string.IsNullOrEmpty(currentUserId))
//                return Unauthorized();

//            var userId = Guid.Parse(currentUserId);

//            // Vérifier si la tentative existe
//            var tentative = await _context.TentativesQuiz
//                .Include(t => t.Quiz)
//                    .ThenInclude(q => q.Questions)
//                        .ThenInclude(q => q.Reponses)
//                .FirstOrDefaultAsync(t => t.TentativeId == id);

//            if (tentative == null)
//                return NotFound();

//            // Vérifier que l'utilisateur est bien le propriétaire de la tentative
//            if (tentative.AppUserId != userId)
//                return Forbid();

//            // Vérifier que la tentative est bien en cours
//            if (tentative.Statut != StatutTentative.EnCours)
//                return BadRequest("Cette tentative n'est pas en cours.");

//            // Mettre à jour la tentative
//            tentative.DateFin = DateTime.UtcNow;
//            tentative.Statut = StatutTentative.Terminee;

//            // Calculer le temps total en secondes
//            var tempsTotal = (int)(tentative.DateFin.Value - tentative.DateDebut).TotalSeconds;

//            // Traiter les réponses de l'utilisateur
//            int questionsCorrectes = 0;
//            double scoreTotal = 0;
//            var reponsesUtilisateur = new List<ReponseUtilisateur>();

//            foreach (var reponseDto in dto.Reponses)
//            {
//                // Vérifier si la question existe et appartient au quiz
//                var question = tentative.Quiz.Questions.FirstOrDefault(q => q.QuestionId == reponseDto.QuestionId);
//                if (question == null)
//                    continue;

//                bool estCorrecte = false;

//                // Traiter selon le type de question
//                if (question.Type == TypeQuestion.ReponseTexte)
//                {
//                    // Pour les questions à réponse texte, on peut implémenter une logique de vérification
//                    // Par exemple, comparer avec des mots-clés ou utiliser un service d'IA
//                    // Pour l'instant, on considère que c'est à évaluer manuellement
//                    estCorrecte = false; // À remplacer par votre logique
//                }
//                else
//                {
//                    // Pour les questions à choix
//                    if (reponseDto.ReponseIds != null && reponseDto.ReponseIds.Any())
//                    {
//                        if (question.Type == TypeQuestion.ChoixUnique || question.Type == TypeQuestion.VraiFaux)
//                        {
//                            // Une seule réponse attendue
//                            var reponseId = reponseDto.ReponseIds.First();
//                            var reponse = question.Reponses.FirstOrDefault(r => r.ReponseId == reponseId);
//                            estCorrecte = reponse != null && reponse.EstCorrecte;

//                            // Créer la réponse utilisateur
//                            var reponseUtilisateur = new ReponseUtilisateur
//                            {
//                                ReponseUtilisateurId = Guid.NewGuid(),
//                                TentativeId = tentative.TentativeId,
//                                QuestionId = question.QuestionId,
//                                ReponseId = reponseId,
//                                TempsReponse = reponseDto.TempsReponse,
//                                EstCorrecte = estCorrecte
//                            };
//                            reponsesUtilisateur.Add(reponseUtilisateur);
//                        }
//                        else if (question.Type == TypeQuestion.ChoixMultiple)
//                        {
//                            // Plusieurs réponses possibles
//                            var reponsesCorrectes = question.Reponses.Where(r => r.EstCorrecte).Select(r => r.ReponseId).ToList();
//                            var reponsesUtilisateurIds = reponseDto.ReponseIds.ToList();

//                            // Vérifier si toutes les réponses correctes sont sélectionnées et aucune incorrecte
//                            estCorrecte = reponsesCorrectes.All(reponsesUtilisateurIds.Contains) &&
//                                         reponsesUtilisateurIds.All(id => reponsesCorrectes.Contains(id));

//                            // Créer les réponses utilisateur
//                            foreach (var reponseId in reponseDto.ReponseIds)
//                            {
//                                var reponseUtilisateur = new ReponseUtilisateur
//                                {
//                                    ReponseUtilisateurId = Guid.NewGuid(),
//                                    TentativeId = tentative.TentativeId,
//                                    QuestionId = question.QuestionId,
//                                    ReponseId = reponseId,
//                                    TempsReponse = reponseDto.TempsReponse,
//                                    EstCorrecte = estCorrecte
//                                };
//                                reponsesUtilisateur.Add(reponseUtilisateur);
//                            }
//                        }
//                    }
//                }

//                // Mettre à jour le score
//                if (estCorrecte)
//                {
//                    questionsCorrectes++;
//                    scoreTotal += question.Points;
//                }
//            }

//            // Calculer le score final en pourcentage
//            var scoreMaxPossible = tentative.Quiz.Questions.Sum(q => q.Points);
//            var scorePourcentage = scoreMaxPossible > 0 ? (scoreTotal / scoreMaxPossible) * 100 : 0;
//            tentative.Score = scorePourcentage;

//            // Déterminer si le quiz est réussi
//            var reussi = scorePourcentage >= tentative.Quiz.ScoreMinimum;

//            // Créer le résultat
//            var resultat = new ResultatQuiz
//            {
//                ResultatId = Guid.NewGuid(),
//                TentativeId = tentative.TentativeId,
//                Score = scorePourcentage,
//                QuestionsCorrectes = questionsCorrectes,
//                NombreQuestions = tentative.Quiz.Questions.Count,
//                TempsTotal = tempsTotal,
//                Reussi = reussi,
//                DateResultat = DateTime.UtcNow,
//                QuizId = tentative.QuizId,
//                AppUserId = tentative.AppUserId
//            };

//            // Enregistrer les données
//            _context.ReponsesUtilisateur.AddRange(reponsesUtilisateur);
//            _context.ResultatsQuiz.Add(resultat);
//            await _context.SaveChangesAsync();

//            // Construire la réponse
//            var resultatDto = new ResultatQuizDto
//            {
//                ResultatId = resultat.ResultatId,
//                Score = resultat.Score,
//                QuestionsCorrectes = resultat.QuestionsCorrectes,
//                NombreQuestions = resultat.NombreQuestions,
//                TempsTotal = resultat.TempsTotal,
//                Reussi = resultat.Reussi,
//                DateResultat = resultat.DateResultat
//            };

//            return Ok(resultatDto);
//        }

//        // PUT: api/TentativeQuiz/{id}/Abandon
//        [HttpPut("{id}/Abandon")]
//        [Authorize]
//        public async Task<IActionResult> AbandonTentative(Guid id)
//        {
//            // Récupérer l'ID de l'utilisateur actuel
//            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            if (string.IsNullOrEmpty(currentUserId))
//                return Unauthorized();

//            var userId = Guid.Parse(currentUserId);

//            // Vérifier si la tentative existe
//            var tentative = await _context.TentativesQuiz.FindAsync(id);
//            if (tentative == null)
//                return NotFound();

//            // Vérifier que l'utilisateur est bien le propriétaire de la tentative
//            if (tentative.AppUserId != userId)
//                return Forbid();

//            // Vérifier que la tentative est bien en cours
//            if (tentative.Statut != StatutTentative.EnCours)
//                return BadRequest("Cette tentative n'est pas en cours.");

//            // Mettre à jour la tentative
//            tentative.DateFin = DateTime.UtcNow;
//            tentative.Statut = StatutTentative.Abandonnee;

//            await _context.SaveChangesAsync();
//            return NoContent();
//        }
//    }
//}