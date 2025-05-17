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
    public class ReponsesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReponsesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Quiz/{quizId}/Question/{questionId}/Reponse
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReponseDto>>> GetReponses(Guid quizId, Guid questionId)
        {
            if (!await _context.Questions.AnyAsync(q => q.QuizId == quizId && q.QuestionId == questionId))
                return NotFound("Question non trouvée");

            var reponses = await _context.Reponses
                .Where(r => r.QuestionId == questionId)
                .Select(r => new ReponseDto
                {
                    ReponseId = r.ReponseId,
                    Texte = r.Texte,
                    EstCorrecte = r.EstCorrecte,
                    Ordre = r.Ordre,
                    Explication = r.Explication,
                    QuestionId = r.QuestionId
                })
                .ToListAsync();
            return Ok(reponses);
        }

        // GET: api/Quiz/{quizId}/Question/{questionId}/Reponse/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ReponseDto>> GetReponse(Guid quizId, Guid questionId, Guid id)
        {
            var reponse = await _context.Reponses
                .Where(r => r.QuestionId == questionId && r.ReponseId == id)
                .Select(r => new ReponseDto
                {
                    ReponseId = r.ReponseId,
                    Texte = r.Texte,
                    EstCorrecte = r.EstCorrecte,
                    Ordre = r.Ordre,
                    Explication = r.Explication,
                    QuestionId = r.QuestionId
                })
                .FirstOrDefaultAsync();

            if (reponse == null)
                return NotFound();

            return Ok(reponse);
        }

        // POST: api/Quiz/{quizId}/Question/{questionId}/Reponse
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult<ReponseDto>> CreateReponse(Guid quizId, Guid questionId, ReponseCreateDto dto)
        {
            if (!await _context.Questions.AnyAsync(q => q.QuizId == quizId && q.QuestionId == questionId))
                return NotFound("Question non trouvée");

            var reponse = new Reponse
            {
                ReponseId = Guid.NewGuid(),
                Texte = dto.Texte,
                EstCorrecte = dto.EstCorrecte,
                Ordre = dto.Ordre,
                Explication = dto.Explication,
                QuestionId = questionId
            };

            _context.Reponses.Add(reponse);
            await _context.SaveChangesAsync();

            var reponseDto = new ReponseDto
            {
                ReponseId = reponse.ReponseId,
                Texte = reponse.Texte,
                EstCorrecte = reponse.EstCorrecte,
                Ordre = reponse.Ordre,
                Explication = reponse.Explication,
                QuestionId = reponse.QuestionId
            };

            return CreatedAtAction(nameof(GetReponse), new { quizId, questionId, id = reponse.ReponseId }, reponseDto);
        }

        // PUT: api/Quiz/{quizId}/Question/{questionId}/Reponse/{id}
        [HttpPut("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateReponse(Guid quizId, Guid questionId, Guid id, ReponseUpdateDto dto)
        {
            var reponse = await _context.Reponses
                .FirstOrDefaultAsync(r => r.QuestionId == questionId && r.ReponseId == id);
            if (reponse == null)
                return NotFound();

            if (!string.IsNullOrEmpty(dto.Texte))
                reponse.Texte = dto.Texte;
            if (dto.EstCorrecte.HasValue)
                reponse.EstCorrecte = dto.EstCorrecte.Value;
            if (dto.Ordre.HasValue)
                reponse.Ordre = dto.Ordre.Value;
            if (!string.IsNullOrEmpty(dto.Explication))
                reponse.Explication = dto.Explication;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/Quiz/{quizId}/Question/{questionId}/Reponse/{id}
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteReponse(Guid quizId, Guid questionId, Guid id)
        {
            var reponse = await _context.Reponses
                .FirstOrDefaultAsync(r => r.QuestionId == questionId && r.ReponseId == id);
            if (reponse == null)
                return NotFound();

            _context.Reponses.Remove(reponse);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool ReponseExists(Guid id)
        {
            return _context.Reponses.Any(e => e.ReponseId == id);
        }
    }
    public class ReponseCreateDto
    {
        [Required]
        [StringLength(500)]
        public string Texte { get; set; }

        public bool EstCorrecte { get; set; } = false;
        public int Ordre { get; set; }
        [StringLength(500)]
        public string Explication { get; set; }
    }

    public class ReponseUpdateDto
    {
        [StringLength(500)]
        public string Texte { get; set; }

        public bool? EstCorrecte { get; set; }
        public int? Ordre { get; set; }
        [StringLength(500)]
        public string Explication { get; set; }
    }

    public class ReponseDto
    {
        public Guid ReponseId { get; set; }
        public string Texte { get; set; }
        public bool EstCorrecte { get; set; }
        public int Ordre { get; set; }
        public string Explication { get; set; }
        public Guid QuestionId { get; set; }
    }

}
