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
    public class QuizController : ControllerBase
    {
        private readonly AppDbContext _context;

        public QuizController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Quiz
        [HttpGet]
        [AllowAnonymous] // Seuls les recruteurs/admins peuvent créer
        public async Task<ActionResult<IEnumerable<QuizDto>>> GetQuizzes()
        {
            var quizzes = await _context.Quizzes
                .Select(q => new QuizDto
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
        public async Task<ActionResult<QuizDto>> GetQuiz(Guid id)
        {
            var quiz = await _context.Quizzes
                .Select(q => new QuizDto
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

        // POST: api/Quiz
        [AllowAnonymous] // Seuls les recruteurs/admins peuvent créer
        [HttpPost]
        public async Task<ActionResult<QuizDto>> CreateQuiz(QuizCreateDto dto)
        {
            var quiz = new Quiz
            {
                QuizId = Guid.NewGuid(),
                Titre = dto.Titre,
                Description = dto.Description,
                Duree = dto.Duree,
                ScoreMinimum = dto.ScoreMinimum,
                OffreEmploiId = dto.OffreEmploiId
            };

            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();

            var quizDto = new QuizDto
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

        // PUT: api/Quiz/{id}
        [AllowAnonymous]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuiz(Guid id, QuizUpdateDto dto)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
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
            if (dto.EstActif.HasValue)
                quiz.EstActif = dto.EstActif.Value;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Quiz/Full
        //        [HttpPost("Full")]
        //        [Authorize]
        //        [ProducesResponseType(typeof(QuizFullDto), 201)]
        //        [ProducesResponseType(400)]
        //        [ProducesResponseType(401)]
        //        public async Task<ActionResult<QuizFullDto>> CreateFullQuiz(CreateFullQuizDto dto)
        //        {
        //            _logger.LogInformation("Creating full quiz with title {Titre}", dto.Titre);

        //            if (!ModelState.IsValid)
        //            {
        //                _logger.LogWarning("Invalid model state for full quiz creation");
        //                return BadRequest(ModelState);
        //            }

        //            // Validate quiz
        //            if (dto.Questions == null || !dto.Questions.Any())
        //            {
        //                _logger.LogWarning("No questions provided for quiz creation");
        //                return BadRequest("Un quiz doit contenir au moins une question.");
        //            }

        //            // Validate questions and responses
        //            foreach (var question in dto.Questions)
        //            {
        //                if (question.Type == TypeQuestion.ReponseTexte)
        //                {
        //                    if (question.Reponses != null && question.Reponses.Any())
        //                    {
        //                        _logger.LogWarning("ReponseTexte question cannot have predefined responses");
        //                        return BadRequest($"La question {question.Texte} de type ReponseTexte ne peut pas avoir de réponses prédéfinies.");
        //                    }
        //                }
        //                else if (question.Type == TypeQuestion.ChoixUnique || question.Type == TypeQuestion.ChoixMultiple || question.Type == TypeQuestion.VraiFaux)
        //                {
        //                    if (question.Reponses == null || !question.Reponses.Any())
        //                    {
        //                        _logger.LogWarning("Non-ReponseTexte question {Texte} must have responses", question.Texte);
        //                        return BadRequest($"La question {question.Texte} de type {question.Type} doit avoir au moins une réponse.");
        //                    }
        //                    if (!question.Reponses.Any(r => r.EstCorrecte))
        //                    {
        //                        _logger.LogWarning("Question {Texte} must have at least one correct response", question.Texte);
        //                        return BadRequest($"La question {question.Texte} doit avoir au moins une réponse correcte.");
        //                    }
        //                    if (question.Type == TypeQuestion.VraiFaux && question.Reponses.Count != 2)
        //                    {
        //                        _logger.LogWarning("VraiFaux question {Texte} must have exactly two responses", question.Texte);
        //                        return BadRequest($"La question {question.Texte} de type VraiFaux doit avoir exactement deux réponses.");
        //                    }
        //                }
        //            }

        //            // Create quiz
        //            var quiz = new Quiz
        //            {
        //                QuizId = Guid.NewGuid(),
        //                Titre = dto.Titre,
        //                Description = dto.Description,
        //                Duree = dto.Duree,
        //                ScoreMinimum = dto.ScoreMinimum,
        //                OffreEmploiId = dto.OffreEmploiId,
        //                EstActif = true,
        //                DateCreation = DateTime.UtcNow
        //            };

        //            _context.Quizzes.Add(quiz);

        //            // Create questions and responses
        //            var questions = new List<Question>();
        //            var reponses = new List<Reponse>();

        //            foreach (var questionDto in dto.Questions)
        //            {
        //                var question = new Question
        //                {
        //                    QuestionId = Guid.NewGuid(),
        //                    QuizId = quiz.QuizId,
        //                    Texte = questionDto.Texte,
        //                    Type = questionDto.Type,
        //                    Points = questionDto.Points,
        //                    Ordre = questionDto.Ordre,
        //                    TempsRecommande = questionDto.TempsRecommande
        //                };
        //                questions.Add(question);

        //                if (questionDto.Reponses != null)
        //                {
        //                    foreach (var reponseDto in questionDto.Reponses)
        //                    {
        //                        var reponse = new Reponse
        //                        {
        //                            ReponseId = Guid.NewGuid(),
        //                            QuestionId = question.QuestionId,
        //                            Texte = reponseDto.Texte,
        //                            EstCorrecte = reponseDto.EstCorrecte,
        //                            Ordre = reponseDto.Ordre,
        //                            Explication = reponseDto.Explication
        //                        };
        //                        reponses.Add(reponse);
        //                    }
        //                }
        //            }

        //            _context.Questions.AddRange(questions);
        //            _context.Reponses.AddRange(reponses);

        //            try
        //            {
        //                await _context.SaveChangesAsync();
        //            }
        //            catch (Exception ex)
        //            {
        //                _logger.LogError(ex, "Failed to save full quiz {QuizId}", quiz.QuizId);
        //                return StatusCode(500, "Erreur lors de la création du quiz.");
        //            }

        //            // Build response DTO
        //            var quizFullDto = new QuizFullDto
        //            {
        //                QuizId = quiz.QuizId,
        //                Titre = quiz.Titre,
        //                Description = quiz.Description,
        //                DateCreation = quiz.DateCreation,
        //                EstActif = quiz.EstActif,
        //                Duree = quiz.Duree,
        //                ScoreMinimum = quiz.ScoreMinimum,
        //                OffreEmploiId = quiz.OffreEmploiId,
        //                Questions = questions.Select(q => new QuestionDto
        //                {
        //                    QuestionId = q.QuestionId,
        //                    Texte = q.Texte,
        //                    Type = q.Type,
        //                    Points = q.Points,
        //                    Ordre = q.Ordre,
        //                    TempsRecommande = q.TempsRecommande,
        //                    QuizId = q.QuizId,
        //                    Reponses = reponses
        //                        .Where(r => r.QuestionId == q.QuestionId)
        //                        .Select(r => new ReponseDto
        //                        {
        //                            ReponseId = r.ReponseId,
        //                            Texte = r.Texte,
        //                            EstCorrecte = r.EstCorrecte,
        //                            Ordre = r.Ordre,
        //                            Explication = r.Explication,
        //                            QuestionId = r.QuestionId
        //                        })
        //                        .ToList()
        //                }).ToList()
        //            };

        //            _logger.LogInformation("Full quiz {QuizId} created successfully", quiz.QuizId);
        //            return CreatedAtAction(nameof(GetQuiz), new { id = quiz.QuizId }, quizFullDto);
        //        }


        // DELETE: api/Quiz/{id}
        [AllowAnonymous]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuiz(Guid id)
        {
            var quiz = await _context.Quizzes.FindAsync(id);
            if (quiz == null)
                return NotFound();

            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
            return NoContent();
        }




        private bool QuizExists(Guid id)
        {
            return _context.Quizzes.Any(e => e.QuizId == id);
        }


        public class QuizCreateDto
        {
            [Required]
            [StringLength(100)]
            public string Titre { get; set; } = string.Empty;

            [StringLength(500)]
            public string Description { get; set; }

            public int Duree { get; set; } // En minutes  
            public int ScoreMinimum { get; set; } = 60;
            public Guid? OffreEmploiId { get; set; } // Optionnel  
        }

        public class QuizUpdateDto
        {
            [StringLength(100)]
            public string Titre { get; set; }

            [StringLength(500)]
            public string Description { get; set; }

            public int? Duree { get; set; }
            public int? ScoreMinimum { get; set; }
            public Guid? OffreEmploiId { get; set; }
            public bool? EstActif { get; set; }
        }

        public class QuizDto
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
    }
}


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
//using Microsoft.Extensions.Logging;

//namespace Backend_poulina_future_jobs.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class QuizController : ControllerBase
//    {
//        private readonly AppDbContext _context;
//        private readonly ILogger<QuizController> _logger;

//        public QuizController(AppDbContext context, ILogger<QuizController> logger)
//        {
//            _context = context;
//            _logger = logger;
//        }

//        // GET: api/Quiz
//        [HttpGet]
//        [AllowAnonymous]
//        public async Task<ActionResult<IEnumerable<QuizDto>>> GetQuizzes()
//        {
//            var quizzes = await _context.Quizzes
//                .Select(q => new QuizDto
//                {
//                    QuizId = q.QuizId,
//                    Titre = q.Titre,
//                    Description = q.Description,
//                    DateCreation = q.DateCreation,
//                    EstActif = q.EstActif,
//                    Duree = q.Duree,
//                    ScoreMinimum = q.ScoreMinimum,
//                    OffreEmploiId = q.OffreEmploiId
//                })
//                .ToListAsync();
//            return Ok(quizzes);
//        }

//        // GET: api/Quiz/{id}
//        [HttpGet("{id}")]
//        [AllowAnonymous]
//        public async Task<ActionResult<QuizDto>> GetQuiz(Guid id)
//        {
//            var quiz = await _context.Quizzes
//                .Select(q => new QuizDto
//                {
//                    QuizId = q.QuizId,
//                    Titre = q.Titre,
//                    Description = q.Description,
//                    DateCreation = q.DateCreation,
//                    EstActif = q.EstActif,
//                    Duree = q.Duree,
//                    ScoreMinimum = q.ScoreMinimum,
//                    OffreEmploiId = q.OffreEmploiId
//                })
//                .FirstOrDefaultAsync(q => q.QuizId == id);

//            if (quiz == null)
//                return NotFound();

//            return Ok(quiz);
//        }

//        // POST: api/Quiz
//        [HttpPost]
//        [Authorize]
//        public async Task<ActionResult<QuizDto>> CreateQuiz(QuizCreateDto dto)
//        {
//            _logger.LogInformation("Creating quiz with title {Titre}", dto.Titre);

//            if (!ModelState.IsValid)
//            {
//                _logger.LogWarning("Invalid model state for quiz creation");
//                return BadRequest(ModelState);
//            }

//            var quiz = new Quiz
//            {
//                QuizId = Guid.NewGuid(),
//                Titre = dto.Titre,
//                Description = dto.Description,
//                Duree = dto.Duree,
//                ScoreMinimum = dto.ScoreMinimum,
//                OffreEmploiId = dto.OffreEmploiId
//            };

//            _context.Quizzes.Add(quiz);
//            await _context.SaveChangesAsync();

//            var quizDto = new QuizDto
//            {
//                QuizId = quiz.QuizId,
//                Titre = quiz.Titre,
//                Description = quiz.Description,
//                DateCreation = quiz.DateCreation,
//                EstActif = quiz.EstActif,
//                Duree = quiz.Duree,
//                ScoreMinimum = quiz.ScoreMinimum,
//                OffreEmploiId = quiz.OffreEmploiId
//            };

//            _logger.LogInformation("Quiz {QuizId} created successfully", quiz.QuizId);
//            return CreatedAtAction(nameof(GetQuiz), new { id = quiz.QuizId }, quizDto);
//        }

//        // POST: api/Quiz/Full
//        [HttpPost("Full")]
//        [Authorize]
//        [ProducesResponseType(typeof(QuizFullDto), 201)]
//        [ProducesResponseType(400)]
//        [ProducesResponseType(401)]
//        public async Task<ActionResult<QuizFullDto>> CreateFullQuiz(CreateFullQuizDto dto)
//        {
//            _logger.LogInformation("Creating full quiz with title {Titre}", dto.Titre);

//            if (!ModelState.IsValid)
//            {
//                _logger.LogWarning("Invalid model state for full quiz creation");
//                return BadRequest(ModelState);
//            }

//            // Validate quiz
//            if (dto.Questions == null || !dto.Questions.Any())
//            {
//                _logger.LogWarning("No questions provided for quiz creation");
//                return BadRequest("Un quiz doit contenir au moins une question.");
//            }

//            // Validate questions and responses
//            foreach (var question in dto.Questions)
//            {
//                if (question.Type == TypeQuestion.ReponseTexte)
//                {
//                    if (question.Reponses != null && question.Reponses.Any())
//                    {
//                        _logger.LogWarning("ReponseTexte question cannot have predefined responses");
//                        return BadRequest($"La question {question.Texte} de type ReponseTexte ne peut pas avoir de réponses prédéfinies.");
//                    }
//                }
//                else if (question.Type == TypeQuestion.ChoixUnique || question.Type == TypeQuestion.ChoixMultiple || question.Type == TypeQuestion.VraiFaux)
//                {
//                    if (question.Reponses == null || !question.Reponses.Any())
//                    {
//                        _logger.LogWarning("Non-ReponseTexte question {Texte} must have responses", question.Texte);
//                        return BadRequest($"La question {question.Texte} de type {question.Type} doit avoir au moins une réponse.");
//                    }
//                    if (!question.Reponses.Any(r => r.EstCorrecte))
//                    {
//                        _logger.LogWarning("Question {Texte} must have at least one correct response", question.Texte);
//                        return BadRequest($"La question {question.Texte} doit avoir au moins une réponse correcte.");
//                    }
//                    if (question.Type == TypeQuestion.VraiFaux && question.Reponses.Count != 2)
//                    {
//                        _logger.LogWarning("VraiFaux question {Texte} must have exactly two responses", question.Texte);
//                        return BadRequest($"La question {question.Texte} de type VraiFaux doit avoir exactement deux réponses.");
//                    }
//                }
//            }

//            // Create quiz
//            var quiz = new Quiz
//            {
//                QuizId = Guid.NewGuid(),
//                Titre = dto.Titre,
//                Description = dto.Description,
//                Duree = dto.Duree,
//                ScoreMinimum = dto.ScoreMinimum,
//                OffreEmploiId = dto.OffreEmploiId,
//                EstActif = true,
//                DateCreation = DateTime.UtcNow
//            };

//            _context.Quizzes.Add(quiz);

//            // Create questions and responses
//            var questions = new List<Question>();
//            var reponses = new List<Reponse>();

//            foreach (var questionDto in dto.Questions)
//            {
//                var question = new Question
//                {
//                    QuestionId = Guid.NewGuid(),
//                    QuizId = quiz.QuizId,
//                    Texte = questionDto.Texte,
//                    Type = questionDto.Type,
//                    Points = questionDto.Points,
//                    Ordre = questionDto.Ordre,
//                    TempsRecommande = questionDto.TempsRecommande
//                };
//                questions.Add(question);

//                if (questionDto.Reponses != null)
//                {
//                    foreach (var reponseDto in questionDto.Reponses)
//                    {
//                        var reponse = new Reponse
//                        {
//                            ReponseId = Guid.NewGuid(),
//                            QuestionId = question.QuestionId,
//                            Texte = reponseDto.Texte,
//                            EstCorrecte = reponseDto.EstCorrecte,
//                            Ordre = reponseDto.Ordre,
//                            Explication = reponseDto.Explication
//                        };
//                        reponses.Add(reponse);
//                    }
//                }
//            }

//            _context.Questions.AddRange(questions);
//            _context.Reponses.AddRange(reponses);

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Failed to save full quiz {QuizId}", quiz.QuizId);
//                return StatusCode(500, "Erreur lors de la création du quiz.");
//            }

//            // Build response DTO
//            var quizFullDto = new QuizFullDto
//            {
//                QuizId = quiz.QuizId,
//                Titre = quiz.Titre,
//                Description = quiz.Description,
//                DateCreation = quiz.DateCreation,
//                EstActif = quiz.EstActif,
//                Duree = quiz.Duree,
//                ScoreMinimum = quiz.ScoreMinimum,
//                OffreEmploiId = quiz.OffreEmploiId,
//                Questions = questions.Select(q => new QuestionDto
//                {
//                    QuestionId = q.QuestionId,
//                    Texte = q.Texte,
//                    Type = q.Type,
//                    Points = q.Points,
//                    Ordre = q.Ordre,
//                    TempsRecommande = q.TempsRecommande,
//                    QuizId = q.QuizId,
//                    Reponses = reponses
//                        .Where(r => r.QuestionId == q.QuestionId)
//                        .Select(r => new ReponseDto
//                        {
//                            ReponseId = r.ReponseId,
//                            Texte = r.Texte,
//                            EstCorrecte = r.EstCorrecte,
//                            Ordre = r.Ordre,
//                            Explication = r.Explication,
//                            QuestionId = r.QuestionId
//                        })
//                        .ToList()
//                }).ToList()
//            };

//            _logger.LogInformation("Full quiz {QuizId} created successfully", quiz.QuizId);
//            return CreatedAtAction(nameof(GetQuiz), new { id = quiz.QuizId }, quizFullDto);
//        }

//        // PUT: api/Quiz/{id}
//        [Authorize]
//        [HttpPut("{id}")]
//        public async Task<IActionResult> UpdateQuiz(Guid id, QuizUpdateDto dto)
//        {
//            var quiz = await _context.Quizzes.FindAsync(id);
//            if (quiz == null)
//                return NotFound();

//            if (!string.IsNullOrEmpty(dto.Titre))
//                quiz.Titre = dto.Titre;
//            if (!string.IsNullOrEmpty(dto.Description))
//                quiz.Description = dto.Description;
//            if (dto.Duree.HasValue)
//                quiz.Duree = dto.Duree.Value;
//            if (dto.ScoreMinimum.HasValue)
//                quiz.ScoreMinimum = dto.ScoreMinimum.Value;
//            if (dto.OffreEmploiId.HasValue)
//                quiz.OffreEmploiId = dto.OffreEmploiId;
//            if (dto.EstActif.HasValue)
//                quiz.EstActif = dto.EstActif.Value;

//            await _context.SaveChangesAsync();
//            return NoContent();
//        }

//        // DELETE: api/Quiz/{id}
//        [Authorize]
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteQuiz(Guid id)
//        {
//            var quiz = await _context.Quizzes.FindAsync(id);
//            if (quiz == null)
//                return NotFound();

//            _context.Quizzes.Remove(quiz);
//            await _context.SaveChangesAsync();
//            return NoContent();
//        }

//        private bool QuizExists(Guid id)
//        {
//            return _context.Quizzes.Any(e => e.QuizId == id);
//        }

//        public class QuizCreateDto
//        {
//            [Required]
//            [StringLength(100)]
//            public string Titre { get; set; } = string.Empty;

//            [StringLength(500)]
//            public string Description { get; set; }

//            public int Duree { get; set; }
//            public int ScoreMinimum { get; set; } = 60;
//            public Guid? OffreEmploiId { get; set; }
//        }

//        public class QuizUpdateDto
//        {
//            [StringLength(100)]
//            public string Titre { get; set; }

//            [StringLength(500)]
//            public string Description { get; set; }

//            public int? Duree { get; set; }
//            public int? ScoreMinimum { get; set; }
//            public Guid? OffreEmploiId { get; set; }
//            public bool? EstActif { get; set; }
//        }

//        public class QuizDto
//        {
//            public Guid QuizId { get; set; }
//            public string Titre { get; set; }
//            public string Description { get; set; }
//            public DateTime DateCreation { get; set; }
//            public bool EstActif { get; set; }
//            public int Duree { get; set; }
//            public int ScoreMinimum { get; set; }
//            public Guid? OffreEmploiId { get; set; }
//        }

//        public class CreateFullQuizDto
//        {
//            [Required]
//            [StringLength(100)]
//            public string Titre { get; set; } = string.Empty;

//            [StringLength(500)]
//            public string Description { get; set; }

//            public int Duree { get; set; }
//            public int ScoreMinimum { get; set; } = 60;
//            public Guid? OffreEmploiId { get; set; }
//            public List<CreateFullQuestionDto> Questions { get; set; }
//        }

//        public class CreateFullQuestionDto
//        {
//            [Required]
//            [StringLength(500)]
//            public string Texte { get; set; }

//            public TypeQuestion Type { get; set; } = TypeQuestion.ChoixUnique;
//            public int Points { get; set; } = 1;
//            public int Ordre { get; set; }
//            public int? TempsRecommande { get; set; }
//            public List<ReponseCreateDto> Reponses { get; set; }
//        }

//        public class ReponseCreateDto
//        {
//            [Required]
//            [StringLength(500)]
//            public string Texte { get; set; }

//            public bool EstCorrecte { get; set; } = false;
//            public int Ordre { get; set; }
//            [StringLength(500)]
//            public string Explication { get; set; }
//        }

//        public class QuizFullDto
//        {
//            public Guid QuizId { get; set; }
//            public string Titre { get; set; }
//            public string Description { get; set; }
//            public DateTime DateCreation { get; set; }
//            public bool EstActif { get; set; }
//            public int Duree { get; set; }
//            public int ScoreMinimum { get; set; }
//            public Guid? OffreEmploiId { get; set; }
//            public List<QuestionDto> Questions { get; set; }
//        }

//        public class QuestionDto
//        {
//            public Guid QuestionId { get; set; }
//            public string Texte { get; set; }
//            public TypeQuestion Type { get; set; }
//            public int Points { get; set; }
//            public int Ordre { get; set; }
//            public int? TempsRecommande { get; set; }
//            public Guid QuizId { get; set; }
//            public List<ReponseDto> Reponses { get; set; }
//        }

//        public class ReponseDto
//        {
//            public Guid ReponseId { get; set; }
//            public string Texte { get; set; }
//            public bool EstCorrecte { get; set; }
//            public int Ordre { get; set; }
//            public string Explication { get; set; }
//            public Guid QuestionId { get; set; }
//        }
//    }
//}