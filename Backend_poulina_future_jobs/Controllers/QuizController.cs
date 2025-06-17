


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_poulina_future_jobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Backend_poulina_future_jobs.DTOs;
using Backend_poulina_future_jobs.Services;
using System.Security.Claims;
using static Backend_poulina_future_jobs.Controllers.TentativeQuizController;

namespace Backend_poulina_future_jobs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<QuizController> _logger;
        private readonly IEmailService _emailService;

        public QuizController(AppDbContext context, ILogger<QuizController> logger, IEmailService emailService)
        {
            _context = context;
            _logger = logger;
            _emailService = emailService;
        }

        // GET: api/Quiz
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<QuizResponseDto>>> GetQuizzes()
        {
            var quizzes = await _context.Quizzes
                .Select(q => new QuizResponseDto
                {
                    QuizId = q.QuizId,
                    Titre = q.Titre,
                    Description = q.Description,
                    DateCreation = q.DateCreation,
                    EstActif = q.EstActif,
                    Duree = q.Duree,
                    ScoreMinimum = q.ScoreMinimum,
                    OffreEmploiId = q.OffreEmploiId
                })
                .ToListAsync();
            return Ok(quizzes);
        }

        // GET: api/Quiz/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<QuizResponseDto>> GetQuiz(Guid id)
        {
            var quiz = await _context.Quizzes
                .Select(q => new QuizResponseDto
                {
                    QuizId = q.QuizId,
                    Titre = q.Titre,
                    Description = q.Description,
                    DateCreation = q.DateCreation,
                    EstActif = q.EstActif,
                    Duree = q.Duree,
                    ScoreMinimum = q.ScoreMinimum,
                    OffreEmploiId = q.OffreEmploiId
                })
                .FirstOrDefaultAsync(q => q.QuizId == id);

            if (quiz == null)
                return NotFound();

            return Ok(quiz);
        }

        // GET: api/Quiz/Full/{id}
        [HttpGet("Full/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<QuizFullResponseDto>> GetFullQuiz(Guid id)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Reponses)
                .FirstOrDefaultAsync(q => q.QuizId == id);

            if (quiz == null)
                return NotFound();

            var quizFullDto = new QuizFullResponseDto
            {
                QuizId = quiz.QuizId,
                Titre = quiz.Titre,
                Description = quiz.Description,
                DateCreation = quiz.DateCreation,
                EstActif = quiz.EstActif,
                Duree = quiz.Duree,
                ScoreMinimum = quiz.ScoreMinimum,
                OffreEmploiId = quiz.OffreEmploiId,
                Questions = quiz.Questions.Select(q => new QuestionsResponseDtos
                {
                    QuestionId = q.QuestionId,
                    Texte = q.Texte,
                    Type = q.Type,
                    Points = q.Points,
                    Ordre = q.Ordre,
                    TempsRecommande = q.TempsRecommande,
                    QuizId = q.QuizId,
                    Reponses = q.Reponses.Select(r => new ReponseResponseDtos
                    {
                        ReponseId = r.ReponseId,
                        Texte = r.Texte,
                        EstCorrecte = r.EstCorrecte,
                        Ordre = r.Ordre,
                        Explication = r.Explication,
                        QuestionId = r.QuestionId
                    }).ToList()
                }).ToList()
            };

            return Ok(quizFullDto);
        }

        // GET: api/Quiz/Offre/{offreId}
        [HttpGet("Offre/{offreId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<QuizResponseDto>>> GetQuizzesByOffre(Guid offreId)
        {
            var quizzes = await _context.Quizzes
                .Where(q => q.OffreEmploiId == offreId)
                .Select(q => new QuizResponseDto
                {
                    QuizId = q.QuizId,
                    Titre = q.Titre,
                    Description = q.Description,
                    DateCreation = q.DateCreation,
                    EstActif = q.EstActif,
                    Duree = q.Duree,
                    ScoreMinimum = q.ScoreMinimum,
                    OffreEmploiId = q.OffreEmploiId
                })
                .ToListAsync();
            return Ok(quizzes);
        }

        // GET: api/Quiz/Actifs
        [HttpGet("Actifs")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<QuizResponseDto>>> GetQuizzesActifs()
        {
            var quizzes = await _context.Quizzes
                .Where(q => q.EstActif)
                .Select(q => new QuizResponseDto
                {
                    QuizId = q.QuizId,
                    Titre = q.Titre,
                    Description = q.Description,
                    DateCreation = q.DateCreation,
                    EstActif = q.EstActif,
                    Duree = q.Duree,
                    ScoreMinimum = q.ScoreMinimum,
                    OffreEmploiId = q.OffreEmploiId
                })
                .ToListAsync();
            return Ok(quizzes);
        }

        // POST: api/Quiz
        [HttpPost]
        [Authorize(Roles = "Admin,Recruteur")]
        public async Task<ActionResult<QuizResponseDto>> CreateQuiz(QuizCreateDto dto)
        {
            var quiz = new Quiz
            {
                QuizId = Guid.NewGuid(),
                Titre = dto.Titre,
                Description = dto.Description,
                Duree = dto.Duree,
                ScoreMinimum = dto.ScoreMinimum,
                OffreEmploiId = dto.OffreEmploiId,
                DateCreation = DateTime.UtcNow,
                EstActif = true
            };

            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            var quizDto = new QuizResponseDto
            {
                QuizId = quiz.QuizId,
                Titre = quiz.Titre,
                Description = quiz.Description,
                DateCreation = quiz.DateCreation,
                EstActif = quiz.EstActif,
                Duree = quiz.Duree,
                ScoreMinimum = quiz.ScoreMinimum,
                OffreEmploiId = quiz.OffreEmploiId
            };

            return CreatedAtAction(nameof(GetQuiz), new { id = quiz.QuizId }, quizDto);
        }

        // POST: api/Quiz/Full
        [HttpPost("Full")]
        [Authorize(Roles = "Admin,Recruteur")]
        public async Task<ActionResult<QuizFullResponseDto>> CreateFullQuiz(CreateFullQuizDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _logger.LogInformation("Creating full quiz with title {Titre} by user {UserId}", dto.Titre, userId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.Questions == null || !dto.Questions.Any())
                return BadRequest("Un quiz doit contenir au moins une question.");

            foreach (var question in dto.Questions)
            {
                if (question.Type == TypeQuestion.ReponseTexte)
                {
                    if (question.Reponses != null && question.Reponses.Any())
                        return BadRequest($"La question {question.Texte} de type ReponseTexte ne peut pas avoir de réponses prédéfinies.");
                }
                else if (question.Type == TypeQuestion.ChoixUnique || question.Type == TypeQuestion.ChoixMultiple || question.Type == TypeQuestion.VraiFaux)
                {
                    if (question.Reponses == null || !question.Reponses.Any())
                        return BadRequest($"La question {question.Texte} doit avoir au moins une réponse.");
                    if (!question.Reponses.Any(r => r.EstCorrecte))
                        return BadRequest($"La question {question.Texte} doit avoir au moins une réponse correcte.");
                    if (question.Type == TypeQuestion.VraiFaux && question.Reponses.Count != 2)
                        return BadRequest($"La question {question.Texte} de type VraiFaux doit avoir exactement deux réponses.");
                }
            }

            var quiz = new Quiz
            {
                QuizId = Guid.NewGuid(),
                Titre = dto.Titre,
                Description = dto.Description,
                Duree = dto.Duree,
                ScoreMinimum = dto.ScoreMinimum,
                OffreEmploiId = dto.OffreEmploiId,
                EstActif = true,
                DateCreation = DateTime.UtcNow
            };

            _context.Quizzes.Add(quiz);

            var questions = new List<Question>();
            var reponses = new List<Reponse>();

            foreach (var questionDto in dto.Questions)
            {
                var question = new Question
                {
                    QuestionId = Guid.NewGuid(),
                    QuizId = quiz.QuizId,
                    Texte = questionDto.Texte,
                    Type = questionDto.Type,
                    Points = questionDto.Points,
                    Ordre = questionDto.Ordre,
                    TempsRecommande = questionDto.TempsRecommande
                };
                questions.Add(question);

                if (questionDto.Reponses != null)
                {
                    foreach (var reponseDto in questionDto.Reponses)
                    {
                        var reponse = new Reponse
                        {
                            ReponseId = Guid.NewGuid(),
                            QuestionId = question.QuestionId,
                            Texte = reponseDto.Texte,
                            EstCorrecte = reponseDto.EstCorrecte,
                            Ordre = reponseDto.Ordre,
                            Explication = reponseDto.Explication
                        };
                        reponses.Add(reponse);
                    }
                }
            }

            _context.Questions.AddRange(questions);
            _context.Reponses.AddRange(reponses);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save full quiz {QuizId}", quiz.QuizId);
                return StatusCode(500, "Erreur lors de la création du quiz.");
            }

            var quizFullDto = new QuizFullResponseDto
            {
                QuizId = quiz.QuizId,
                Titre = quiz.Titre,
                Description = quiz.Description,
                DateCreation = quiz.DateCreation,
                EstActif = quiz.EstActif,
                Duree = quiz.Duree,
                ScoreMinimum = quiz.ScoreMinimum,
                OffreEmploiId = quiz.OffreEmploiId,
                Questions = questions.Select(q => new QuestionsResponseDtos
                {
                    QuestionId = q.QuestionId,
                    Texte = q.Texte,
                    Type = q.Type,
                    Points = q.Points,
                    Ordre = q.Ordre,
                    TempsRecommande = q.TempsRecommande,
                    QuizId = q.QuizId,
                    Reponses = reponses
                        .Where(r => r.QuestionId == q.QuestionId)
                        .Select(r => new ReponseResponseDtos
                        {
                            ReponseId = r.ReponseId,
                            Texte = r.Texte,
                            EstCorrecte = r.EstCorrecte,
                            Ordre = r.Ordre,
                            Explication = r.Explication,
                            QuestionId = r.QuestionId
                        })
                        .ToList()
                }).ToList()
            };

            return CreatedAtAction(nameof(GetQuiz), new { id = quiz.QuizId }, quizFullDto);
        }

        // UPDATE FULL
        // PUT: api/Quiz/Full/{id}
        [HttpPut("Full/{id}")]
        [Authorize(Roles = "Admin,Recruteur")]
        public async Task<IActionResult> UpdateFullQuiz(Guid id, UpdateFullQuizDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _logger.LogInformation("Updating full quiz {Id} by user {UserId}", id, userId);

            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Reponses)
                .FirstOrDefaultAsync(q => q.QuizId == id);

            if (quiz == null)
                return NotFound();

            if (!string.IsNullOrEmpty(dto.Titre))
                quiz.Titre = dto.Titre;
            if (!string.IsNullOrEmpty(dto.Description))
                quiz.Description = dto.Description;
            if (dto.Duree.HasValue)
                quiz.Duree = dto.Duree.Value;
            if (dto.ScoreMinimum.HasValue)
                quiz.ScoreMinimum = dto.ScoreMinimum.Value;
            if (dto.OffreEmploiId.HasValue)
                quiz.OffreEmploiId = dto.OffreEmploiId;

            var oldQuestions = quiz.Questions.ToList();
            foreach (var oldQ in oldQuestions)
            {
                _context.Reponses.RemoveRange(oldQ.Reponses);
            }
            _context.Questions.RemoveRange(oldQuestions);

            var newQuestions = new List<Question>();
            var newReponses = new List<Reponse>();
            foreach (var questionDto in dto.Questions)
            {
                var question = new Question
                {
                    QuestionId = Guid.NewGuid(),
                    QuizId = quiz.QuizId,
                    Texte = questionDto.Texte,
                    Type = questionDto.Type,
                    Points = questionDto.Points,
                    Ordre = questionDto.Ordre,
                    TempsRecommande = questionDto.TempsRecommande
                };
                newQuestions.Add(question);

                if (questionDto.Reponses != null)
                {
                    foreach (var reponseDto in questionDto.Reponses)
                    {
                        var reponse = new Reponse
                        {
                            ReponseId = Guid.NewGuid(),
                            QuestionId = question.QuestionId,
                            Texte = reponseDto.Texte,
                            EstCorrecte = reponseDto.EstCorrecte,
                            Ordre = reponseDto.Ordre,
                            Explication = reponseDto.Explication
                        };
                        newReponses.Add(reponse);
                    }
                }
            }
            _context.Questions.AddRange(newQuestions);
            _context.Reponses.AddRange(newReponses);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("EnvoyerConvocation")]
        [Authorize(Roles = "Admin,Recruteur")]
        public async Task<IActionResult> EnvoyerConvocation([FromBody] ConvocationQuizDto dto)
        {
            try
            {
                var candidature = await _context.Candidatures
                    .Include(c => c.AppUser)
                    .Include(c => c.Offre)
                    .FirstOrDefaultAsync(c => c.IdCandidature == dto.CandidatureId);

                if (candidature == null)
                    return NotFound("Candidature non trouvée");

                if (candidature.Statut == "Refusé")
                {
                    var subject = "Retour sur votre candidature";
                    var body = $@"
                <html>
                <body>
                    <p>Bonjour {candidature.AppUser.Prenom} {candidature.AppUser.Nom},</p>
                    <p>Nous sommes au regret de vous informer que votre candidature pour le poste {candidature.Offre?.Specialite} n'a pas été retenue.</p>
                    <p>Merci pour l'intérêt que vous avez porté à notre entreprise.</p>
                </body>
                </html>";
                    await _emailService.EnvoyerEmailAsync(candidature.AppUser.Email, subject, body, true);
                    return BadRequest("Candidat refusé. Email de feedback envoyé.");
                }

                if (candidature.Statut != "Acceptée")
                    return BadRequest("Seuls les candidats acceptés peuvent recevoir une convocation");

                var quiz = await _context.Quizzes.FindAsync(dto.QuizId);
                if (quiz == null)
                    return NotFound("Quiz non trouvé");

                var token = Guid.NewGuid().ToString();

                string baseUrl = dto.BaseUrl ?? "https://votresite.com";

                var tentative = new TentativeQuiz
                {
                    TentativeId = Guid.NewGuid(),
                    QuizId = quiz.QuizId,
                    AppUserId = candidature.AppUserId,
                    CandidatureId = candidature.IdCandidature,
                    DateDebut = null,
                    DateFin = DateTime.UtcNow.AddDays(7),
                    Score = null,
                    Statut = StatutTentative.Expiree,
                    Token = token
                };

                _context.TentativesQuiz.Add(tentative);
                await _context.SaveChangesAsync();

                var lienQuiz = $"{baseUrl}/quiz/start/{tentative.TentativeId}?token={token}";

                var subjectQuiz = $"Convocation pour passer le quiz: {quiz.Titre}";
                var bodyQuiz = $@"
            <html>
            <body>
                <h2>Bonjour {candidature.AppUser.Prenom} {candidature.AppUser.Nom},</h2>
                <p>Vous êtes convié(e) à passer un quiz dans le cadre de votre candidature pour le poste {candidature.Offre?.Specialite}.</p>
                <p><strong>Détails du quiz :</strong></p>
                <ul>
                    <li><strong>Titre:</strong> {quiz.Titre}</li>
                    <li><strong>Durée:</strong> {quiz.Duree} minutes</li>
                    <li><strong>Score minimum requis:</strong> {quiz.ScoreMinimum}%</li>
                </ul>
                <p>Pour commencer le quiz, cliquez sur le lien ci-dessous :</p>
                <a href='{lienQuiz}'>Passer le quiz</a>
                <p>Ce lien est valable pour une durée de 7 jours.</p>
                <p>Bonne chance!</p>
            </body>
            </html>";

                var emailEnvoye = await _emailService.EnvoyerEmailAsync(candidature.AppUser.Email, subjectQuiz, bodyQuiz, true);

                if (!emailEnvoye)
                    return StatusCode(500, "Erreur lors de l'envoi de l'email");

                candidature.Statut = "Convoqué au quiz";
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Convocation envoyée avec succès",
                    tentativeId = tentative.TentativeId,
                    candidat = $"{candidature.AppUser.Prenom} {candidature.AppUser.Nom}",
                    email = candidature.AppUser.Email
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi de la convocation");
                return StatusCode(500, "Erreur interne du serveur");
            }
        }

        // POST: api/Quiz/Soumettre
        [HttpPost("Soumettre")]
        [Authorize]
        public async Task<ActionResult<ResultatQuizResponseDto>> SoumettreQuiz([FromBody] SoumettreQuizDto dto)
        {
            var tentative = await _context.TentativesQuiz
                .Include(t => t.Quiz)
                .Include(t => t.ReponsesUtilisateur)
                .FirstOrDefaultAsync(t => t.TentativeId == dto.TentativeId);

            if (tentative == null)
                return NotFound("Tentative de quiz non trouvée");

            if (tentative.Statut != StatutTentative.EnCours)
                return BadRequest("Cette tentative de quiz n'est plus en cours");

            var userId = User.FindFirst("sub")?.Value;
            if (userId == null || tentative.AppUserId.ToString() != userId)
                return Forbid();

            var tempsEcoule = (int)(DateTime.UtcNow - tentative.DateDebut.Value).TotalMinutes;
            if (tempsEcoule > tentative.Quiz.Duree)
            {
                tentative.Statut = StatutTentative.Expiree;
                await _context.SaveChangesAsync();
                return BadRequest("Le temps alloué pour ce quiz est dépassé");
            }

            var questions = await _context.Questions
                .Include(q => q.Reponses)
                .Where(q => q.QuizId == tentative.QuizId)
                .ToListAsync();

            foreach (var reponseDto in dto.Reponses)
            {
                var question = questions.FirstOrDefault(q => q.QuestionId == reponseDto.QuestionId);
                if (question == null) continue;

                bool estCorrecte = false;

                if (question.Type == TypeQuestion.ReponseTexte)
                {
                    estCorrecte = true; // À personnaliser selon ta logique de correction
                }
                else if (reponseDto.ReponseIds != null && reponseDto.ReponseIds.Any())
                {
                    if (question.Type == TypeQuestion.ChoixUnique || question.Type == TypeQuestion.VraiFaux)
                    {
                        var reponseId = reponseDto.ReponseIds.First();
                        estCorrecte = question.Reponses.Any(r => r.ReponseId == reponseId && r.EstCorrecte);
                    }
                    else if (question.Type == TypeQuestion.ChoixMultiple)
                    {
                        var reponsesCorrectes = question.Reponses.Where(r => r.EstCorrecte).Select(r => r.ReponseId).ToList();
                        var reponsesIncorrectes = question.Reponses.Where(r => !r.EstCorrecte).Select(r => r.ReponseId).ToList();
                        estCorrecte = reponsesCorrectes.All(r => reponseDto.ReponseIds.Contains(r)) &&
                                      !reponseDto.ReponseIds.Any(r => reponsesIncorrectes.Contains(r));
                    }
                }

                var reponseUtilisateur = tentative.ReponsesUtilisateur
                    .FirstOrDefault(ru => ru.QuestionId == reponseDto.QuestionId);

                if (reponseUtilisateur == null)
                {
                    reponseUtilisateur = new ReponseUtilisateur
                    {
                        ReponseUtilisateurId = Guid.NewGuid(),
                        TentativeId = tentative.TentativeId,
                        QuestionId = reponseDto.QuestionId,
                        EstCorrecte = estCorrecte,
                        TempsReponse = reponseDto.TempsReponse
                    };

                    if (question.Type == TypeQuestion.ReponseTexte)
                        reponseUtilisateur.TexteReponse = reponseDto.TexteReponse;
                    else if (reponseDto.ReponseIds != null && reponseDto.ReponseIds.Any())
                        reponseUtilisateur.ReponseId = reponseDto.ReponseIds.First();

                    _context.ReponsesUtilisateur.Add(reponseUtilisateur);
                }
                else
                {
                    reponseUtilisateur.EstCorrecte = estCorrecte;
                    reponseUtilisateur.TempsReponse = reponseDto.TempsReponse;
                    if (question.Type == TypeQuestion.ReponseTexte)
                        reponseUtilisateur.TexteReponse = reponseDto.TexteReponse;
                    else if (reponseDto.ReponseIds != null && reponseDto.ReponseIds.Any())
                        reponseUtilisateur.ReponseId = reponseDto.ReponseIds.First();
                }
            }

            tentative.Statut = StatutTentative.Terminee;
            tentative.DateFin = DateTime.UtcNow;

            int questionsCorrectes = tentative.ReponsesUtilisateur.Count(ru => ru.EstCorrecte);
            double score = questions.Count > 0 ? (double)questionsCorrectes / questions.Count * 100 : 0;
            tentative.Score = score;

            var resultat = new ResultatQuiz
            {
                ResultatId = Guid.NewGuid(),
                TentativeId = tentative.TentativeId,
                Score = score,
                QuestionsCorrectes = questionsCorrectes,
                NombreQuestions = questions.Count,
                TempsTotal = tempsEcoule,
                Reussi = score >= tentative.Quiz.ScoreMinimum,
                DateResultat = DateTime.UtcNow,
                QuizId = tentative.QuizId,
                AppUserId = tentative.AppUserId
            };
            _context.ResultatsQuiz.Add(resultat);
            await _context.SaveChangesAsync();

            return Ok(new ResultatQuizResponseDto
            {
                ResultatId = resultat.ResultatId,
                TentativeId = resultat.TentativeId,
                Score = resultat.Score,
                QuestionsCorrectes = resultat.QuestionsCorrectes,
                NombreQuestions = resultat.NombreQuestions,
                TempsTotal = resultat.TempsTotal,
                Reussi = resultat.Reussi,
                DateResultat = resultat.DateResultat
            });
        }

        // GET: api/Quiz/Resultats/{tentativeId}
        [HttpGet("Resultats/{tentativeId}")]
        [Authorize]
        public async Task<ActionResult<ResultatDetailResponseDto>> GetResultatQuiz(Guid tentativeId)
        {
            var tentative = await _context.TentativesQuiz
                .Include(t => t.Quiz)
                .Include(t => t.Resultat)
                .Include(t => t.ReponsesUtilisateur)
                    .ThenInclude(ru => ru.Question)
                .Include(t => t.ReponsesUtilisateur)
                    .ThenInclude(ru => ru.Reponse)
                .FirstOrDefaultAsync(t => t.TentativeId == tentativeId);

            if (tentative == null)
                return NotFound("Tentative de quiz non trouvée");

            var userId = User.FindFirst("sub")?.Value;
            var userRoles = User.FindFirst("role")?.Value;

            if (userId == null ||
                (tentative.AppUserId.ToString() != userId &&
                 userRoles != "Admin" &&
                 userRoles != "Recruteur"))
            {
                return Forbid();
            }

            if (tentative.Statut != StatutTentative.Terminee)
                return BadRequest("Cette tentative de quiz n'est pas terminée");

            if (tentative.Resultat == null)
                return NotFound("Résultat non trouvé pour cette tentative");

            var resultatDetailDto = new ResultatDetailResponseDto
            {
                ResultatId = tentative.Resultat.ResultatId,
                TentativeId = tentative.TentativeId,
                QuizId = tentative.QuizId,
                QuizTitre = tentative.Quiz.Titre,
                Score = tentative.Resultat.Score,
                QuestionsCorrectes = tentative.Resultat.QuestionsCorrectes,
                NombreQuestions = tentative.Resultat.NombreQuestions,
                TempsTotal = tentative.Resultat.TempsTotal,
                Reussi = tentative.Resultat.Reussi,
                DateResultat = tentative.Resultat.DateResultat,
                ReponsesDetail = tentative.ReponsesUtilisateur.Select(ru => new ReponseDetailResponseDto
                {
                    QuestionId = ru.QuestionId,
                    QuestionTexte = ru.Question.Texte,
                    QuestionType = ru.Question.Type,
                    EstCorrecte = ru.EstCorrecte,
                    TempsReponse = ru.TempsReponse,
                    TexteReponse = ru.TexteReponse,
                    ReponseId = ru.ReponseId,
                    ReponseTexte = ru.Reponse?.Texte,
                    Explication = ru.Reponse?.Explication
                }).ToList()
            };

            return Ok(resultatDetailDto);
        }

        // NEW ENDPOINT: GET api/Quiz/ResultatsByQuizAndToken/{quizId}?token={token}
        [HttpGet("ResultatsByQuizAndToken/{quizId}")]
        [Authorize]
        public async Task<ActionResult<ResultatDetailResponseDto>> GetResultatByQuizIdAndToken(Guid quizId, [FromQuery] string token)
        {
            // Validate the quiz exists
            var quiz = await _context.Quizzes.FindAsync(quizId);
            if (quiz == null)
                return NotFound("Quiz non trouvé");

            // Find the latest completed attempt for the quiz and token
            var tentative = await _context.TentativesQuiz
                .Include(t => t.Quiz)
                .Include(t => t.Resultat)
                .Include(t => t.ReponsesUtilisateur)
                    .ThenInclude(ru => ru.Question)
                .Include(t => t.ReponsesUtilisateur)
                    .ThenInclude(ru => ru.Reponse)
                .FirstOrDefaultAsync(t => t.QuizId == quizId && t.Token == token && t.Statut == StatutTentative.Terminee);

            if (tentative == null)
                return NotFound("Aucune tentative terminée trouvée pour ce quiz et ce token");

            // Verify user authorization
            var userId = User.FindFirst("sub")?.Value;
            var userRoles = User.FindFirst("role")?.Value;

            if (userId == null ||
                (tentative.AppUserId.ToString() != userId &&
                 userRoles != "Admin" &&
                 userRoles != "Recruteur"))
            {
                return Forbid();
            }

            if (tentative.Resultat == null)
                return NotFound("Résultat non trouvé pour cette tentative");

            // Construct the response
            var resultatDetailDto = new ResultatDetailResponseDto
            {
                ResultatId = tentative.Resultat.ResultatId,
                TentativeId = tentative.TentativeId,
                QuizId = tentative.QuizId,
                QuizTitre = tentative.Quiz.Titre,
                Score = tentative.Resultat.Score,
                QuestionsCorrectes = tentative.Resultat.QuestionsCorrectes,
                NombreQuestions = tentative.Resultat.NombreQuestions,
                TempsTotal = tentative.Resultat.TempsTotal,
                Reussi = tentative.Resultat.Reussi,
                DateResultat = tentative.Resultat.DateResultat,
                ReponsesDetail = tentative.ReponsesUtilisateur.Select(ru => new ReponseDetailResponseDto
                {
                    QuestionId = ru.QuestionId,
                    QuestionTexte = ru.Question.Texte,
                    QuestionType = ru.Question.Type,
                    EstCorrecte = ru.EstCorrecte,
                    TempsReponse = ru.TempsReponse,
                    TexteReponse = ru.TexteReponse,
                    ReponseId = ru.ReponseId,
                    ReponseTexte = ru.Reponse?.Texte,
                    Explication = ru.Reponse?.Explication
                }).ToList()
            };

            return Ok(resultatDetailDto);
        }

        // GET: api/Quiz/Recherche?titre={titre}
        [HttpGet("Recherche")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<QuizResponseDto>>> RechercherQuizParTitre([FromQuery] string titre)
        {
            if (string.IsNullOrWhiteSpace(titre))
                return BadRequest("Le paramètre 'titre' est requis.");

            var quizzes = await _context.Quizzes
                .Where(q => q.Titre.Contains(titre))
                .Select(q => new QuizResponseDto
                {
                    QuizId = q.QuizId,
                    Titre = q.Titre,
                    Description = q.Description,
                    DateCreation = q.DateCreation,
                    EstActif = q.EstActif,
                    Duree = q.Duree,
                    ScoreMinimum = q.ScoreMinimum,
                    OffreEmploiId = q.OffreEmploiId
                })
                .ToListAsync();

            return Ok(quizzes);
        }

        [HttpGet("StartQuiz/{tentativeId}")]
        [Authorize(Roles = "Candidate")] // Requires Candidate role
        public async Task<ActionResult<QuizPourCandidatDto>> DemarrerQuiz(
            [FromQuery] Guid tentativeId)
        {
            var tentative = await _context.TentativesQuiz
                .Include(t => t.Quiz)
                .FirstOrDefaultAsync(t => t.TentativeId == tentativeId);

            if (tentative == null)
                return NotFound("Tentative non trouvée");

            if (tentative.DateDebut != null)
                return BadRequest("Quiz déjà commencé");

            if (tentative.DateFin < DateTime.UtcNow)
                return BadRequest("Lien expiré");

            tentative.DateDebut = DateTime.UtcNow;
            tentative.Statut = StatutTentative.EnCours;

            var quiz = await _context.Quizzes
                .Include(q => q.Questions)
                    .ThenInclude(q => q.Reponses)
                .FirstOrDefaultAsync(q => q.QuizId == tentative.QuizId);

            if (quiz == null)
                return NotFound("Quiz introuvable");

            var quizDto = new QuizPourCandidatDto
            {
                QuizId = quiz.QuizId,
                Titre = quiz.Titre,
                Description = quiz.Description,
                Duree = quiz.Duree,
                Questions = quiz.Questions.Select(q => new QuestionPourCandidatDto
                {
                    QuestionId = q.QuestionId,
                    Texte = q.Texte,
                    Type = q.Type,
                    Points = q.Points,
                    Ordre = q.Ordre,
                    TempsRecommande = q.TempsRecommande,
                    Reponses = q.Reponses.Select(r => new ReponsePourCandidatDto
                    {
                        ReponseId = r.ReponseId,
                        Texte = r.Texte,
                        Ordre = r.Ordre,
                        Explication = null
                    }).ToList()
                }).ToList()
            };

            await _context.SaveChangesAsync();
            return Ok(quizDto);
        }


        [HttpGet("Status/{tentativeId}")]
        [AllowAnonymous]
        public async Task<ActionResult<TentativeStatusDto>> GetTentativeStatus(Guid tentativeId)
        {
            var tentative = await _context.TentativesQuiz.FindAsync(tentativeId);

            if (tentative == null) return NotFound();

            return new TentativeStatusDto
            {
                Statut = tentative.Statut,
                TempsRestant = tentative.DateDebut.HasValue
                    ? tentative.Quiz.Duree * 60 - (int)(DateTime.UtcNow - tentative.DateDebut.Value).TotalSeconds
                    : null
            };
        }

        // DTOs
        public class QuizPourCandidatDto
        {
            public Guid QuizId { get; set; }
            public string Titre { get; set; }
            public string Description { get; set; }
            public int Duree { get; set; }
            public List<QuestionPourCandidatDto> Questions { get; set; }
        }

        public class QuestionPourCandidatDto
        {
            public Guid QuestionId { get; set; }
            public string Texte { get; set; }
            public TypeQuestion Type { get; set; }
            public int Points { get; set; }
            public int Ordre { get; set; }
            public int? TempsRecommande { get; set; }
            public List<ReponsePourCandidatDto> Reponses { get; set; }
        }

        public class ReponsePourCandidatDto
        {
            public Guid ReponseId { get; set; }
            public string Texte { get; set; }
            public int Ordre { get; set; }
            public string Explication { get; set; } // Toujours null pour candidat
        }
    }
}