//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Backend_poulina_future_jobs.Models;
//using System.ComponentModel.DataAnnotations;
//using Microsoft.AspNetCore.Authorization;
//using Backend_poulina_future_jobs.DTOs;

//namespace Backend_poulina_future_jobs.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class QuestionController : ControllerBase
//    {
//        private readonly AppDbContext _context;
//        private readonly ILogger<QuestionController> _logger;

//        public QuestionController(AppDbContext context, ILogger<QuestionController> logger)
//        {
//            _context = context;
//            _logger = logger;
//        }

//        // GET: api/Question/Quiz/{quizId}
//        [HttpGet("Quiz/{quizId}")]
//        [AllowAnonymous]
//        public async Task<ActionResult<IEnumerable<QuestionResponseDto>>> GetQuestionsByQuiz(Guid quizId)
//        {
//            var questions = await _context.Questions
//                .Where(q => q.QuizId == quizId)
//                .Include(q => q.Reponses)
//                .OrderBy(q => q.Ordre)
//                .Select(q => new QuestionResponseDto
//                {
//                    QuestionId = q.QuestionId,
//                    Texte = q.Texte,
//                    Type = q.Type,
//                    Points = q.Points,
//                    Ordre = q.Ordre,
//                    TempsRecommande = q.TempsRecommande,
//                    QuizId = q.QuizId,
//                    Reponses = q.Reponses.OrderBy(r => r.Ordre).Select(r => new ReponseResponseDtos
//                    {
//                        ReponseId = r.ReponseId,
//                        Texte = r.Texte,
//                        EstCorrecte = r.EstCorrecte,
//                        Ordre = r.Ordre,
//                        Explication = r.Explication
//                    }).ToList()
//                })
//                .ToListAsync();

//            return Ok(questions);
//        }

//        // GET: api/Question/{id}
//        [HttpGet("{id}")]
//        [AllowAnonymous]
//        public async Task<ActionResult<QuestionResponseDto>> GetQuestion(Guid id)
//        {
//            var question = await _context.Questions
//                .Include(q => q.Reponses)
//                .Where(q => q.QuestionId == id)
//                .Select(q => new QuestionResponseDto
//                {
//                    QuestionId = q.QuestionId,
//                    Texte = q.Texte,
//                    Type = q.Type,
//                    Points = q.Points,
//                    Ordre = q.Ordre,
//                    TempsRecommande = q.TempsRecommande,
//                    QuizId = q.QuizId,
//                    Reponses = q.Reponses.OrderBy(r => r.Ordre).Select(r => new ReponseResponseDtos
//                    {
//                        ReponseId = r.ReponseId,
//                        Texte = r.Texte,
//                        EstCorrecte = r.EstCorrecte,
//                        Ordre = r.Ordre,
//                        Explication = r.Explication
//                    }).ToList()
//                })
//                .FirstOrDefaultAsync();

//            if (question == null)
//                return NotFound();

//            return Ok(question);
//        }

//        // POST: api/Question
//        [HttpPost]
//        [Authorize(Roles = "Admin,Recruteur")]
//        public async Task<ActionResult<QuestionResponseDto>> CreateQuestion(QuestionCreateDto dto)
//        {
//            var quiz = await _context.Quizzes.FindAsync(dto.QuizId);
//            if (quiz == null)
//                return BadRequest("Quiz non trouvé");

//            // Validation des types de questions
//            var validationError = ValidateQuestion(dto);
//            if (validationError != null)
//                return BadRequest(validationError);

//            var question = new Question
//            {
//                QuestionId = Guid.NewGuid(),
//                QuizId = dto.QuizId,
//                Texte = dto.Texte,
//                Type = dto.Type,
//                Points = dto.Points,
//                Ordre = dto.Ordre,
//                TempsRecommande = dto.TempsRecommande
//            };

//            _context.Questions.Add(question);

//            // Ajouter les réponses si nécessaire
//            if (dto.Reponses != null && dto.Reponses.Any())
//            {
//                foreach (var reponseDto in dto.Reponses)
//                {
//                    var reponse = new Reponse
//                    {
//                        ReponseId = Guid.NewGuid(),
//                        QuestionId = question.QuestionId,
//                        Texte = reponseDto.Texte,
//                        EstCorrecte = reponseDto.EstCorrecte,
//                        Ordre = reponseDto.Ordre,
//                        Explication = reponseDto.Explication
//                    };
//                    _context.Reponses.Add(reponse);
//                }
//            }

//            await _context.SaveChangesAsync();

//            var questionDto = new QuestionResponseDto
//            {
//                QuestionId = question.QuestionId,
//                Texte = question.Texte,
//                Type = question.Type,
//                Points = question.Points,
//                Ordre = question.Ordre,
//                TempsRecommande = question.TempsRecommande,
//                QuizId = question.QuizId,
//                Reponses = question.Reponses?.Select(r => new ReponseResponseDtos
//                {
//                    ReponseId = r.ReponseId,
//                    Texte = r.Texte,
//                    EstCorrecte = r.EstCorrecte,
//                    Ordre = r.Ordre,
//                    Explication = r.Explication
//                }).ToList() ?? new List<ReponseResponseDtos>()
//            };

//            return CreatedAtAction(nameof(GetQuestion), new { id = question.QuestionId }, questionDto);
//        }

//        // PUT: api/Question/{id}
//        [HttpPut("{id}")]
//        [Authorize(Roles = "Admin,Recruteur")]
//        public async Task<IActionResult> UpdateQuestion(Guid id, QuestionUpdateDto dto)
//        {
//            var question = await _context.Questions.FindAsync(id);
//            if (question == null)
//                return NotFound();

//            if (!string.IsNullOrEmpty(dto.Texte))
//                question.Texte = dto.Texte;

//            if (dto.Points.HasValue)
//                question.Points = dto.Points.Value;

//            if (dto.Ordre.HasValue)
//                question.Ordre = dto.Ordre.Value;

//            if (dto.TempsRecommande.HasValue)
//                question.TempsRecommande = dto.TempsRecommande.Value;

//            await _context.SaveChangesAsync();
//            return NoContent();
//        }

//        // DELETE: api/Question/{id}
//        [HttpDelete("{id}")]
//        [Authorize(Roles = "Admin,Recruteur")]
//        public async Task<IActionResult> DeleteQuestion(Guid id)
//        {
//            var question = await _context.Questions
//                .Include(q => q.Reponses)
//                .Include(q => q.ReponsesUtilisateur)
//                .FirstOrDefaultAsync(q => q.QuestionId == id);

//            if (question == null)
//                return NotFound();

//            _context.ReponsesUtilisateur.RemoveRange(question.ReponsesUtilisateur);
//            _context.Reponses.RemoveRange(question.Reponses);
//            _context.Questions.Remove(question);

//            await _context.SaveChangesAsync();
//            return NoContent();
//        }

//        private string ValidateQuestion(QuestionCreateDto dto)
//        {
//            switch (dto.Type)
//            {
//                case TypeQuestion.ReponseTexte:
//                    if (dto.Reponses != null && dto.Reponses.Any())
//                        return "Une question de type RéponseTexte ne peut pas avoir de réponses prédéfinies";
//                    break;

//                case TypeQuestion.ChoixUnique:
//                case TypeQuestion.ChoixMultiple:
//                    if (dto.Reponses == null || !dto.Reponses.Any())
//                        return $"Une question de type {dto.Type} doit avoir au moins une réponse";

//                    if (!dto.Reponses.Any(r => r.EstCorrecte))
//                        return "La question doit avoir au moins une réponse correcte";

//                    if (dto.Type == TypeQuestion.ChoixUnique && dto.Reponses.Count(r => r.EstCorrecte) > 1)
//                        return "Une question à choix unique ne peut avoir qu'une seule réponse correcte";
//                    break;

//                case TypeQuestion.VraiFaux:
//                    if (dto.Reponses == null || dto.Reponses.Count != 2)
//                        return "Une question Vrai/Faux doit avoir exactement deux réponses";

//                    if (dto.Reponses.Count(r => r.EstCorrecte) != 1)
//                        return "Une question Vrai/Faux doit avoir exactement une réponse correcte";
//                    break;
//            }

//            return null;
//        }
//    }

//    // DTOs pour Question
//    public class QuestionCreateDto
//    {
//        [Required]
//        [StringLength(500)]
//        public string Texte { get; set; }

//        [Required]
//        public TypeQuestion Type { get; set; }

//        [Required]
//        [Range(1, 10)]
//        public int Points { get; set; } = 1;

//        [Required]
//        public int Ordre { get; set; }

//        [Range(5, 300)]
//        public int? TempsRecommande { get; set; } // en secondes

//        [Required]
//        public Guid QuizId { get; set; }

//        public List<ReponseCreateDto> Reponses { get; set; }
//    }

//    public class QuestionUpdateDto
//    {
//        [StringLength(500)]
//        public string Texte { get; set; }

//        [Range(1, 10)]
//        public int? Points { get; set; }

//        public int? Ordre { get; set; }

//        [Range(5, 300)]
//        public int? TempsRecommande { get; set; }
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
//        public List<ReponseResponseDtos> Reponses { get; set; }
//    }
//}


