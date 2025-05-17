//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Backend_poulina_future_jobs.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.ComponentModel.DataAnnotations;

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
//            var userDtos = new List<object>();

//            foreach (var user in users)
//            {
//                var roles = await _userManager.GetRolesAsync(user);
//                var primaryRole = roles.FirstOrDefault();
//                userDtos.Add(new
//                {
//                    user.Id,
//                    user.UserName,
//                    user.Email,
//                    user.FullName,
//                    user.Nom,
//                    user.Prenom,
//                    user.PhoneNumber,
//                    user.Photo,
//                    user.Entreprise,
//                    user.Poste,
//                    Role = primaryRole
//                });
//            }

//            return Ok(new ApiResponse<object>
//            {
//                Data = userDtos,
//                Success = true,
//                Message = "Liste des utilisateurs récupérée avec succès"
//            });
//        }

//        // GET: api/AppUsers/{id} - Récupérer un utilisateur spécifique
//        [HttpGet("{id}")]
//        [AllowAnonymous]
//        public async Task<ActionResult> GetAppUser(Guid id)
//        {
//            var appUser = await _context.AppUser.FindAsync(id);
//            if (appUser == null)
//            {
//                return NotFound(new ApiResponse<object> { Success = false, Message = "Utilisateur non trouvé" });
//            }

//            var currentUser = await _userManager.GetUserAsync(User);
//            if (currentUser != null && currentUser.Id != id && !await _userManager.IsInRoleAsync(currentUser, "Admin"))
//            {
//                return Forbid();
//            }

//            var roles = await _userManager.GetRolesAsync(appUser);
//            var primaryRole = roles.FirstOrDefault();

//            var userDto = new
//            {
//                appUser.Id,
//                appUser.UserName,
//                appUser.Email,
//                appUser.FullName,
//                appUser.Nom,
//                appUser.Prenom,
//                appUser.PhoneNumber,
//                appUser.Photo,
//                appUser.Entreprise,
//                appUser.Poste,
//                Role = primaryRole
//            };

//            return Ok(new ApiResponse<object>
//            {
//                Data = userDto,
//                Success = true,
//                Message = "Utilisateur récupéré avec succès"
//            });
//        }

//        // GET: api/AppUsers/role/{roleName} - Récupérer les utilisateurs par rôle
//        [HttpGet("role/{roleName}")]
//        [AllowAnonymous]
//        public async Task<IActionResult> GetUsersByRole(string roleName)
//        {
//            var validRoles = new[] { "Admin", "Candidate", "Recruteur" };
//            if (!validRoles.Contains(roleName))
//            {
//                return BadRequest(new ApiResponse<object> { Success = false, Message = "Rôle invalide" });
//            }

//            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
//            if (usersInRole == null || !usersInRole.Any())
//            {
//                return NotFound(new ApiResponse<object> { Success = false, Message = $"Aucun utilisateur trouvé avec le rôle {roleName}" });
//            }

//            var userDtos = new List<object>();
//            foreach (var user in usersInRole)
//            {
//                var roles = await _userManager.GetRolesAsync(user);
//                var primaryRole = roles.FirstOrDefault();
//                userDtos.Add(new
//                {
//                    user.Id,
//                    user.UserName,
//                    user.Email,
//                    user.FullName,
//                    user.Nom,
//                    user.Prenom,
//                    user.PhoneNumber,
//                    user.Photo,
//                    user.Entreprise,
//                    user.Poste,
//                    Role = primaryRole
//                });
//            }

//            return Ok(new ApiResponse<object>
//            {
//                Data = userDtos,
//                Success = true,
//                Message = $"Liste des utilisateurs avec le rôle {roleName} récupérée avec succès"
//            });
//        }

//        // POST: api/AppUsers - Créer un utilisateur
//        [HttpPost]
//        [AllowAnonymous]
//        public async Task<IActionResult> CreateUser([FromBody] CreateUserModel createUserModel)
//        {
//            var validationErrors = new List<string>();

//            if (string.IsNullOrWhiteSpace(createUserModel.Nom))
//                validationErrors.Add("Le nom est requis.");
//            if (string.IsNullOrWhiteSpace(createUserModel.Prenom))
//                validationErrors.Add("Le prénom est requis.");
//            if (string.IsNullOrWhiteSpace(createUserModel.Email))
//                validationErrors.Add("L'email est requis.");
//            if (!string.IsNullOrWhiteSpace(createUserModel.Email) && !new EmailAddressAttribute().IsValid(createUserModel.Email))
//                validationErrors.Add("L'email doit être dans un format valide.");
//            if (string.IsNullOrWhiteSpace(createUserModel.Role))
//                validationErrors.Add("Le rôle est requis.");
//            else
//            {
//                var validRoles = new[] { "Admin", "Candidate", "Recruteur" };
//                if (!validRoles.Contains(createUserModel.Role))
//                    validationErrors.Add("Le rôle doit être 'Admin', 'Candidate' ou 'Recruteur'.");
//            }

//            if (validationErrors.Any())
//            {
//                return BadRequest(new ApiResponse<object>
//                {
//                    Success = false,
//                    Message = "Données invalides",
//                    Errors = validationErrors
//                });
//            }

//            var existingUser = await _userManager.FindByEmailAsync(createUserModel.Email);
//            if (existingUser != null)
//            {
//                return BadRequest(new ApiResponse<object>
//                {
//                    Success = false,
//                    Message = "Cet email est déjà utilisé par un autre utilisateur."
//                });
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
//                PhoneNumber = createUserModel.PhoneNumber ?? string.Empty,
//                Entreprise = createUserModel.Entreprise ?? string.Empty,
//                Poste = createUserModel.Poste ?? string.Empty,
//                EmailConfirmed = true
//            };

//            var defaultPassword = _configuration["AppSettings:DefaultPassword"] ?? "Password123!";
//            var result = await _userManager.CreateAsync(user, defaultPassword);

//            if (!result.Succeeded)
//            {
//                return BadRequest(new ApiResponse<object>
//                {
//                    Success = false,
//                    Message = "Erreur lors de la création",
//                    Errors = result.Errors.Select(e => e.Description)
//                });
//            }

//            var role = createUserModel.Role;
//            var roleResult = await _userManager.AddToRoleAsync(user, role);
//            if (!roleResult.Succeeded)
//            {
//                await _userManager.DeleteAsync(user);
//                return BadRequest(new ApiResponse<object>
//                {
//                    Success = false,
//                    Message = "Erreur lors de l'ajout du rôle",
//                    Errors = roleResult.Errors.Select(e => e.Description)
//                });
//            }

//            return Ok(new ApiResponse<object>
//            {
//                Data = new { UserId = user.Id, Email = user.Email, GeneratedPassword = defaultPassword, Role = role },
//                Success = true,
//                Message = $"Utilisateur créé avec succès avec le rôle {role}. Mot de passe par défaut : {defaultPassword}"
//            });
//        }

//        // PUT: api/AppUsers/{id} - Mettre à jour un utilisateur
//        [HttpPut("{id}")]
//        [AllowAnonymous]

//        public async Task<IActionResult> PutAppUser(Guid id, [FromBody] UpdateUserModel updatedUser)
//        {
//            if (id != updatedUser.Id)
//            {
//                return BadRequest(new ApiResponse<object> { Success = false, Message = "ID mismatch" });
//            }

//            var existingUser = await _userManager.FindByIdAsync(id.ToString());
//            if (existingUser == null)
//            {
//                return NotFound(new ApiResponse<object> { Success = false, Message = "Utilisateur non trouvé" });
//            }

//            var validationErrors = new List<string>();

//            if (string.IsNullOrWhiteSpace(updatedUser.Nom))
//                validationErrors.Add("Le nom est requis.");
//            if (string.IsNullOrWhiteSpace(updatedUser.Prenom))
//                validationErrors.Add("Le prénom est requis.");
//            if (!string.IsNullOrWhiteSpace(updatedUser.Role))
//            {
//                var validRoles = new[] { "Admin", "Candidate", "Recruteur" };
//                if (!validRoles.Contains(updatedUser.Role))
//                    validationErrors.Add("Le rôle doit être 'Admin', 'Candidate' ou 'Recruteur'.");
//            }

//            if (validationErrors.Any())
//            {
//                return BadRequest(new ApiResponse<object>
//                {
//                    Success = false,
//                    Message = "Données invalides",
//                    Errors = validationErrors
//                });
//            }

//            // Ne pas permettre la mise à jour de l'email
//            if (!string.IsNullOrEmpty(updatedUser.Email) && updatedUser.Email != existingUser.Email)
//            {
//                return BadRequest(new ApiResponse<object>
//                {
//                    Success = false,
//                    Message = "La modification de l'email n'est pas autorisée."
//                });
//            }

//            // Mise à jour des autres champs
//            existingUser.FullName = updatedUser.FullName ?? existingUser.FullName;
//            existingUser.Nom = updatedUser.Nom ?? existingUser.Nom;
//            existingUser.Prenom = updatedUser.Prenom ?? existingUser.Prenom;
//            existingUser.PhoneNumber = updatedUser.PhoneNumber ?? existingUser.PhoneNumber;
//            existingUser.Photo = updatedUser.Photo ?? existingUser.Photo;
//            existingUser.Entreprise = updatedUser.Entreprise ?? existingUser.Entreprise;
//            existingUser.Poste = updatedUser.Poste ?? existingUser.Poste;

//            if (!string.IsNullOrEmpty(updatedUser.Role))
//            {
//                var currentRoles = await _userManager.GetRolesAsync(existingUser);
//                var removeRolesResult = await _userManager.RemoveFromRolesAsync(existingUser, currentRoles);
//                if (!removeRolesResult.Succeeded)
//                {
//                    return BadRequest(new ApiResponse<object>
//                    {
//                        Success = false,
//                        Message = "Erreur lors de la suppression des anciens rôles",
//                        Errors = removeRolesResult.Errors.Select(e => e.Description)
//                    });
//                }

//                var addRoleResult = await _userManager.AddToRoleAsync(existingUser, updatedUser.Role);
//                if (!addRoleResult.Succeeded)
//                {
//                    return BadRequest(new ApiResponse<object>
//                    {
//                        Success = false,
//                        Message = "Erreur lors de l'ajout du nouveau rôle",
//                        Errors = addRoleResult.Errors.Select(e => e.Description)
//                    });
//                }
//            }

//            var updateResult = await _userManager.UpdateAsync(existingUser);
//            if (!updateResult.Succeeded)
//            {
//                return BadRequest(new ApiResponse<object>
//                {
//                    Success = false,
//                    Message = "Erreur lors de la mise à jour de l'utilisateur",
//                    Errors = updateResult.Errors.Select(e => e.Description)
//                });
//            }

//            return Ok(new ApiResponse<object> { Success = true, Message = "Utilisateur mis à jour avec succès" });
//        }
//        //public async Task<IActionResult> PutAppUser(Guid id, [FromBody] UpdateUserModel updatedUser)
//        //{
//        //    if (id != updatedUser.Id)
//        //    {
//        //        return BadRequest(new ApiResponse<object> { Success = false, Message = "ID mismatch" });
//        //    }

//        //    var existingUser = await _userManager.FindByIdAsync(id.ToString());
//        //    if (existingUser == null)
//        //    {
//        //        return NotFound(new ApiResponse<object> { Success = false, Message = "Utilisateur non trouvé" });
//        //    }

//        //    var validationErrors = new List<string>();

//        //    if (string.IsNullOrWhiteSpace(updatedUser.Nom))
//        //        validationErrors.Add("Le nom est requis.");
//        //    if (string.IsNullOrWhiteSpace(updatedUser.Prenom))
//        //        validationErrors.Add("Le prénom est requis.");
//        //    if (string.IsNullOrWhiteSpace(updatedUser.Email))
//        //        validationErrors.Add("L'email est requis.");
//        //    if (!string.IsNullOrWhiteSpace(updatedUser.Email) && !new EmailAddressAttribute().IsValid(updatedUser.Email))
//        //        validationErrors.Add("L'email doit être dans un format valide.");
//        //    if (!string.IsNullOrWhiteSpace(updatedUser.Role))
//        //    {
//        //        var validRoles = new[] { "Admin", "Candidate", "Recruteur" };
//        //        if (!validRoles.Contains(updatedUser.Role))
//        //            validationErrors.Add("Le rôle doit être 'Admin', 'Candidate' ou 'Recruteur'.");
//        //    }

//        //    if (validationErrors.Any())
//        //    {
//        //        return BadRequest(new ApiResponse<object>
//        //        {
//        //            Success = false,
//        //            Message = "Données invalides",
//        //            Errors = validationErrors
//        //        });
//        //    }

//        //    if (!string.IsNullOrEmpty(updatedUser.Email) && updatedUser.Email != existingUser.Email)
//        //    {
//        //        var emailExists = await _userManager.FindByEmailAsync(updatedUser.Email);
//        //        if (emailExists != null && emailExists.Id != existingUser.Id)
//        //        {
//        //            return BadRequest(new ApiResponse<object>
//        //            {
//        //                Success = false,
//        //                Message = "Cet email est déjà utilisé par un autre utilisateur."
//        //            });
//        //        }

//        //        var setEmailResult = await _userManager.SetEmailAsync(existingUser, updatedUser.Email);
//        //        if (!setEmailResult.Succeeded)
//        //        {
//        //            return BadRequest(new ApiResponse<object>
//        //            {
//        //                Success = false,
//        //                Message = "Erreur lors de la mise à jour de l'email",
//        //                Errors = setEmailResult.Errors.Select(e => e.Description)
//        //            });
//        //        }
//        //        await _userManager.SetUserNameAsync(existingUser, updatedUser.Email);
//        //    }

//        //    existingUser.FullName = updatedUser.FullName ?? existingUser.FullName;
//        //    existingUser.Nom = updatedUser.Nom ?? existingUser.Nom;
//        //    existingUser.Prenom = updatedUser.Prenom ?? existingUser.Prenom;
//        //    existingUser.PhoneNumber = updatedUser.PhoneNumber ?? existingUser.PhoneNumber;
//        //    existingUser.Photo = updatedUser.Photo ?? existingUser.Photo;
//        //    existingUser.Entreprise = updatedUser.Entreprise ?? existingUser.Entreprise;
//        //    existingUser.Poste = updatedUser.Poste ?? existingUser.Poste;

//        //    if (!string.IsNullOrEmpty(updatedUser.Role))
//        //    {
//        //        var currentRoles = await _userManager.GetRolesAsync(existingUser);
//        //        var removeRolesResult = await _userManager.RemoveFromRolesAsync(existingUser, currentRoles);
//        //        if (!removeRolesResult.Succeeded)
//        //        {
//        //            return BadRequest(new ApiResponse<object>
//        //            {
//        //                Success = false,
//        //                Message = "Erreur lors de la suppression des anciens rôles",
//        //                Errors = removeRolesResult.Errors.Select(e => e.Description)
//        //            });
//        //        }

//        //        var addRoleResult = await _userManager.AddToRoleAsync(existingUser, updatedUser.Role);
//        //        if (!addRoleResult.Succeeded)
//        //        {
//        //            return BadRequest(new ApiResponse<object>
//        //            {
//        //                Success = false,
//        //                Message = "Erreur lors de l'ajout du nouveau rôle",
//        //                Errors = addRoleResult.Errors.Select(e => e.Description)
//        //            });
//        //        }
//        //    }

//        //    var updateResult = await _userManager.UpdateAsync(existingUser);
//        //    if (!updateResult.Succeeded)
//        //    {
//        //        return BadRequest(new ApiResponse<object>
//        //        {
//        //            Success = false,
//        //            Message = "Erreur lors de la mise à jour de l'utilisateur",
//        //            Errors = updateResult.Errors.Select(e => e.Description)
//        //        });
//        //    }

//        //    return Ok(new ApiResponse<object> { Success = true, Message = "Utilisateur mis à jour avec succès" });
//        //}

//        // DELETE: api/AppUsers/{id} - Supprimer un utilisateur
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
//                return BadRequest(new ApiResponse<object>
//                {
//                    Success = false,
//                    Message = "Erreur lors de la suppression",
//                    Errors = result.Errors.Select(e => e.Description)
//                });
//            }

//            return Ok(new ApiResponse<object> { Success = true, Message = "Utilisateur supprimé avec succès" });
//        }

//        // GET: api/AppUsers/check-email - Vérifier si un email existe
//        [HttpGet("check-email")]
//        [AllowAnonymous]
//        public async Task<IActionResult> CheckEmail(string email)
//        {
//            if (string.IsNullOrWhiteSpace(email))
//            {
//                return BadRequest(new ApiResponse<object> { Success = false, Message = "Email requis" });
//            }

//            var user = await _userManager.FindByEmailAsync(email);
//            return Ok(new { exists = user != null });
//        }

//        [HttpGet("recruteurs")]
//        [AllowAnonymous]
//        public async Task<ActionResult<object>> GetRecruteurs()
//        {
//            // Use UserManager to fetch users in the "Recruteur" role
//            var recruteurs = await _userManager.GetUsersInRoleAsync("Recruteur");

//            if (recruteurs == null || !recruteurs.Any())
//            {
//                return NotFound(new ApiResponse<object>
//                {
//                    Success = false,
//                    Message = "Aucun recruteur trouvé"
//                });
//            }

//            // Select only the necessary fields (Id and UserName)
//            var recruteurDtos = recruteurs.Select(u => new
//            {
//                Id = u.Id,
//                Username = u.UserName,// Guid from AppUser
//                fullaName = u.FullName  // Consistent with Identity's UserName property
//            }).ToList();

//            return Ok(new ApiResponse<object>
//            {
//                Success = true,
//                Message = "Recruteurs récupérés avec succès",
//                Data = recruteurDtos
//            });
//        }

//        // Modèle de réponse standardisée
//        public class ApiResponse<T>
//        {
//            public T Data { get; set; }
//            public bool Success { get; set; }
//            public string Message { get; set; }
//            public IEnumerable<string> Errors { get; set; }
//        }

//        // Modèle pour la création d'utilisateur
//        public class CreateUserModel
//        {
//            public string Email { get; set; }
//            public string? FullName { get; set; }
//            public string? Nom { get; set; }
//            public string? Prenom { get; set; }
//            public string? Photo { get; set; }
//            public string? PhoneNumber { get; set; }
//            public string? Entreprise { get; set; }
//            public string? Poste { get; set; }
//            public string? Role { get; set; } // "Admin", "Candidat", "Recruteur"
//        }

//        // Modèle pour la mise à jour d'utilisateur
//        public class UpdateUserModel
//        {
//            public Guid Id { get; set; }
//            public string? Email { get; set; }
//            public string? FullName { get; set; }
//            public string? Nom { get; set; }
//            public string? Prenom { get; set; }
//            public string? PhoneNumber { get; set; }
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
using System.ComponentModel.DataAnnotations;

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
            var users = await _context.AppUser.Include(u => u.Filiale).ToListAsync();
            var userDtos = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var primaryRole = roles.FirstOrDefault();
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
                    user.Poste,
                    IdFiliale = user.IdFiliale,
                    NomFiliale = user.Filiale?.Nom,
                    Role = primaryRole
                });
            }

            return Ok(new ApiResponse<object>
            {
                Data = userDtos,
                Success = true,
                Message = "Liste des utilisateurs récupérée avec succès"
            });
        }

        // GET: api/AppUsers/{id} - Récupérer un utilisateur spécifique
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetAppUser(Guid id)
        {
            var appUser = await _context.AppUser.Include(u => u.Filiale).FirstOrDefaultAsync(u => u.Id == id);
            if (appUser == null)
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = "Utilisateur non trouvé" });
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null && currentUser.Id != id && !await _userManager.IsInRoleAsync(currentUser, "Admin"))
            {
                return Forbid();
            }

            var roles = await _userManager.GetRolesAsync(appUser);
            var primaryRole = roles.FirstOrDefault();

            var userDto = new
            {
                appUser.Id,
                appUser.UserName,
                appUser.Email,
                appUser.FullName,
                appUser.Nom,
                appUser.Prenom,
                appUser.PhoneNumber,
                appUser.Photo,
                appUser.Poste,
                IdFiliale = appUser.IdFiliale,
                NomFiliale = appUser.Filiale?.Nom,
                Role = primaryRole
            };

            return Ok(new ApiResponse<object>
            {
                Data = userDto,
                Success = true,
                Message = "Utilisateur récupéré avec succès"
            });
        }

        // GET: api/AppUsers/role/{roleName} - Récupérer les utilisateurs par rôle
        [HttpGet("role/{roleName}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUsersByRole(string roleName)
        {
            var validRoles = new[] { "Admin", "Candidate", "Recruteur" };
            if (!validRoles.Contains(roleName))
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Rôle invalide" });
            }

            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName);
            if (usersInRole == null || !usersInRole.Any())
            {
                return NotFound(new ApiResponse<object> { Success = false, Message = $"Aucun utilisateur trouvé avec le rôle {roleName}" });
            }

            var userDtos = new List<object>();
            foreach (var user in usersInRole)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var primaryRole = roles.FirstOrDefault();
                var filiale = await _context.Filiales.FindAsync(user.IdFiliale);
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
                    user.Poste,
                    IdFiliale = user.IdFiliale,
                    NomFiliale = filiale?.Nom,
                    Role = primaryRole
                });
            }

            return Ok(new ApiResponse<object>
            {
                Data = userDtos,
                Success = true,
                Message = $"Liste des utilisateurs avec le rôle {roleName} récupérée avec succès"
            });
        }

        // POST: api/AppUsers - Créer un utilisateur
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserModel createUserModel)
        {
            var validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(createUserModel.Nom))
                validationErrors.Add("Le nom est requis.");
            if (string.IsNullOrWhiteSpace(createUserModel.Prenom))
                validationErrors.Add("Le prénom est requis.");
            if (string.IsNullOrWhiteSpace(createUserModel.Email))
                validationErrors.Add("L'email est requis.");
            if (!string.IsNullOrWhiteSpace(createUserModel.Email) && !new EmailAddressAttribute().IsValid(createUserModel.Email))
                validationErrors.Add("L'email doit être dans un format valide.");
            if (string.IsNullOrWhiteSpace(createUserModel.Role))
                validationErrors.Add("Le rôle est requis.");
            else
            {
                var validRoles = new[] { "Admin", "Candidate", "Recruteur" };
                if (!validRoles.Contains(createUserModel.Role))
                    validationErrors.Add("Le rôle doit être 'Admin', 'Candidate' ou 'Recruteur'.");
            }
            if (createUserModel.IdFiliale.HasValue)
            {
                var filialeExists = await _context.Filiales.AnyAsync(f => f.IdFiliale == createUserModel.IdFiliale);
                if (!filialeExists)
                    validationErrors.Add("La filiale spécifiée n'existe pas.");
            }

            if (validationErrors.Any())
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Données invalides",
                    Errors = validationErrors
                });
            }

            var existingUser = await _userManager.FindByEmailAsync(createUserModel.Email);
            if (existingUser != null)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Cet email est déjà utilisé par un autre utilisateur."
                });
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
                Poste = createUserModel.Poste ?? string.Empty,
                IdFiliale = createUserModel.IdFiliale,
                EmailConfirmed = true
            };

            var defaultPassword = _configuration["AppSettings:DefaultPassword"] ?? "Password123!";
            var result = await _userManager.CreateAsync(user, defaultPassword);

            if (!result.Succeeded)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Erreur lors de la création",
                    Errors = result.Errors.Select(e => e.Description)
                });
            }

            var role = createUserModel.Role;
            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Erreur lors de l'ajout du rôle",
                    Errors = roleResult.Errors.Select(e => e.Description)
                });
            }

            return Ok(new ApiResponse<object>
            {
                Data = new { UserId = user.Id, Email = user.Email, GeneratedPassword = defaultPassword, Role = role, IdFiliale = user.IdFiliale },
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

            var validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(updatedUser.Nom))
                validationErrors.Add("Le nom est requis.");
            if (string.IsNullOrWhiteSpace(updatedUser.Prenom))
                validationErrors.Add("Le prénom est requis.");
            if (!string.IsNullOrWhiteSpace(updatedUser.Role))
            {
                var validRoles = new[] { "Admin", "Candidate", "Recruteur" };
                if (!validRoles.Contains(updatedUser.Role))
                    validationErrors.Add("Le rôle doit être 'Admin', 'Candidate' ou 'Recruteur'.");
            }
            if (updatedUser.IdFiliale.HasValue)
            {
                var filialeExists = await _context.Filiales.AnyAsync(f => f.IdFiliale == updatedUser.IdFiliale);
                if (!filialeExists)
                    validationErrors.Add("La filiale spécifiée n'existe pas.");
            }

            if (validationErrors.Any())
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Données invalides",
                    Errors = validationErrors
                });
            }

            // Ne pas permettre la mise à jour de l'email
            if (!string.IsNullOrEmpty(updatedUser.Email) && updatedUser.Email != existingUser.Email)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "La modification de l'email n'est pas autorisée."
                });
            }

            // Mise à jour des champs
            existingUser.FullName = updatedUser.FullName ?? existingUser.FullName;
            existingUser.Nom = updatedUser.Nom ?? existingUser.Nom;
            existingUser.Prenom = updatedUser.Prenom ?? existingUser.Prenom;
            existingUser.PhoneNumber = updatedUser.PhoneNumber ?? existingUser.PhoneNumber;
            existingUser.Photo = updatedUser.Photo ?? existingUser.Photo;
            existingUser.Poste = updatedUser.Poste ?? existingUser.Poste;
            existingUser.IdFiliale = updatedUser.IdFiliale ?? existingUser.IdFiliale;

            if (!string.IsNullOrEmpty(updatedUser.Role))
            {
                var currentRoles = await _userManager.GetRolesAsync(existingUser);
                var removeRolesResult = await _userManager.RemoveFromRolesAsync(existingUser, currentRoles);
                if (!removeRolesResult.Succeeded)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Erreur lors de la suppression des anciens rôles",
                        Errors = removeRolesResult.Errors.Select(e => e.Description)
                    });
                }

                var addRoleResult = await _userManager.AddToRoleAsync(existingUser, updatedUser.Role);
                if (!addRoleResult.Succeeded)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Erreur lors de l'ajout du nouveau rôle",
                        Errors = addRoleResult.Errors.Select(e => e.Description)
                    });
                }
            }

            var updateResult = await _userManager.UpdateAsync(existingUser);
            if (!updateResult.Succeeded)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Erreur lors de la mise à jour de l'utilisateur",
                    Errors = updateResult.Errors.Select(e => e.Description)
                });
            }

            return Ok(new ApiResponse<object> { Success = true, Message = "Utilisateur mis à jour avec succès" });
        }

         
        // DELETE: api/AppUsers/{id} - Supprimer un utilisateur
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
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Erreur lors de la suppression",
                    Errors = result.Errors.Select(e => e.Description)
                });
            }

            return Ok(new ApiResponse<object> { Success = true, Message = "Utilisateur supprimé avec succès" });
        }

        // GET: api/AppUsers/check-email - Vérifier si un email existe
        [HttpGet("check-email")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new ApiResponse<object> { Success = false, Message = "Email requis" });
            }

            var user = await _userManager.FindByEmailAsync(email);
            return Ok(new { exists = user != null });
        }

        // GET: api/AppUsers/recruteurs - Récupérer les recruteurs
        [HttpGet("recruteurs")]
        [AllowAnonymous]
        public async Task<ActionResult<object>> GetRecruteurs()
        {
            var recruteurs = await _userManager.GetUsersInRoleAsync("Recruteur");

            if (recruteurs == null || !recruteurs.Any())
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Aucun recruteur trouvé"
                });
            }

            var recruteurDtos = recruteurs.Select(u => new
            {
                Id = u.Id,
                Username = u.UserName,
                FullName = u.FullName
            }).ToList();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Recruteurs récupérés avec succès",
                Data = recruteurDtos
            });
        }

        // Modèle de réponse standardisée
        public class ApiResponse<T>
        {
            public T Data { get; set; }
            public bool Success { get; set; }
            public string Message { get; set; }
            public IEnumerable<string> Errors { get; set; }
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
            public string? Poste { get; set; }
            public string? Role { get; set; } // "Admin", "Candidate", "Recruteur"
            public Guid? IdFiliale { get; set; } // Relation avec Filiale
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
            public string? Poste { get; set; }
            public string? Role { get; set; } // "Admin", "Candidate", "Recruteur"
            public Guid? IdFiliale { get; set; } // Relation avec Filiale
        }
    }
}