//using Backend_poulina_future_jobs.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Security.Claims;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Logging;
//using System.ComponentModel.DataAnnotations;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;
//using Microsoft.AspNetCore.Http;
//using System.Text.Json;
//using Microsoft.IdentityModel.Tokens;
//using System.Collections.Generic;

//namespace Backend_poulina_future_jobs.Controllers
//{
//    [Authorize]
//    public static class AccountEndpoints
//    {
//        public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
//        {
//            app.MapGet("/api/account/profile", GetUserProfile);
//            app.MapPut("/api/account/profile", EditUserProfile);
//            app.MapPost("/api/account/assign-role", AssignRole);

//            return app;
//        }

//        private static async Task<dynamic> GetUserDetails(UserManager<AppUser> userManager, string userId)
//        {
//            if (string.IsNullOrEmpty(userId))
//                throw new ArgumentNullException(nameof(userId));

//            return await userManager.Users
//                .Include(u => u.Filiale)
//                .Where(u => u.Id.ToString() == userId)
//                .Select(u => new
//                {
//                    u.Email,
//                    u.Nom,
//                    u.Prenom,
//                    u.PhoneNumber,
//                    u.Adresse,
//                    u.Ville,
//                    u.Pays,
//                    u.Photo,
//                    u.DateNaissance,
//                    Filiale = u.Filiale != null ? new { u.Filiale.IdFiliale, u.Filiale.Nom } : null
//                })
//                .FirstOrDefaultAsync();
//        }

//        [Authorize]
//        private static async Task<IResult> GetUserProfile(
//            HttpContext context,
//            UserManager<AppUser> userManager,
//            ILoggerFactory loggerFactory)
//        {
//            var logger = loggerFactory.CreateLogger("AccountEndpoints.GetUserProfile");
//            try
//            {
//                // Enhanced token debugging
//                var authHeader = context.Request.Headers["Authorization"].ToString();
//                logger.LogInformation("Authorization Header: {AuthHeader}", authHeader);

//                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
//                {
//                    logger.LogWarning("Missing or invalid Authorization header");
//                    return Results.Unauthorized();
//                }

//                // Extract token for debugging
//                var token = authHeader.Substring("Bearer ".Length).Trim();
//                logger.LogDebug("JWT Token: {Token}", token);

//                // Log all claims
//                var claims = context.User.Claims
//                    .Select(c => new { Type = c.Type, Value = c.Value })
//                    .ToList();

//                logger.LogInformation("User claims: {@Claims}", claims);

//                // Try multiple claim types
//                string userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
//                                context.User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
//                                context.User.FindFirstValue("sub") ??
//                                context.User.FindFirstValue("uid") ??
//                                context.User.FindFirstValue("userId") ??
//                                context.User.FindFirstValue("nameid");

//                if (string.IsNullOrEmpty(userId))
//                {
//                    logger.LogWarning("User ID not found in token. Available claims: {@Claims}", claims);
//                    return Results.Unauthorized();
//                }

//                logger.LogInformation("Extracted User ID: {UserId}", userId);

//                var userDetails = await GetUserDetails(userManager, userId);
//                if (userDetails == null)
//                {
//                    logger.LogWarning("User not found for ID: {UserId}", userId);
//                    return Results.NotFound(new { message = "Utilisateur introuvable." });
//                }

//                // Construire l'URL complète de la photo
//                var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
//                var photoUrl = !string.IsNullOrEmpty(userDetails.Photo)
//                    ? $"{baseUrl}/api/FileUpload/files/{userDetails.Photo}"
//                    : null;

//                return Results.Ok(new
//                {
//                    userDetails.Email,
//                    userDetails.Nom,
//                    userDetails.Prenom,
//                    userDetails.PhoneNumber,
//                    userDetails.Adresse,
//                    userDetails.Ville,
//                    userDetails.Pays,
//                    Photo = userDetails.Photo,   // Chemin relatif
//                    PhotoUrl = photoUrl,          // URL complète
//                    userDetails.DateNaissance,
//                    userDetails.Filiale
//                });
//            }
//            catch (Exception ex)
//            {
//                logger.LogError(ex, "Error in GetUserProfile");
//                return Results.Problem(detail: "Une erreur est survenue lors du traitement de votre demande.", statusCode: 500);
//            }
//        }

//        [Authorize]
//        private static async Task<IResult> EditUserProfile(
//            HttpContext context,
//            UserManager<AppUser> userManager,
//            ILoggerFactory loggerFactory,
//            [FromBody] EditProfileRequest model)
//        {
//            var logger = loggerFactory.CreateLogger("AccountEndpoints.EditUserProfile");
//            try
//            {
//                // Extract user ID with multiple fallbacks
//                string userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
//                                context.User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
//                                context.User.FindFirstValue("sub") ??
//                                context.User.FindFirstValue("uid") ??
//                                context.User.FindFirstValue("userId") ??
//                                context.User.FindFirstValue("nameid");

//                if (string.IsNullOrEmpty(userId))
//                {
//                    logger.LogWarning("User ID not found in token");
//                    return Results.Unauthorized();
//                }

//                logger.LogInformation("Editing profile for User ID: {UserId}", userId);

//                var userDetails = await userManager.FindByIdAsync(userId);
//                if (userDetails == null)
//                {
//                    logger.LogWarning("User not found for ID: {UserId}", userId);
//                    return Results.NotFound(new { message = "Utilisateur introuvable." });
//                }

//                // Validation checks remain the same
//                if (string.IsNullOrEmpty(model.Nom) || string.IsNullOrEmpty(model.Prenom) ||
//                    string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.PhoneNumber) ||
//                    string.IsNullOrEmpty(model.Adresse) || string.IsNullOrEmpty(model.Ville) ||
//                    string.IsNullOrEmpty(model.Pays))
//                {
//                    return Results.BadRequest(new { message = "Tous les champs obligatoires doivent être remplis." });
//                }

//                if (await userManager.IsInRoleAsync(userDetails, "Recruteur"))
//                {
//                    if (!model.IdFiliale.HasValue)
//                    {
//                        return Results.BadRequest(new { message = "IdFiliale est obligatoire pour un Recruteur." });
//                    }
//                    if (!model.DateNaissance.HasValue)
//                    {
//                        return Results.BadRequest(new { message = "DateNaissance est obligatoire pour un Recruteur." });
//                    }
//                }

//                if (await userManager.IsInRoleAsync(userDetails, "Admin") && !model.DateNaissance.HasValue)
//                {
//                    return Results.BadRequest(new { message = "DateNaissance est obligatoire pour un Admin." });
//                }

//                // Update properties
//                userDetails.Nom = model.Nom;
//                userDetails.Prenom = model.Prenom;
//                userDetails.Email = model.Email;
//                userDetails.PhoneNumber = model.PhoneNumber;
//                userDetails.Adresse = model.Adresse;
//                userDetails.Ville = model.Ville;
//                userDetails.Pays = model.Pays;
//                userDetails.IdFiliale = model.IdFiliale;
//                userDetails.DateNaissance = model.DateNaissance;

//                // Mettre à jour la photo si fournie
//                if (!string.IsNullOrEmpty(model.Photo))
//                {
//                    userDetails.Photo = model.Photo;
//                }

//                var result = await userManager.UpdateAsync(userDetails);
//                if (!result.Succeeded)
//                {
//                    logger.LogWarning("Failed to update user profile: {Errors}",
//                        string.Join(", ", result.Errors.Select(e => e.Description)));

//                    return Results.BadRequest(new
//                    {
//                        message = "Erreur lors de la mise à jour du profil",
//                        errors = result.Errors.Select(e => e.Description)
//                    });
//                }

//                return Results.Ok(new { message = "Profil mis à jour avec succès." });
//            }
//            catch (Exception ex)
//            {
//                logger.LogError(ex, "Error in EditUserProfile for user");
//                return Results.Problem(detail: "Une erreur est survenue lors de la mise à jour du profil.", statusCode: 500);
//            }
//        }

//        [Authorize(Roles = "Admin")]
//        private static async Task<IResult> AssignRole(
//            HttpContext context,
//            UserManager<AppUser> userManager,
//            ILoggerFactory loggerFactory,
//            [FromBody] AssignRoleRequest model)
//        {
//            var logger = loggerFactory.CreateLogger("AccountEndpoints.AssignRole");
//            try
//            {
//                // Extract admin ID with multiple fallbacks
//                string adminId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
//                                 context.User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
//                                 context.User.FindFirstValue("sub") ??
//                                 context.User.FindFirstValue("uid") ??
//                                 context.User.FindFirstValue("userId") ??
//                                 context.User.FindFirstValue("nameid");

//                if (string.IsNullOrEmpty(adminId))
//                {
//                    logger.LogWarning("Admin ID not found in token");
//                    return Results.Unauthorized();
//                }

//                logger.LogInformation("Admin action by User ID: {AdminId}", adminId);

//                var targetUser = await userManager.FindByIdAsync(model.UserId.ToString());
//                if (targetUser == null)
//                {
//                    logger.LogWarning("Target user not found for ID: {UserId}", model.UserId);
//                    return Results.NotFound(new { message = "Utilisateur cible introuvable." });
//                }

//                if (string.IsNullOrEmpty(model.Role))
//                {
//                    return Results.BadRequest(new { message = "Le rôle est obligatoire." });
//                }

//                var validRoles = new[] { "Candidat", "Recruteur", "Admin" };
//                if (!validRoles.Contains(model.Role))
//                {
//                    return Results.BadRequest(new
//                    {
//                        message = "Rôle invalide. Les rôles autorisés sont : Candidat, Recruteur, Admin."
//                    });
//                }

//                var currentRoles = await userManager.GetRolesAsync(targetUser);
//                await userManager.RemoveFromRolesAsync(targetUser, currentRoles);
//                var result = await userManager.AddToRoleAsync(targetUser, model.Role);

//                if (!result.Succeeded)
//                {
//                    logger.LogWarning("Failed to assign role: {Errors}",
//                        string.Join(", ", result.Errors.Select(e => e.Description)));

//                    return Results.BadRequest(new
//                    {
//                        message = "Erreur lors de l'attribution du rôle",
//                        errors = result.Errors.Select(e => e.Description)
//                    });
//                }

//                return Results.Ok(new { message = $"Rôle {model.Role} attribué avec succès à l'utilisateur." });
//            }
//            catch (Exception ex)
//            {
//                logger.LogError(ex, "Error in AssignRole for admin");
//                return Results.Problem(detail: "Une erreur est survenue lors de l'attribution du rôle.", statusCode: 500);
//            }
//        }

//        public class EditProfileRequest
//        {
//            [Required] public string Nom { get; set; }
//            [Required] public string Prenom { get; set; }
//            [Required, EmailAddress] public string Email { get; set; }
//            [Required, Phone] public string PhoneNumber { get; set; }
//            [Required] public string Adresse { get; set; }
//            [Required] public string Ville { get; set; }
//            [Required] public string Pays { get; set; }
//            public Guid? IdFiliale { get; set; }
//            public DateTime? DateNaissance { get; set; }
//            public string Photo { get; set; } // Nouvelle propriété pour la photo
//        }

//        public class AssignRoleRequest
//        {
//            public Guid UserId { get; set; }
//            public string Role { get; set; }
//        }
//    }
//}
using Backend_poulina_future_jobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace Backend_poulina_future_jobs.Controllers
{
    [Authorize]
    public static class AccountEndpoints
    {
        public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/api/account/profile", GetUserProfile);
            app.MapPut("/api/account/profile", EditUserProfile);
            app.MapPost("/api/account/assign-role", AssignRole);

            return app;
        }

        private static async Task<dynamic> GetUserDetails(UserManager<AppUser> userManager, string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException(nameof(userId));

            return await userManager.Users
                .Include(u => u.Filiale)
                .Where(u => u.Id.ToString() == userId)
                .Select(u => new
                {
                    u.Email,
                    u.Nom,
                    u.Prenom,
                    u.PhoneNumber,
                    u.Adresse,
                    u.Ville,
                    u.Pays,
                    u.Photo,
                    u.DateNaissance,
                    Filiale = u.Filiale != null ? new { u.Filiale.IdFiliale, u.Filiale.Nom } : null
                })
                .FirstOrDefaultAsync();
        }

        [Authorize]
        private static async Task<IResult> GetUserProfile(
            HttpContext context,
            UserManager<AppUser> userManager,
            ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger("AccountEndpoints.GetUserProfile");
            try
            {
                // Enhanced token debugging
                var authHeader = context.Request.Headers["Authorization"].ToString();
                logger.LogInformation("Authorization Header: {AuthHeader}", authHeader);

                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    logger.LogWarning("Missing or invalid Authorization header");
                    return Results.Unauthorized();
                }

                // Extract token for debugging
                var token = authHeader.Substring("Bearer ".Length).Trim();
                logger.LogDebug("JWT Token: {Token}", token);

                // Log all claims
                var claims = context.User.Claims
                    .Select(c => new { Type = c.Type, Value = c.Value })
                    .ToList();

                logger.LogInformation("User claims: {@Claims}", claims);

                // Try multiple claim types
                string userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                                context.User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                                context.User.FindFirstValue("sub") ??
                                context.User.FindFirstValue("uid") ??
                                context.User.FindFirstValue("userId") ??
                                context.User.FindFirstValue("nameid");

                if (string.IsNullOrEmpty(userId))
                {
                    logger.LogWarning("User ID not found in token. Available claims: {@Claims}", claims);
                    return Results.Unauthorized();
                }

                logger.LogInformation("Extracted User ID: {UserId}", userId);

                var userDetails = await GetUserDetails(userManager, userId);
                if (userDetails == null)
                {
                    logger.LogWarning("User not found for ID: {UserId}", userId);
                    return Results.NotFound(new { message = "Utilisateur introuvable." });
                }

                // Construire l'URL complète de la photo
                var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
                var photoUrl = !string.IsNullOrEmpty(userDetails.Photo)
                    ? $"{baseUrl}/api/FileUpload/files/{userDetails.Photo}"
                    : null;

                return Results.Ok(new
                {
                    userDetails.Email,
                    userDetails.Nom,
                    userDetails.Prenom,
                    userDetails.PhoneNumber,
                    userDetails.Adresse,
                    userDetails.Ville,
                    userDetails.Pays,
                    Photo = userDetails.Photo,
                    PhotoUrl = photoUrl,
                    userDetails.DateNaissance,
                    userDetails.Filiale
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetUserProfile");
                return Results.Problem(detail: "Une erreur est survenue lors du traitement de votre demande.", statusCode: 500);
            }
        }

        [Authorize]
        private static async Task<IResult> EditUserProfile(
            HttpContext context,
            UserManager<AppUser> userManager,
            ILoggerFactory loggerFactory,
            [FromBody] EditProfileRequest model)
        {
            var logger = loggerFactory.CreateLogger("AccountEndpoints.EditUserProfile");
            try
            {
                // Extract user ID with multiple fallbacks
                string userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                                context.User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                                context.User.FindFirstValue("sub") ??
                                context.User.FindFirstValue("uid") ??
                                context.User.FindFirstValue("userId") ??
                                context.User.FindFirstValue("nameid");

                if (string.IsNullOrEmpty(userId))
                {
                    logger.LogWarning("User ID not found in token");
                    return Results.Unauthorized();
                }

                logger.LogInformation("Editing profile for User ID: {UserId}", userId);

                var userDetails = await userManager.FindByIdAsync(userId);
                if (userDetails == null)
                {
                    logger.LogWarning("User not found for ID: {UserId}", userId);
                    return Results.NotFound(new { message = "Utilisateur introuvable." });
                }

                // Validation checks for required fields
                if (string.IsNullOrEmpty(model.Nom) || string.IsNullOrEmpty(model.Prenom) ||
                    string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.PhoneNumber) ||
                    string.IsNullOrEmpty(model.Adresse) || string.IsNullOrEmpty(model.Ville) ||
                    string.IsNullOrEmpty(model.Pays))
                {
                    return Results.BadRequest(new { message = "Tous les champs obligatoires doivent être remplis." });
                }

                if (await userManager.IsInRoleAsync(userDetails, "Recruteur"))
                {
                    if (!model.IdFiliale.HasValue)
                    {
                        return Results.BadRequest(new { message = "IdFiliale est obligatoire pour un Recruteur." });
                    }
                    if (!model.DateNaissance.HasValue)
                    {
                        return Results.BadRequest(new { message = "DateNaissance est obligatoire pour un Recruteur." });
                    }
                }

                if (await userManager.IsInRoleAsync(userDetails, "Admin") && !model.DateNaissance.HasValue)
                {
                    return Results.BadRequest(new { message = "DateNaissance est obligatoire pour un Admin." });
                }

                // Update properties
                userDetails.Nom = model.Nom;
                userDetails.Prenom = model.Prenom;
                userDetails.Email = model.Email;
                userDetails.PhoneNumber = model.PhoneNumber;
                userDetails.Adresse = model.Adresse;
                userDetails.Ville = model.Ville;
                userDetails.Pays = model.Pays;
                userDetails.IdFiliale = model.IdFiliale;
                userDetails.DateNaissance = model.DateNaissance;

                // Update photo if provided
                if (!string.IsNullOrEmpty(model.Photo))
                {
                    userDetails.Photo = model.Photo;
                }

                // Handle password update if provided
                if (!string.IsNullOrEmpty(model.CurrentPassword) && !string.IsNullOrEmpty(model.NewPassword))
                {
                    // Validate current password
                    var passwordCheck = await userManager.CheckPasswordAsync(userDetails, model.CurrentPassword);
                    if (!passwordCheck)
                    {
                        logger.LogWarning("Invalid current password for User ID: {UserId}", userId);
                        return Results.BadRequest(new { message = "Mot de passe actuel incorrect." });
                    }

                    // Validate new password
                    if (model.NewPassword.Length < 6)
                    {
                        return Results.BadRequest(new { message = "Le nouveau mot de passe doit contenir au moins 6 caractères." });
                    }

                    // Update password
                    var passwordResult = await userManager.ChangePasswordAsync(userDetails, model.CurrentPassword, model.NewPassword);
                    if (!passwordResult.Succeeded)
                    {
                        logger.LogWarning("Failed to update password: {Errors}",
                            string.Join(", ", passwordResult.Errors.Select(e => e.Description)));
                        return Results.BadRequest(new
                        {
                            message = "Erreur lors de la mise à jour du mot de passe",
                            errors = passwordResult.Errors.Select(e => e.Description)
                        });
                    }
                    logger.LogInformation("Password updated successfully for User ID: {UserId}", userId);
                }
                else if (string.IsNullOrEmpty(model.CurrentPassword) && !string.IsNullOrEmpty(model.NewPassword) ||
                         !string.IsNullOrEmpty(model.CurrentPassword) && string.IsNullOrEmpty(model.NewPassword))
                {
                    return Results.BadRequest(new { message = "Le mot de passe actuel et le nouveau mot de passe doivent être fournis ensemble." });
                }

                var result = await userManager.UpdateAsync(userDetails);
                if (!result.Succeeded)
                {
                    logger.LogWarning("Failed to update user profile: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));

                    return Results.BadRequest(new
                    {
                        message = "Erreur lors de la mise à jour du profil",
                        errors = result.Errors.Select(e => e.Description)
                    });
                }

                return Results.Ok(new { message = "Profil mis à jour avec succès." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in EditUserProfile for user");
                return Results.Problem(detail: "Une erreur est survenue lors de la mise à jour du profil.", statusCode: 500);
            }
        }

        [Authorize(Roles = "Admin")]
        private static async Task<IResult> AssignRole(
            HttpContext context,
            UserManager<AppUser> userManager,
            ILoggerFactory loggerFactory,
            [FromBody] AssignRoleRequest model)
        {
            var logger = loggerFactory.CreateLogger("AccountEndpoints.AssignRole");
            try
            {
                // Extract admin ID with multiple fallbacks
                string adminId = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                                 context.User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                                 context.User.FindFirstValue("sub") ??
                                 context.User.FindFirstValue("uid") ??
                                 context.User.FindFirstValue("userId") ??
                                 context.User.FindFirstValue("nameid");

                if (string.IsNullOrEmpty(adminId))
                {
                    logger.LogWarning("Admin ID not found in token");
                    return Results.Unauthorized();
                }

                logger.LogInformation("Admin action by User ID: {AdminId}", adminId);

                var targetUser = await userManager.FindByIdAsync(model.UserId.ToString());
                if (targetUser == null)
                {
                    logger.LogWarning("Target user not found for ID: {UserId}", model.UserId);
                    return Results.NotFound(new { message = "Utilisateur cible introuvable." });
                }

                if (string.IsNullOrEmpty(model.Role))
                {
                    return Results.BadRequest(new { message = "Le rôle est obligatoire." });
                }

                var validRoles = new[] { "Candidat", "Recruteur", "Admin" };
                if (!validRoles.Contains(model.Role))
                {
                    return Results.BadRequest(new
                    {
                        message = "Rôle invalide. Les rôles autorisés sont : Candidat, Recruteur, Admin."
                    });
                }

                var currentRoles = await userManager.GetRolesAsync(targetUser);
                await userManager.RemoveFromRolesAsync(targetUser, currentRoles);
                var result = await userManager.AddToRoleAsync(targetUser, model.Role);

                if (!result.Succeeded)
                {
                    logger.LogWarning("Failed to assign role: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));

                    return Results.BadRequest(new
                    {
                        message = "Erreur lors de l'attribution du rôle",
                        errors = result.Errors.Select(e => e.Description)
                    });
                }

                return Results.Ok(new { message = $"Rôle {model.Role} attribué avec succès à l'utilisateur." });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in AssignRole for admin");
                return Results.Problem(detail: "Une erreur est survenue lors de l'attribution du rôle.", statusCode: 500);
            }
        }

        public class EditProfileRequest
        {
            [Required] public string Nom { get; set; }
            [Required] public string Prenom { get; set; }
            [Required, EmailAddress] public string Email { get; set; }
            [Required, Phone] public string PhoneNumber { get; set; }
            [Required] public string Adresse { get; set; }
            [Required] public string Ville { get; set; }
            [Required] public string Pays { get; set; }
            public Guid? IdFiliale { get; set; }
            public DateTime? DateNaissance { get; set; }
            public string Photo { get; set; }
            public string CurrentPassword { get; set; } // Added for current password
            public string NewPassword { get; set; } // Added for new password
        }

        public class AssignRoleRequest
        {
            public Guid UserId { get; set; }
            public string Role { get; set; }
        }
    }
}