//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Backend_poulina_future_jobs.Models;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Authorization;
//using System.Security.Claims;

//namespace Backend_poulina_future_jobs.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    [AllowAnonymous]
//    public class ProfileController : ControllerBase
//    {
//        private readonly AppDbContext _context;
//        private readonly UserManager<AppUser> _userManager;
//        public ProfileController(AppDbContext context, UserManager<AppUser> userManager)
//        {
//            _context = context;
//            _userManager = userManager;
//        }

//        // GET: api/Profile
//        // GET: Récupérer le profil de l'utilisateur connecté
//        [HttpGet("me")]
//        [Authorize] // Remplace AllowAnonymous
//        public async Task<IActionResult> GetProfile()
//        {
//            if (!User.Identity.IsAuthenticated)
//            {
//                return Unauthorized("Utilisateur non authentifié");
//            }

//            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            if (string.IsNullOrEmpty(userId))
//            {
//                return BadRequest("ID utilisateur non trouvé dans le token");
//            }

//            var user = await _userManager.FindByIdAsync(userId);
//            if (user == null)
//            {
//                return NotFound("Utilisateur non trouvé dans la base de données");
//            }

//            return Ok(user);
//        }


//        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
//        // PUT: Mettre à jour le profil
       
//        [HttpPut("update")]
//        [Authorize] // Remplace AllowAnonymous
//        public async Task<IActionResult> UpdateProfile([FromBody] AppUser updatedUser)
//        {
//            // Vérifier si l'utilisateur est authentifié
//            if (!User.Identity.IsAuthenticated)
//            {
//                return Unauthorized("Utilisateur non authentifié");
//            }

//            // Récupérer l'ID utilisateur depuis le token
//            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//            if (string.IsNullOrEmpty(userId))
//            {
//                return BadRequest("ID utilisateur non trouvé dans le token");
//            }

//            // Récupérer l'utilisateur depuis la base de données
//            var user = await _userManager.FindByIdAsync(userId);
//            if (user == null)
//            {
//                return NotFound("Utilisateur non trouvé");
//            }

//            // Mise à jour de tous les champs
//            user.FullName = updatedUser.FullName ?? user.FullName;
//            user.Nom = updatedUser.Nom ?? user.Nom;
//            user.Prenom = updatedUser.Prenom ?? user.Prenom;
//            user.Photo = updatedUser.Photo ?? user.Photo;
//            user.DateNaissance = updatedUser.DateNaissance ?? user.DateNaissance;
//            user.Adresse = updatedUser.Adresse ?? user.Adresse;
//            user.Ville = updatedUser.Ville ?? user.Ville;
//            user.Pays = updatedUser.Pays ?? user.Pays;
//            user.phone = updatedUser.phone ?? user.phone;
//            user.NiveauEtude = updatedUser.NiveauEtude ?? user.NiveauEtude;
//            user.Diplome = updatedUser.Diplome ?? user.Diplome;
//            user.Universite = updatedUser.Universite ?? user.Universite;
//            user.specialite = updatedUser.specialite ?? user.specialite;
//            user.cv = updatedUser.cv ?? user.cv;
//            user.linkedIn = updatedUser.linkedIn ?? user.linkedIn;
//            user.github = updatedUser.github ?? user.github;
//            user.portfolio = updatedUser.portfolio ?? user.portfolio;
//            user.Entreprise = updatedUser.Entreprise ?? user.Entreprise;
//            user.Poste = updatedUser.Poste ?? user.Poste;

//            // Mettre à jour l'utilisateur dans la base de données
//            var result = await _userManager.UpdateAsync(user);
//            if (result.Succeeded)
//            {
//                return Ok("Profil mis à jour avec succès");
//            }

//            return BadRequest(result.Errors);
//        }
//    }
//}

