using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_poulina_future_jobs.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace Backend_poulina_future_jobs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QuestionsController(AppDbContext context)
        {
            _context = context;
        }



        // GET: api/Quiz/{quizId}/Question
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuestionDto>>> GetQuestions(Guid quizId)
        {
            if (!await _context.Quizzes.AnyAsync(q => q.QuizId == quizId))
                return NotFound("Quiz non trouvé");

            var questions = await _context.Questions
                .Where(q => q.QuizId == quizId)
                .Select(q => new QuestionDto
                {
                    QuestionId = q.QuestionId,
                    Texte = q.Texte,
                    Type = q.Type,
                    Points = q.Points,
                    Ordre = q.Ordre,
                    TempsRecommande = q.TempsRecommande,
                    QuizId = q.QuizId
                })
                .ToListAsync();
            return Ok(questions);
        }

        // GET: api/Quiz/{quizId}/Question/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionDto>> GetQuestion(Guid quizId, Guid id)
        {
            var question = await _context.Questions
                .Where(q => q.QuizId == quizId && q.QuestionId == id)
                .Select(q => new QuestionDto
                {
                    QuestionId = q.QuestionId,
                    Texte = q.Texte,
                    Type = q.Type,
                    Points = q.Points,
                    Ordre = q.Ordre,
                    TempsRecommande = q.TempsRecommande,
                    QuizId = q.QuizId
                })
                .FirstOrDefaultAsync();

            if (question == null)
                return NotFound();

            return Ok(question);
        }

        // POST: api/Quiz/{quizId}/Question
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<QuestionDto>> CreateQuestion(Guid quizId, QuestionCreateDto dto)
        {
            if (!await _context.Quizzes.AnyAsync(q => q.QuizId == quizId))
                return NotFound("Quiz non trouvé");

            var question = new Question
            {
                QuestionId = Guid.NewGuid(),
                Texte = dto.Texte,
                Type = dto.Type,
                Points = dto.Points,
                Ordre = dto.Ordre,
                TempsRecommande = dto.TempsRecommande,
                QuizId = quizId
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            var questionDto = new QuestionDto
            {
                QuestionId = question.QuestionId,
                Texte = question.Texte,
                Type = question.Type,
                Points = question.Points,
                Ordre = question.Ordre,
                TempsRecommande = question.TempsRecommande,
                QuizId = question.QuizId
            };

            return CreatedAtAction(nameof(GetQuestion), new { quizId, id = question.QuestionId }, questionDto);
        }

        // PUT: api/Quiz/{quizId}/Question/{id}
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateQuestion(Guid quizId, Guid id, QuestionUpdateDto dto)
        {
            var question = await _context.Questions
                .FirstOrDefaultAsync(q => q.QuizId == quizId && q.QuestionId == id);
            if (question == null)
                return NotFound();

            if (!string.IsNullOrEmpty(dto.Texte))
                question.Texte = dto.Texte;
            if (dto.Type.HasValue)
                question.Type = dto.Type.Value;
            if (dto.Points.HasValue)
                question.Points = dto.Points.Value;
            if (dto.Ordre.HasValue)
                question.Ordre = dto.Ordre.Value;
            if (dto.TempsRecommande.HasValue)
                question.TempsRecommande = dto.TempsRecommande;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Quiz/{quizId}/Question/{id}
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteQuestion(Guid quizId, Guid id)
        {
            var question = await _context.Questions
                .FirstOrDefaultAsync(q => q.QuizId == quizId && q.QuestionId == id);
            if (question == null)
                return NotFound();

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool QuestionExists(Guid id)
        {
            return _context.Questions.Any(e => e.QuestionId == id);
        }
    }

    public class QuestionCreateDto
    {
        [Required]
        [StringLength(500)]
        public string Texte { get; set; }

        public TypeQuestion Type { get; set; } = TypeQuestion.ChoixUnique;
        public int Points { get; set; } = 1;
        public int Ordre { get; set; }
        public int? TempsRecommande { get; set; } // En secondes
    }

    public class QuestionUpdateDto
    {
        [StringLength(500)]
        public string Texte { get; set; }

        public TypeQuestion? Type { get; set; }
        public int? Points { get; set; }
        public int? Ordre { get; set; }
        public int? TempsRecommande { get; set; }
    }

    public class QuestionDto
    {
        public Guid QuestionId { get; set; }
        public string Texte { get; set; }
        public TypeQuestion Type { get; set; }
        public int Points { get; set; }
        public int Ordre { get; set; }
        public int? TempsRecommande { get; set; }
        public Guid QuizId { get; set; }
    }
}

