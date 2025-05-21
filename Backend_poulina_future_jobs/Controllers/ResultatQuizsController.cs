using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_poulina_future_jobs.Models;
using Backend_poulina_future_jobs.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Backend_poulina_future_jobs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultatQuizController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ResultatQuizController> _logger;

        public ResultatQuizController(AppDbContext context, ILogger<ResultatQuizController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/ResultatQuiz/User/{userId}
        [HttpGet("User/{userId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ResultatQuizDto>>> GetResultatsByUser(Guid userId)
        {
            // Vérifier que l'utilisateur actuel est autorisé à voir ces résultats
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId != userId.ToString() && !User.IsInRole("Admin") && !User.IsInRole("Recruteur"))
                return Forbid();

            var resultats = await _context.ResultatsQuiz
                .Include(r => r.Tentative)
                    .ThenInclude(t => t.Quiz)
                .Where(r => r.AppUserId == userId)
                .Select(r => new ResultatQuizDto
                {
                    ResultatId = r.ResultatId,
                    TentativeId = r.TentativeId,
                    Score = r.Score,
                    QuestionsCorrectes = r.QuestionsCorrectes,
                    NombreQuestions = r.NombreQuestions,
                    TempsTotal = r.TempsTotal,
                    Reussi = r.Reussi,
                    Commentaire = r.Commentaire,
                    DateResultat = r.DateResultat,
                    QuizId = r.QuizId,
                    QuizTitre = r.Tentative.Quiz.Titre
                })
                .OrderByDescending(r => r.DateResultat)
                .ToListAsync();

            return Ok(resultats);
        }

        // GET: api/ResultatQuiz/Quiz/{quizId}
        [HttpGet("Quiz/{quizId}")]
        [Authorize(Roles = "Admin,Recruteur")]
        public async Task<ActionResult<IEnumerable<ResultatQuizDto>>> GetResultatsByQuiz(Guid quizId)
        {
            var resultats = await _context.ResultatsQuiz
                .Include(r => r.Tentative)
                    .ThenInclude(t => t.AppUser)
                .Where(r => r.QuizId == quizId)
                .Select(r => new ResultatQuizDto
                {
                    ResultatId = r.ResultatId,
                    TentativeId = r.TentativeId,
                    Score = r.Score,
                    QuestionsCorrectes = r.QuestionsCorrectes,
                    NombreQuestions = r.NombreQuestions,
                    TempsTotal = r.TempsTotal,
                    Reussi = r.Reussi,
                    Commentaire = r.Commentaire,
                    DateResultat = r.DateResultat,
                    AppUserId = r.AppUserId,
                    AppUserNom = r.Tentative.AppUser.Nom,
                    AppUserPrenom = r.Tentative.AppUser.Prenom,
                    AppUserEmail = r.Tentative.AppUser.Email
                })
                .OrderByDescending(r => r.DateResultat)
                .ToListAsync();

            return Ok(resultats);
        }

        // GET: api/ResultatQuiz/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ResultatQuizDetailDto>> GetResultat(Guid id)
        {
            var resultat = await _context.ResultatsQuiz
                .Include(r => r.Tentative)
                    .ThenInclude(t => t.Quiz)
                .Include(r => r.Tentative)
                    .ThenInclude(t => t.AppUser)
                .Include(r => r.Tentative)
                    .ThenInclude(t => t.ReponsesUtilisateur)
                        .ThenInclude(ru => ru.Question)
                .Include(r => r.Tentative)
                    .ThenInclude(t => t.ReponsesUtilisateur)
                        .ThenInclude(ru => ru.Reponse)
                .FirstOrDefaultAsync(r => r.ResultatId == id);

            if (resultat == null)
                return NotFound();

            // Vérifier que l'utilisateur actuel est autorisé à voir ce résultat
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId != resultat.AppUserId.ToString() && !User.IsInRole("Admin") && !User.IsInRole("Recruteur"))
                return Forbid();

            var resultatDto = new ResultatQuizDetailDto
            {
                ResultatId = resultat.ResultatId,
                TentativeId = resultat.TentativeId,
                Score = resultat.Score,
                QuestionsCorrectes = resultat.QuestionsCorrectes,
                NombreQuestions = resultat.NombreQuestions,
                TempsTotal = resultat.TempsTotal,
                Reussi = resultat.Reussi,
                Commentaire = resultat.Commentaire,
                DateResultat = resultat.DateResultat,
                QuizId = resultat.QuizId,
                QuizTitre = resultat.Tentative.Quiz.Titre,
                QuizDescription = resultat.Tentative.Quiz.Description,
                QuizScoreMinimum = resultat.Tentative.Quiz.ScoreMinimum,
                AppUserId = resultat.AppUserId,
                AppUserNom = resultat.Tentative.AppUser.Nom,
                AppUserPrenom = resultat.Tentative.AppUser.Prenom,
                AppUserEmail = resultat.Tentative.AppUser.Email,
                ReponsesUtilisateur = resultat.Tentative.ReponsesUtilisateur.Select(ru => new ReponseUtilisateurDto
                {
                    ReponseUtilisateurId = ru.ReponseUtilisateurId,
                    QuestionId = ru.QuestionId,
                    QuestionTexte = ru.Question.Texte,
                    QuestionType = ru.Question.Type,
                    QuestionPoints = ru.Question.Points,
                    ReponseId = ru.ReponseId,
                    ReponseTexte = ru.Reponse?.Texte,
                    TexteReponse = ru.TexteReponse,
                    TempsReponse = ru.TempsReponse,
                    EstCorrecte = ru.EstCorrecte
                }).ToList()
            };

            return Ok(resultatDto);
        }

        // PUT: api/ResultatQuiz/{id}/Commentaire
        [HttpPut("{id}/Commentaire")]
        [Authorize(Roles = "Admin,Recruteur")]
        public async Task<IActionResult> UpdateCommentaire(Guid id, CommentaireDto dto)
        {
            var resultat = await _context.ResultatsQuiz.FindAsync(id);
            if (resultat == null)
                return NotFound();

            resultat.Commentaire = dto.Commentaire;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
