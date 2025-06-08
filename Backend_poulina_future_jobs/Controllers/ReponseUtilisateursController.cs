using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class ReponseUtilisateurController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReponseUtilisateurController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ReponseUtilisateur/Tentative/{tentativeId}
        [HttpGet("Tentative/{tentativeId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ReponseUtilisateurDto>>> GetReponsesByTentative(Guid tentativeId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tentative = await _context.TentativesQuiz.FindAsync(tentativeId);

            if (tentative == null)
                return NotFound();

            if (currentUserId != tentative.AppUserId.ToString() && !User.IsInRole("Admin") && !User.IsInRole("Recruteur"))
                return Forbid();

            var reponses = await _context.ReponsesUtilisateur
               .Include(ru => ru.Question)
               .Include(ru => ru.Reponse)
               .Where(ru => ru.TentativeId == tentativeId)
               .Select(ru => new ReponseUtilisateurDto
               {
                   ReponseUtilisateurId = ru.ReponseUtilisateurId,
                   QuestionId = ru.QuestionId,
                   QuestionTexte = ru.Question.Texte,
                   QuestionType = ru.Question.Type,
                   QuestionPoints = ru.Question.Points,
                   ReponseId = ru.ReponseId,
                   ReponseTexte = ru.Reponse != null ? ru.Reponse.Texte : string.Empty, // Fix for CS8601 and CS8072  
                   TexteReponse = ru.TexteReponse,
                   TempsReponse = ru.TempsReponse,
                   EstCorrecte = ru.EstCorrecte
               })
               .ToListAsync();

            return Ok(reponses);
        }

        // POST: api/ReponseUtilisateur
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ReponseUtilisateurDto>> CreateReponseUtilisateur(ReponseUtilisateurCreateDto dto)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var tentative = await _context.TentativesQuiz.FindAsync(dto.TentativeId);

            if (tentative == null)
                return NotFound("Tentative non trouvée");

            if (currentUserId != tentative.AppUserId.ToString())
                return Forbid();

            var question = await _context.Questions.FindAsync(dto.QuestionId);
            if (question == null)
                return NotFound("Question non trouvée");

            var reponseUtilisateur = new ReponseUtilisateur
            {
                ReponseUtilisateurId = Guid.NewGuid(),
                TentativeId = dto.TentativeId,
                QuestionId = dto.QuestionId,
                ReponseId = dto.ReponseIds?.FirstOrDefault(),
                TexteReponse = dto.TexteReponse,
                TempsReponse = dto.TempsReponse,
                EstCorrecte = false // Will be calculated when submitting the quiz
            };

            _context.ReponsesUtilisateur.Add(reponseUtilisateur);
            await _context.SaveChangesAsync();

            var reponseDto = new ReponseUtilisateurDto
            {
                ReponseUtilisateurId = reponseUtilisateur.ReponseUtilisateurId,
                QuestionId = reponseUtilisateur.QuestionId,
                QuestionTexte = question.Texte,
                QuestionType = question.Type,
                QuestionPoints = question.Points,
                ReponseId = reponseUtilisateur.ReponseId,
                ReponseTexte = reponseUtilisateur.Reponse?.Texte,
                TexteReponse = reponseUtilisateur.TexteReponse,
                TempsReponse = reponseUtilisateur.TempsReponse,
                EstCorrecte = reponseUtilisateur.EstCorrecte
            };

            return CreatedAtAction(nameof(GetReponsesByTentative), new { tentativeId = reponseUtilisateur.TentativeId }, reponseDto);
        }
    }
}