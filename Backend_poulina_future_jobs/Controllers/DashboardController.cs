//using Backend_poulina_future_jobs.Models.DTOs;
//using Backend_poulina_future_jobs.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Authorization;

//namespace Backend_poulina_future_jobs.Controllers
//{
//    public class DashboardController : Controller
//    {
//        private readonly AppDbContext _context;

//        public DashboardController(AppDbContext context)
//        {
//            _context = context;
//        }

//        // GET: api/Dashboard/stats
//        [HttpGet("stats")]
//        [AllowAnonymous]
//        public async Task<ActionResult<DashboardStatsDto>> GetDashboardStats()
//        {
//            try
//            {
//                // Récupérer les IDs des rôles "Recruteur" et "Candidate"
//                var recruteurRole = await _context.Roles
//                    .Where(r => r.Name == "Recruteur")
//                    .FirstOrDefaultAsync();

//                var candidateRole = await _context.Roles
//                    .Where(r => r.Name == "Candidate")
//                    .FirstOrDefaultAsync();

//                if (recruteurRole == null || candidateRole == null)
//                {
//                    return BadRequest(new
//                    {
//                        success = false,
//                        message = "Les rôles 'Recruteur' ou 'Candidate' n'existent pas dans la base de données."
//                    });
//                }

//                // Compter les utilisateurs ayant le rôle "Recruteur"
//                var recruteurCount = await _context.UserRoles
//                    .Where(ur => ur.RoleId == recruteurRole.Id)
//                    .CountAsync();

//                // Compter les utilisateurs ayant le rôle "Candidate"
//                var candidateCount = await _context.UserRoles
//                    .Where(ur => ur.RoleId == candidateRole.Id)
//                    .CountAsync();

//                // Compter les filiales et les offres d'emploi
//                var stats = new DashboardStatsDto
//                {
//                    FilialeCount = await _context.Filiales.CountAsync(),
//                    RecruteurCount = recruteurCount,
//                    CandidateCount = candidateCount,
//                    OffreEmploiCount = await _context.OffresEmploi.CountAsync()
//                };

//                return Ok(new
//                {
//                    success = true,
//                    message = "Statistiques récupérées avec succès.",
//                    data = stats
//                });
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new
//                {
//                    success = false,
//                    message = "Erreur lors de la récupération des statistiques: " + ex.Message
//                });
//            }
//        }
//    }
//}
