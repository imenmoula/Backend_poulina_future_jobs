//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Backend_poulina_future_jobs.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Backend_poulina_future_jobs.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AppUsersController : ControllerBase
//    {
//        private readonly AppDbContext _context;
//        private readonly UserManager<AppUser> _userManager;
//        private readonly IConfiguration _configuration;

//        public AppUsersController(
//            AppDbContext context,
//            UserManager<AppUser> userManager,
//            IConfiguration configuration)
//        {
//            _context = context;
//            _userManager = userManager;
//            _configuration = configuration;
//        }

//        // GET: api/AppUsers - Récupérer tous les utilisateurs
//        [HttpGet]
//        [AllowAnonymous]
//        public async Task<ActionResult> GetAppUsers()
//        {
//            var users = await _context.AppUser.ToListAsync();
//            return Ok(new ApiResponse<List<AppUser>>
//            {
//                Data = users,
//                Success = true,
//                Message = "Liste des utilisateurs récupérée avec succès"
//            });
//        }

//        // GET: api/AppUsers/5 - Récupérer un utilisateur spécifique
//        [HttpGet("{id}")]
//        [AllowAnonymous]
//        public async Task<ActionResult> GetAppUser(Guid id)
//        {
//            var appUser = await _context.AppUser.FindAsync(id);
//            if (appUser == null)
//            {
//                return NotFound(new ApiResponse<object> { Success = false, Message = "Utilisateur non trouvé" });
//            }

//            // Vérifier si l'utilisateur connecté est un admin ou l'utilisateur lui-même
//            var currentUser = await _userManager.GetUserAsync(User);
//            if (currentUser.Id != id && !await _userManager.IsInRoleAsync(currentUser, "Admin"))
//            {
//                return Forbid();
//            }

//            return Ok(new ApiResponse<AppUser>
//            {
//                Data = appUser,
//                Success = true,
//                Message = "Utilisateur récupéré avec succès"
//            });
//        }

//        // GET: api/AppUsers/role/{roleName} - Récupérer les utilisateurs par rôle
//        [HttpGet("role/{roleName}")]
//        [AllowAnonymous]
//        public async Task<IActionResult> GetUsersByRole(string roleName)
//        {
//            var validRoles = new[] { "Admin", "Candidat", "Recruteur" };
//            if (!validRoles.Contains(roleName))
//            {
//                return BadRequest(new ApiResponse<object> { Success = false, Message = "Rôle invalide" });
//            }

//            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
//            return Ok(new ApiResponse<IList<AppUser>>
//            {
//                Data = usersInRole,
//                Success = true,
//                Message = $"Liste des utilisateurs avec le rôle {roleName} récupérée avec succès"
//            });
//        }

//        // POST: api/AppUsers - Créer un utilisateur
//        [HttpPost]
//        [AllowAnonymous]
//        public async Task<IActionResult> CreateUser([FromBody] CreateUserModel createUserModel)
//        {
//            if (!ModelState.IsValid || string.IsNullOrEmpty(createUserModel.Email))
//            {
//                return BadRequest(new ApiResponse<object> { Success = false, Message = "Données invalides ou email requis" });
//            }

//            var user = new AppUser
//            {
//                Id = Guid.NewGuid(),
//                UserName = createUserModel.Email,
//                Email = createUserModel.Email,
//                FullName = createUserModel.FullName ?? string.Empty,
//                Nom = createUserModel.Nom ?? string.Empty,
//                Prenom = createUserModel.Prenom ?? string.Empty,
//                Photo = createUserModel.Photo ?? string.Empty,
//                Entreprise = createUserModel.Entreprise ?? string.Empty,
//                Poste = createUserModel.Poste ?? string.Empty,
//                EmailConfirmed = true
//            };

//            var defaultPassword = _configuration["AppSettings:DefaultPassword"] ?? "Password123!";
//            var result = await _userManager.CreateAsync(user, defaultPassword);

//            if (!result.Succeeded)
//            {
//                return BadRequest(new ApiResponse<object> { Success = false, Message = "Erreur lors de la création", Errors = result.Errors });
//            }

//            var role = createUserModel.Role switch
//            {
//                "Admin" => "Admin",
//                "Candidat" => "Candidat",
//                "Recruteur" => "Recruteur",
//                _ => "Candidat" 
//            };

//            var roleResult = await _userManager.AddToRoleAsync(user, role);
//            if (!roleResult.Succeeded)
//            {
//                await _userManager.DeleteAsync(user); // Rollback si le rôle échoue
//                return BadRequest(new ApiResponse<object> { Success = false, Message = "Erreur lors de l'ajout du rôle", Errors = roleResult.Errors });
//            }

//            return Ok(new ApiResponse<object>
//            {
//                Data = new { UserId = user.Id, Email = user.Email },
//                Success = true,
//                Message = $"Utilisateur créé avec succès avec le rôle {role}. Mot de passe par défaut : {defaultPassword}"
//            });
//        }

//        // PUT: api/AppUsers/5 - Mettre à jour un utilisateur
//        [HttpPut("{id}")]
//        [Authorize] // Authentification requise
//        public async Task<IActionResult> PutAppUser(Guid id, [FromBody] AppUser UpdateUserModel)
//        {
//            if (id != UpdateUserModel.Id)
//            {
//                return BadRequest(new ApiResponse<object> { Success = false, Message = "ID mismatch" });
//            }

//            var existingUser = await _userManager.FindByIdAsync(id.ToString());
//            if (existingUser == null)
//            {
//                return NotFound(new ApiResponse<object> { Success = false, Message = "Utilisateur non trouvé" });
//            }

//            // Vérifier les permissions
//            var currentUser = await _userManager.GetUserAsync(User);
//            if (currentUser.Id != id && !await _userManager.IsInRoleAsync(currentUser, "Admin"))
//            {
//                return Forbid();
//            }

//            // Mise à jour des champs
//            existingUser.FullName = UpdateUserModel.FullName ?? existingUser.FullName;
//            existingUser.Nom = UpdateUserModel.Nom ?? existingUser.Nom;
//            existingUser.Prenom = UpdateUserModel.Prenom ?? existingUser.Prenom;
//            existingUser.PhoneNumber = UpdateUserModel.PhoneNumber ?? existingUser.PhoneNumber;
//            existingUser.Photo = UpdateUserModel.Photo ?? existingUser.Photo;
//            existingUser.Entreprise = UpdateUserModel.Entreprise ?? existingUser.Entreprise;
//            existingUser.Poste = UpdateUserModel.Poste ?? existingUser.Poste;

//            if (!string.IsNullOrEmpty(UpdateUserModel.Email) && UpdateUserModel.Email != existingUser.Email)
//            {
//                var setEmailResult = await _userManager.SetEmailAsync(existingUser, UpdateUserModel.Email);
//                if (!setEmailResult.Succeeded)
//                {
//                    return BadRequest(new ApiResponse<object> { Success = false, Message = "Erreur email", Errors = setEmailResult.Errors });
//                }
//                await _userManager.SetUserNameAsync(existingUser, UpdateUserModel.Email);
//            }

//            var updateResult = await _userManager.UpdateAsync(existingUser);
//            if (!updateResult.Succeeded)
//            {
//                return BadRequest(new ApiResponse<object> { Success = false, Message = "Erreur mise à jour", Errors = updateResult.Errors });
//            }

//            return Ok(new ApiResponse<object> { Success = true, Message = "Utilisateur mis à jour avec succès" });
//        }

//        // DELETE: api/AppUsers/5 - Supprimer un utilisateur
//        [HttpDelete("{id}")]
//        [AllowAnonymous]
//        public async Task<IActionResult> DeleteAppUser(Guid id)
//        {
//            var appUser = await _userManager.FindByIdAsync(id.ToString());
//            if (appUser == null)
//            {
//                return NotFound(new ApiResponse<object> { Success = false, Message = "Utilisateur non trouvé" });
//            }

//            var result = await _userManager.DeleteAsync(appUser);
//            if (!result.Succeeded)
//            {
//                return BadRequest(new ApiResponse<object> { Success = false, Message = "Erreur suppression", Errors = result.Errors });
//            }

//            return Ok(new ApiResponse<object> { Success = true, Message = "Utilisateur supprimé avec succès" });
//        }

//        // Modèle de réponse standardisée
//        public class ApiResponse<T>
//        {
//            public T Data { get; set; }
//            public bool Success { get; set; }
//            public string Message { get; set; }
//            public IEnumerable<IdentityError> Errors { get; set; }
//        }

//        // Modèle pour la création d'utilisateur
//        public class CreateUserModel
//        {
//            public string Email { get; set; }
//            public string? FullName { get; set; }
//            public string? Nom { get; set; }
//            public string? Prenom { get; set; }
//            public string? Photo { get; set; }
//            public string? Entreprise { get; set; }
//            public string? Poste { get; set; }
//            public string? Role { get; set; } // "Admin", "Candidat", "Recruteur"
//        }
//    }
//}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend_poulina_future_jobs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_poulina_future_jobs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppUsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;

        public AppUsersController(
            AppDbContext context,
            UserManager<AppUser> userManager,
            IConfiguration configuration)
        {
            _context = context;
            _userManager = userManager;
            _configuration = configuration;
        }

        // GET: api/AppUsers - Récupérer tous les utilisateurs
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetAppUsers()
        {
            var users = await _context.AppUser.ToListAsync();
            return Ok(new ApiResponse<List<AppUser>>
            {
                Data = users,
                Success = true,
                Message = "Liste des utilisateurs récupérée avec succès"
            });
        }

        // GET: api/AppUsers/5 - Récupérer un utilisateur spécifique
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetAppUser(Guid id)
        {
            var appUser = await _context.AppUser.FindAsync(id);
            if (appUser == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Utilisateur non trouvé" });
            }

            // Vérifier si l'utilisateur connecté est un admin ou l'utilisateur lui-même
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null && currentUser.Id != id && !await _userManager.IsInRoleAsync(currentUser, "Admin"))
            {
                return Forbid();
            }

            return Ok(new ApiResponse<AppUser>
            {
                Data = appUser,
                Success = true,
                Message = "Utilisateur récupéré avec succès"
            });
        }

        // GET: api/AppUsers/role/{roleName} - Récupérer les utilisateurs par rôle
        [HttpGet("role/{roleName}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUsersByRole(string roleName)
        {
            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
            if (usersInRole == null || !usersInRole.Any())
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Aucun utilisateur trouvé avec le rôle {roleName}" });
            }

            var userDtos = new List<object>();
            foreach (var user in usersInRole)
            {
                // Récupérer tous les rôles de l'utilisateur
                var roles = await _userManager.GetRolesAsync(user);
                var primaryRole = roles.FirstOrDefault(); // Prendre le premier rôle (ou ajuster selon votre logique)

                userDtos.Add(new
                {
                    user.Id,
                    user.UserName,
                    user.Email,
                    user.FullName,
                    user.Nom,
                    user.Prenom,
                    user.PhoneNumber,
                    user.Photo,
                    user.Entreprise,
                    user.Poste,
                    Role = primaryRole // Ajouter le rôle dans la réponse
                });
            }

            return Ok(new ApiResponse<object> { Success = true, Message = "Utilisateurs récupérés avec succès", Data = userDtos });
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserModel createUserModel)
        {
            if (!ModelState.IsValid || string.IsNullOrEmpty(createUserModel.Email))
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Données invalides ou email requis" });
            }

            var user = new AppUser
            {
                Id = Guid.NewGuid(),
                UserName = createUserModel.Email,
                Email = createUserModel.Email,
                FullName = createUserModel.FullName ?? string.Empty,
                Nom = createUserModel.Nom ?? string.Empty,
                Prenom = createUserModel.Prenom ?? string.Empty,
                Photo = createUserModel.Photo ?? string.Empty,
                PhoneNumber = createUserModel.PhoneNumber ?? string.Empty,
                Entreprise = createUserModel.Entreprise ?? string.Empty,
                Poste = createUserModel.Poste ?? string.Empty,
                EmailConfirmed = true
            };

            var defaultPassword = _configuration["AppSettings:DefaultPassword"] ?? "Password123!";
            var result = await _userManager.CreateAsync(user, defaultPassword);

            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Erreur lors de la création", Errors = result.Errors });
            }

            var role = createUserModel.Role switch
            {
                "Admin" => "Admin",
                "Candidate" => "Candidate",
                "Recruteur" => "Recruteur",
                _ => "Candidate"
            };

            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user); // Rollback si le rôle échoue
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Erreur lors de l'ajout du rôle", Errors = roleResult.Errors });
            }

            return Ok(new ApiResponse<object>
            {
                Data = new { UserId = user.Id, Email = user.Email },
                Success = true,
                Message = $"Utilisateur créé avec succès avec le rôle {role}. Mot de passe par défaut : {defaultPassword}"
            });
        }

        // PUT: api/AppUsers/{id} - Mettre à jour un utilisateur
        [HttpPut("{id}")]
[AllowAnonymous]
        public async Task<IActionResult> PutAppUser(Guid id, [FromBody] UpdateUserModel updatedUser)
        {
            if (id != updatedUser.Id)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "ID mismatch" });
            }

            var existingUser = await _userManager.FindByIdAsync(id.ToString());
            if (existingUser == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Utilisateur non trouvé" });
            }

            // Supprimer la vérification des permissions
            // var currentUser = await _userManager.GetUserAsync(User);
            // if (currentUser == null) { ... }
            // if (currentUser.Id != id && !await _userManager.IsInRoleAsync(currentUser, "Admin")) { ... }

            // Mise à jour des champs
            existingUser.FullName = updatedUser.FullName ?? existingUser.FullName;
            existingUser.Nom = updatedUser.Nom ?? existingUser.Nom;
            existingUser.Prenom = updatedUser.Prenom ?? existingUser.Prenom;
            existingUser.PhoneNumber = updatedUser.PhoneNumber ?? existingUser.PhoneNumber;
            existingUser.Photo = updatedUser.Photo ?? existingUser.Photo;
            existingUser.Entreprise = updatedUser.Entreprise ?? existingUser.Entreprise;
            existingUser.Poste = updatedUser.Poste ?? existingUser.Poste;

            if (!string.IsNullOrEmpty(updatedUser.Email) && updatedUser.Email != existingUser.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(existingUser, updatedUser.Email);
                if (!setEmailResult.Succeeded)
                {
                    return BadRequest(new ApiResponse<object> { Success = false, Message = "Erreur lors de la mise à jour de l'email", Errors = setEmailResult.Errors });
                }
                await _userManager.SetUserNameAsync(existingUser, updatedUser.Email);
            }

            if (!string.IsNullOrEmpty(updatedUser.Role))
            {
                var validRoles = new[] { "Admin", "Candidat", "Recruteur" };
                if (!validRoles.Contains(updatedUser.Role))
                {
                    return BadRequest(new ApiResponse<object> { Success = false, Message = "Rôle invalide" });
                }

                var currentRoles = await _userManager.GetRolesAsync(existingUser);
                var removeRolesResult = await _userManager.RemoveFromRolesAsync(existingUser, currentRoles);
                if (!removeRolesResult.Succeeded)
                {
                    return BadRequest(new ApiResponse<object> { Success = false, Message = "Erreur lors de la suppression des anciens rôles", Errors = removeRolesResult.Errors });
                }

                var addRoleResult = await _userManager.AddToRoleAsync(existingUser, updatedUser.Role);
                if (!addRoleResult.Succeeded)
                {
                    return BadRequest(new ApiResponse<object> { Success = false, Message = "Erreur lors de l'ajout du nouveau rôle", Errors = addRoleResult.Errors });
                }
            }

            var updateResult = await _userManager.UpdateAsync(existingUser);
            if (!updateResult.Succeeded)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Erreur lors de la mise à jour de l'utilisateur", Errors = updateResult.Errors });
            }

            return Ok(new ApiResponse<object> { Success = true, Message = "Utilisateur mis à jour avec succès" });
        }

        
        [HttpDelete("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteAppUser(Guid id)
        {
            var appUser = await _userManager.FindByIdAsync(id.ToString());
            if (appUser == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Utilisateur non trouvé" });
            }

            var result = await _userManager.DeleteAsync(appUser);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Erreur lors de la suppression", Errors = result.Errors });
            }

            return Ok(new ApiResponse<object> { Success = true, Message = "Utilisateur supprimé avec succès" });
        }

        // Modèle de réponse standardisée
        public class ApiResponse<T>
        {
            public T Data { get; set; }
            public bool Success { get; set; }
            public string Message { get; set; }
            public IEnumerable<IdentityError> Errors { get; set; }
        }

        // Modèle pour la création d'utilisateur
        public class CreateUserModel
        {
            public string Email { get; set; }
            public string? FullName { get; set; }
            public string? Nom { get; set; }
            public string? Prenom { get; set; }
            public string? Photo { get; set; }
            public string? PhoneNumber { get; set; }

            public string? Entreprise { get; set; }
            public string? Poste { get; set; }
            public string? Role { get; set; } // "Admin", "Candidat", "Recruteur"
        }

        // Modèle pour la mise à jour d'utilisateur
        public class UpdateUserModel
        {
            public Guid Id { get; set; }
            public string? Email { get; set; }
            public string? FullName { get; set; }
            public string? Nom { get; set; }
            public string? Prenom { get; set; }
            public string? PhoneNumber { get; set; }
            public string? Photo { get; set; }
            public string? Entreprise { get; set; }
            public string? Poste { get; set; }
            public string? Role { get; set; } // "Admin", "Candidat", "Recruteur"
        }
    }
}