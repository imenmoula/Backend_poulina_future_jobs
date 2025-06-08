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
//    public class ReponseController : ControllerBase
//    {
//        private readonly AppDbContext _context;
//        private readonly ILogger<ReponseController> _logger;

//        public ReponseController(AppDbContext context, ILogger<ReponseController> logger)
//        {
//            _context = context;
//            _logger = logger;
//        }

//        // GET: api/Reponse/Question/{questionId}
//        [HttpGet("Question/{questionId}")]
//        [AllowAnonymous]
//        public async Task<ActionResult<IEnumerable<ReponseResponseDto>>> GetReponsesByQuestion(Guid questionId)
//        {
//            var reponses = await _context.Reponses
//                .Where(r => r.QuestionId == questionId)
//                .OrderBy(r => r.Ordre)
//                .Select(r => new ReponseResponseDto
//                {
//                    ReponseId = r.ReponseId,
//                    Texte = r.Texte,
//                    EstCorrecte = r.EstCorrecte,
//                    Ordre = r.Ordre,
//                    Explication = r.Explication,
//                    QuestionId = r.QuestionId
//                })
//                .ToListAsync();

//            return Ok(reponses);
//        }

//        // GET: api/Reponse/{id}
//        [HttpGet("{id}")]
//        [AllowAnonymous]
//        public async Task<ActionResult<ReponseResponseDto>> GetReponse(Guid id)
//        {
//            var reponse = await _context.Reponses
//                .Where(r => r.ReponseId == id)
//                .Select(r => new ReponseResponseDto
//                {
//                    ReponseId = r.ReponseId,
//                    Texte = r.Texte,
//                    EstCorrecte = r.EstCorrecte,
//                    Ordre = r.Ordre,
//                    Explication = r.Explication,
//                    QuestionId = r.QuestionId
//                })
//                .FirstOrDefaultAsync();

//            if (reponse == null)
//                return NotFound();

//            return Ok(reponse);
//        }

//        // POST: api/Reponse
//        [HttpPost]
//        [Authorize(Roles = "Admin,Recruteur")]
//        public async Task<ActionResult<ReponseResponseDto>> CreateReponse(ReponseCreateDto dto)
//        {
//            var question = await _context.Questions.FindAsync(dto.QuestionId);
//            if (question == null)
//                return BadRequest("Question non trouvée");

//            if (question.Type == TypeQuestion.ReponseTexte)
//                return BadRequest("Impossible d'ajouter des réponses à une question de type RéponseTexte");

//            if (question.Type == TypeQuestion.VraiFaux)
//            {
//                var count = await _context.Reponses.CountAsync(r => r.QuestionId == dto.QuestionId);
//                if (count >= 2)
//                    return BadRequest("Une question Vrai/Faux ne peut avoir que deux réponses");
//            }

//            var reponse = new Reponse
//            {
//                ReponseId = Guid.NewGuid(),
//                QuestionId = dto.QuestionId,
//                Texte = dto.Texte,
//                EstCorrecte = dto.EstCorrecte,
//                Ordre = dto.Ordre,
//                Explication = dto.Explication
//            };

//            _context.Reponses.Add(reponse);
//            await _context.SaveChangesAsync();

//            var reponseDto = new ReponseResponseDto
//            {
//                ReponseId = reponse.ReponseId,
//                Texte = reponse.Texte,
//                EstCorrecte = reponse.EstCorrecte,
//                Ordre = reponse.Ordre,
//                Explication = reponse.Explication,
//                QuestionId = reponse.QuestionId
//            };

//            return CreatedAtAction(nameof(GetReponse), new { id = reponse.ReponseId }, reponseDto);
//        }

//        // PUT: api/Reponse/{id}
//        [HttpPut("{id}")]
//        [Authorize(Roles = "Admin,Recruteur")]
//        public async Task<IActionResult> UpdateReponse(Guid id, ReponseUpdateDto dto)
//        {
//            var reponse = await _context.Reponses.FindAsync(id);
//            if (reponse == null)
//                return NotFound();

//            if (!string.IsNullOrEmpty(dto.Texte))
//                reponse.Texte = dto.Texte;

//            if (dto.EstCorrecte.HasValue)
//                reponse.EstCorrecte = dto.EstCorrecte.Value;

//            if (dto.Ordre.HasValue)
//                reponse.Ordre = dto.Ordre.Value;

//            if (dto.Explication != null)
//                reponse.Explication = dto.Explication;

//            await _context.SaveChangesAsync();
//            return NoContent();
//        }

//        // DELETE: api/Reponse/{id}
//        [HttpDelete("{id}")]
//        [Authorize(Roles = "Admin,Recruteur")]
//        public async Task<IActionResult> DeleteReponse(Guid id)
//        {
//            var reponse = await _context.Reponses
//                .Include(r => r.ReponsesUtilisateur)
//                .FirstOrDefaultAsync(r => r.ReponseId == id);

//            if (reponse == null)
//                return NotFound();

//            var question = await _context.Questions.FindAsync(reponse.QuestionId);
//            if (question != null && question.Type == TypeQuestion.VraiFaux)
//            {
//                var count = await _context.Reponses.CountAsync(r => r.QuestionId == reponse.QuestionId);
//                if (count <= 2)
//                    return BadRequest("Une question Vrai/Faux doit avoir exactement deux réponses");
//            }

//            _context.ReponsesUtilisateur.RemoveRange(reponse.ReponsesUtilisateur);
//            _context.Reponses.Remove(reponse);

//            await _context.SaveChangesAsync();
//            return NoContent();
//        }
//    }

//    // DTOs pour Reponse
//    public class ReponseCreateDto
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

//        [Required]
//        public Guid QuestionId { get; set; }
//    }

//    public class ReponseUpdateDto
//    {
//        [StringLength(500)]
//        public string Texte { get; set; }

//        public bool? EstCorrecte { get; set; }

//        public int? Ordre { get; set; }

//        [StringLength(500)]
//        public string Explication { get; set; }
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

//}

