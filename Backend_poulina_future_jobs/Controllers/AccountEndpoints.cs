//using Backend_poulina_future_jobs.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using System.Security.Claims;

//namespace Backend_poulina_future_jobs.Controllers
//{
//    public static class AccountEndpoints
//    {
//        public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
//        {
//            app.MapGet("/UserProfile", GetUserProfile);
//            app.MapPut("/EditProfile", EditUserProfile);
//            app.MapPost("/UploadImage", UploadImage);
//            app.MapPost("/UploadCV", UploadCV);
//            return app;
//        }
//        [Authorize]
//        private static async Task<IResult> GetUserProfile(
//         ClaimsPrincipal user,
//         UserManager<AppUser> userManager,
//         IConfiguration configuration,
//         HttpContext context,
//         ApplicationDbContext dbContext,
//         ILoggerFactory loggerFactory)
//        {
//            var logger = loggerFactory.CreateLogger("AccountEndpoints.GetUserProfile");
//            try
//            {
//                logger.LogInformation("GetUserProfile called for user ID: {UserId}", user.Claims.FirstOrDefault(x => x.Type == "userId")?.Value);
//                string userID = user.Claims.First(x => x.Type == "userId").Value;
//                logger.LogInformation("Fetching user with ID: {UserId}", userID);

//                var userDetails = await userManager.Users
//                    .Where(u => u.Id.ToString() == userID)
//                    .Select(u => new
//                    {
//                        u.Email,
//                        u.FullName,
//                        u.Nom,
//                        u.Prenom,
//                        u.Photo,
//                        u.cv,
//                        u.DateNaissance,
//                        u.Adresse,
//                        u.Ville,
//                        u.Pays,
//                        u.phone,
//                        u.NiveauEtude,
//                        u.Diplome,
//                        u.Universite,
//                        u.specialite,
//                        u.linkedIn,
//                        u.github,
//                        u.portfolio,
//                        u.Poste,
//                        u.Statut,
//                        Filiale = u.Filiale != null
//                            ? new { u.Filiale.IdFiliale, u.Filiale.Nom, u.Filiale.Description }
//                            : null
//                    })
//                    .FirstOrDefaultAsync();

//                if (userDetails == null)
//                {
//                    logger.LogWarning("User not found for ID: {UserId}", userID);
//                    return Results.NotFound(new { message = "Utilisateur introuvable." });
//                }

//                logger.LogInformation("User fetched successfully for ID: {UserId}", userID);
//                return Results.Ok(userDetails);
//            }
//            catch (Exception ex)
//            {
//                logger.LogError(ex, "Error in GetUserProfile for user ID: {UserId}", user.Claims.FirstOrDefault(x => x.Type == "userId")?.Value);
//                if (ex.Message.Contains("The token expired"))
//                {
//                    var refreshToken = context.Request.Headers["X-Refresh-Token"].FirstOrDefault();
//                    if (string.IsNullOrEmpty(refreshToken))
//                    {
//                        logger.LogWarning("Refresh token missing in header.");
//                        return Results.Unauthorized();
//                    }

//                    var refreshResult = await IdentityUserEndpoints.RefreshToken(
//                        userManager,
//                        configuration,
//                        new RefreshTokenRequest { RefreshToken = refreshToken });

//                    if (refreshResult is IResult okResult && okResult.GetType().GetProperty("Value")?.GetValue(okResult) is { } newTokenResponse)
//                    {
//                        var tokenResponse = (dynamic)newTokenResponse;
//                        if (tokenResponse.Success)
//                        {
//                            logger.LogInformation("Token refreshed successfully. New token issued.");
//                            context.Response.Headers.Add("X-New-Token", tokenResponse.Token);
//                            string userID = user.Claims.First(x => x.Type == "userId").Value;

//                            var userDetails = await userManager.Users
//                                .Where(u => u.Id.ToString() == userID)
//                                .Select(u => new
//                                {
//                                    u.Email,
//                                    u.FullName,
//                                    u.Nom,
//                                    u.Prenom,
//                                    u.Photo,
//                                    u.cv,
//                                    u.DateNaissance,
//                                    u.Adresse,
//                                    u.Ville,
//                                    u.Pays,
//                                    u.phone,
//                                    u.NiveauEtude,
//                                    u.Diplome,
//                                    u.Universite,
//                                    u.specialite,
//                                    u.linkedIn,
//                                    u.github,
//                                    u.portfolio,
//                                    u.Poste,
//                                    u.Statut,
//                                    Filiale = u.Filiale != null
//                                        ? new { u.Filiale.IdFiliale, u.Filiale.Nom, u.Filiale.Description }
//                                        : null
//                                })
//                                .FirstOrDefaultAsync();

//                            if (userDetails == null)
//                            {
//                                logger.LogWarning("User not found for ID: {UserId}", userID);
//                                return Results.NotFound(new { message = "Utilisateur introuvable." });
//                            }

//                            logger.LogInformation("User fetched successfully for ID: {UserId} after token refresh", userID);
//                            return Results.Ok(userDetails);
//                        }
//                    }
//                    logger.LogWarning("Token refresh failed.");
//                    return Results.Unauthorized();
//                }
//                // Remplacement de Results.StatusCode par Results.Problem
//                return Results.Problem(
//                    detail: "Une erreur est survenue : " + ex.Message,
//                    statusCode: 500,
//                    title: "Erreur serveur"
//                );
//            }
//        }

//        [Authorize]
//        private static async Task<IResult> EditUserProfile(
//            ClaimsPrincipal user,
//            UserManager<AppUser> userManager,
//            IConfiguration configuration,
//            HttpContext context,
//            ApplicationDbContext dbContext,
//            ILoggerFactory loggerFactory,
//            [FromBody] EditProfileRequest model)
//        {
//            var logger = loggerFactory.CreateLogger("AccountEndpoints.EditUserProfile");
//            try
//            {
//                logger.LogInformation("EditUserProfile called for user ID: {UserId}", user.Claims.FirstOrDefault(x => x.Type == "userId")?.Value);
//                string userID = user.Claims.First(x => x.Type == "userId").Value;
//                var userDetails = await userManager.FindByIdAsync(userID);

//                if (userDetails == null)
//                {
//                    logger.LogWarning("User not found for ID: {UserId}", userID);
//                    return Results.NotFound(new { message = "Utilisateur introuvable." });
//                }

//                if (model.IdFiliale.HasValue)
//                {
//                    logger.LogInformation("Validating IdFiliale: {IdFiliale}", model.IdFiliale);
//                    var filiale = await dbContext.Filiales.FindAsync(model.IdFiliale);
//                    if (filiale == null)
//                    {
//                        logger.LogWarning("Invalid IdFiliale: {IdFiliale}", model.IdFiliale);
//                        return Results.BadRequest(new { message = "Le IdFiliale spécifié est invalide ou n'existe pas." });
//                    }
//                }

//                userDetails.FullName = model.FullName ?? userDetails.FullName;
//                userDetails.Nom = model.Nom ?? userDetails.Nom;
//                userDetails.Prenom = model.Prenom ?? userDetails.Prenom;
//                userDetails.DateNaissance = model.DateNaissance ?? userDetails.DateNaissance;
//                userDetails.Adresse = model.Adresse ?? userDetails.Adresse;
//                userDetails.Ville = model.Ville ?? userDetails.Ville;
//                userDetails.Pays = model.Pays ?? userDetails.Pays;
//                userDetails.phone = model.phone ?? userDetails.phone;
//                userDetails.NiveauEtude = model.NiveauEtude ?? userDetails.NiveauEtude;
//                userDetails.Diplome = model.Diplome ?? userDetails.Diplome;
//                userDetails.Universite = model.Universite ?? userDetails.Universite;
//                userDetails.specialite = model.specialite ?? userDetails.specialite;
//                userDetails.linkedIn = model.linkedIn ?? userDetails.linkedIn;
//                userDetails.github = model.github ?? userDetails.github;
//                userDetails.portfolio = model.portfolio ?? userDetails.portfolio;
//                userDetails.Poste = model.Poste ?? userDetails.Poste;
//                userDetails.Statut = model.Statut ?? userDetails.Statut;
//                userDetails.Photo = model.Photo ?? userDetails.Photo;
//                userDetails.cv = model.cv ?? userDetails.cv;
//                userDetails.IdFiliale = model.IdFiliale ?? userDetails.IdFiliale;

//                logger.LogInformation("Updating user profile for ID: {UserId}", userID);
//                var result = await userManager.UpdateAsync(userDetails);
//                if (!result.Succeeded)
//                {
//                    logger.LogWarning("Failed to update user profile: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
//                    return Results.BadRequest(new { message = "Erreur lors de la mise à jour du profil", errors = result.Errors });
//                }

//                logger.LogInformation("Profile updated successfully for user ID: {UserId}", userID);
//                return Results.Ok(new { message = "Profil mis à jour avec succès." });
//            }
//            catch (Exception ex)
//            {
//                logger.LogError(ex, "Error in EditUserProfile for user ID: {UserId}", user.Claims.FirstOrDefault(x => x.Type == "userId")?.Value);
//                if (ex.Message.Contains("The token expired"))
//                {
//                    var refreshToken = context.Request.Headers["X-Refresh-Token"].FirstOrDefault();
//                    if (string.IsNullOrEmpty(refreshToken))
//                    {
//                        logger.LogWarning("Refresh token missing in header.");
//                        return Results.Unauthorized();
//                    }

//                    var refreshResult = await IdentityUserEndpoints.RefreshToken(
//                        userManager,
//                        configuration,
//                        new RefreshTokenRequest { RefreshToken = refreshToken });

//                    if (refreshResult is IResult okResult && okResult.GetType().GetProperty("Value")?.GetValue(okResult) is { } newTokenResponse)
//                    {
//                        var tokenResponse = (dynamic)newTokenResponse;
//                        if (tokenResponse.Success)
//                        {
//                            logger.LogInformation("Token refreshed successfully. New token issued.");
//                            context.Response.Headers.Add("X-New-Token", tokenResponse.Token);
//                            string userID = user.Claims.First(x => x.Type == "userId").Value;
//                            var userDetails = await userManager.FindByIdAsync(userID);

//                            if (userDetails == null)
//                            {
//                                logger.LogWarning("User not found for ID: {UserId}", userID);
//                                return Results.NotFound(new { message = "Utilisateur introuvable." });
//                            }

//                            if (model.IdFiliale.HasValue)
//                            {
//                                logger.LogInformation("Validating IdFiliale after token refresh: {IdFiliale}", model.IdFiliale);
//                                var filiale = await dbContext.Filiales.FindAsync(model.IdFiliale);
//                                if (filiale == null)
//                                {
//                                    logger.LogWarning("Invalid IdFiliale: {IdFiliale}", model.IdFiliale);
//                                    return Results.BadRequest(new { message = "Le IdFiliale spécifié est invalide ou n'existe pas." });
//                                }
//                            }

//                            userDetails.FullName = model.FullName ?? userDetails.FullName;
//                            userDetails.Nom = model.Nom ?? userDetails.Nom;
//                            userDetails.Prenom = model.Prenom ?? userDetails.Prenom;
//                            userDetails.DateNaissance = model.DateNaissance ?? userDetails.DateNaissance;
//                            userDetails.Adresse = model.Adresse ?? userDetails.Adresse;
//                            userDetails.Ville = model.Ville ?? userDetails.Ville;
//                            userDetails.Pays = model.Pays ?? userDetails.Pays;
//                            userDetails.phone = model.phone ?? userDetails.phone;
//                            userDetails.NiveauEtude = model.NiveauEtude ?? userDetails.NiveauEtude;
//                            userDetails.Diplome = model.Diplome ?? userDetails.Diplome;
//                            userDetails.Universite = model.Universite ?? userDetails.Universite;
//                            userDetails.specialite = model.specialite ?? userDetails.specialite;
//                            userDetails.linkedIn = model.linkedIn ?? userDetails.linkedIn;
//                            userDetails.github = model.github ?? userDetails.github;
//                            userDetails.portfolio = model.portfolio ?? userDetails.portfolio;
//                            userDetails.Poste = model.Poste ?? userDetails.Poste;
//                            userDetails.Statut = model.Statut ?? userDetails.Statut;
//                            userDetails.Photo = model.Photo ?? userDetails.Photo;
//                            userDetails.cv = model.cv ?? userDetails.cv;
//                            userDetails.IdFiliale = model.IdFiliale ?? userDetails.IdFiliale;

//                            logger.LogInformation("Updating user profile after token refresh for ID: {UserId}", userID);
//                            var result = await userManager.UpdateAsync(userDetails);
//                            if (!result.Succeeded)
//                            {
//                                logger.LogWarning("Failed to update user profile after token refresh: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
//                                return Results.BadRequest(new { message = "Erreur lors de la mise à jour du profil", errors = result.Errors });
//                            }

//                            logger.LogInformation("Profile updated successfully after token refresh for user ID: {UserId}", userID);
//                            return Results.Ok(new { message = "Profil mis à jour avec succès." });
//                        }
//                    }
//                    logger.LogWarning("Token refresh failed.");
//                    return Results.Unauthorized();
//                }
//                // Remplacement de Results.StatusCode par Results.Problem
//                return Results.Problem(
//                    detail: "Une erreur est survenue : " + ex.Message,
//                    statusCode: 500,
//                    title: "Erreur serveur"
//                );
//            }
//        }

//        [Authorize]
//        private static async Task<IResult> UploadImage(
//            ClaimsPrincipal user,
//            UserManager<AppUser> userManager,
//            HttpContext context,
//            IConfiguration configuration,
//            ILoggerFactory loggerFactory)
//        {
//            var logger = loggerFactory.CreateLogger("AccountEndpoints.UploadImage");
//            try
//            {
//                logger.LogInformation("UploadImage endpoint called for user ID: {UserId}", user.Claims.FirstOrDefault(x => x.Type == "userId")?.Value);

//                if (!user.Identity.IsAuthenticated)
//                {
//                    logger.LogWarning("User is not authenticated.");
//                    return Results.Unauthorized();
//                }

//                logger.LogInformation("Request Content-Type: {ContentType}", context.Request.ContentType);

//                if (string.IsNullOrEmpty(context.Request.ContentType) || !context.Request.ContentType.ToLower().Contains("multipart/form-data"))
//                {
//                    logger.LogWarning("Invalid Content-Type: {ContentType}. Expected multipart/form-data.", context.Request.ContentType);
//                    return Results.BadRequest(new { message = "Requête invalide : Content-Type doit être multipart/form-data." });
//                }

//                if (context.Request.Form.Files.Count == 0)
//                {
//                    logger.LogWarning("No files found in the request.");
//                    return Results.BadRequest(new { message = "Aucun fichier image sélectionné. Assurez-vous que la clé du fichier est 'file'." });
//                }

//                var formKeys = context.Request.Form.Keys;
//                logger.LogInformation("Form keys received: {FormKeys}", string.Join(", ", formKeys));

//                var file = context.Request.Form.Files.GetFile("file");
//                if (file == null || file.Length == 0)
//                {
//                    logger.LogWarning("No file found with key 'file' or file is empty.");
//                    return Results.BadRequest(new { message = "Aucun fichier image sélectionné. Assurez-vous que la clé du fichier est 'file'." });
//                }

//                logger.LogInformation("File received: Name={FileName}, Size={FileSize} bytes", file.FileName, file.Length);

//                var allowedExtensions = new[] { ".png", ".jfif", ".jpeg", ".jpg", ".gif", ".bmp" };
//                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
//                if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
//                {
//                    logger.LogWarning("Unsupported file extension: {Extension}. Allowed: {AllowedExtensions}", extension, string.Join(", ", allowedExtensions));
//                    return Results.BadRequest(new { message = $"Format de fichier non pris en charge. Formats autorisés : {string.Join(", ", allowedExtensions)}" });
//                }

//                const long maxFileSize = 5 * 1024 * 1024; // 5MB
//                if (file.Length > maxFileSize)
//                {
//                    logger.LogWarning("File size exceeds limit: {FileSize} bytes. Max allowed: {MaxFileSize} bytes", file.Length, maxFileSize);
//                    return Results.BadRequest(new { message = "La taille du fichier dépasse la limite de 5 Mo." });
//                }

//                string userID = user.Claims.First(x => x.Type == "userId").Value;
//                var userDetails = await userManager.FindByIdAsync(userID);
//                if (userDetails == null)
//                {
//                    logger.LogWarning("User not found for ID: {UserId}", userID);
//                    return Results.NotFound(new { message = "Utilisateur introuvable." });
//                }

//                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "images");
//                logger.LogInformation("Creating directory if not exists: {UploadsFolder}", uploadsFolder);
//                Directory.CreateDirectory(uploadsFolder);

//                if (!string.IsNullOrEmpty(userDetails.Photo))
//                {
//                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", userDetails.Photo.TrimStart('/'));
//                    if (File.Exists(oldImagePath))
//                    {
//                        logger.LogInformation("Deleting existing photo: {OldImagePath}", oldImagePath);
//                        File.Delete(oldImagePath);
//                    }
//                    else
//                    {
//                        logger.LogWarning("Old photo not found at path: {OldImagePath}", oldImagePath);
//                    }
//                }

//                var fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{Guid.NewGuid()}{extension}";
//                var filePath = Path.Combine(uploadsFolder, fileName);
//                logger.LogInformation("Saving file to: {FilePath}", filePath);

//                using (var stream = new FileStream(filePath, FileMode.Create))
//                {
//                    await file.CopyToAsync(stream);
//                }

//                var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
//                var fileUrl = $"{baseUrl}/Uploads/images/{fileName}";
//                userDetails.Photo = $"/Uploads/images/{fileName}";
//                var result = await userManager.UpdateAsync(userDetails);
//                if (!result.Succeeded)
//                {
//                    logger.LogWarning("Failed to update user profile with new photo: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
//                    return Results.BadRequest(new { message = "Erreur lors de la mise à jour du profil", errors = result.Errors });
//                }

//                logger.LogInformation("Image uploaded successfully for user ID: {UserId}. URL: {FileUrl}", userID, fileUrl);
//                return Results.Ok(new
//                {
//                    message = "Téléchargement réussi!",
//                    url = fileUrl
//                });
//            }
//            catch (Exception ex)
//            {
//                logger.LogError(ex, "Error occurred during image upload for user ID: {UserId}", user.Claims.FirstOrDefault(x => x.Type == "userId")?.Value);
//                if (ex.Message.Contains("The token expired"))
//                {
//                    var refreshToken = context.Request.Headers["X-Refresh-Token"].FirstOrDefault();
//                    if (string.IsNullOrEmpty(refreshToken))
//                    {
//                        logger.LogWarning("Refresh token missing in header.");
//                        return Results.Unauthorized();
//                    }

//                    var refreshResult = await IdentityUserEndpoints.RefreshToken(
//                        userManager,
//                        configuration,
//                        new RefreshTokenRequest { RefreshToken = refreshToken });

//                    if (refreshResult is IResult okResult && okResult.GetType().GetProperty("Value")?.GetValue(okResult) is { } newTokenResponse)
//                    {
//                        var tokenResponse = (dynamic)newTokenResponse;
//                        if (tokenResponse.Success)
//                        {
//                            logger.LogInformation("Token refreshed successfully. New token issued.");
//                            context.Response.Headers.Add("X-New-Token", tokenResponse.Token);
//                            var file = context.Request.Form.Files.GetFile("file");
//                            if (file == null || file.Length == 0)
//                            {
//                                logger.LogWarning("No file found with key 'file' or file is empty after token refresh.");
//                                return Results.BadRequest(new { message = "Aucun fichier image sélectionné. Assurez-vous que la clé du fichier est 'file'." });
//                            }

//                            var allowedExtensions = new[] { ".png", ".jfif", ".jpeg", ".jpg", ".gif", ".bmp" };
//                            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
//                            if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
//                            {
//                                logger.LogWarning("Unsupported file extension after token refresh: {Extension}. Allowed: {AllowedExtensions}", extension, string.Join(", ", allowedExtensions));
//                                return Results.BadRequest(new { message = $"Format de fichier non pris en charge. Formats autorisés : {string.Join(", ", allowedExtensions)}" });
//                            }

//                            const long maxFileSize = 5 * 1024 * 1024; // 5MB
//                            if (file.Length > maxFileSize)
//                            {
//                                logger.LogWarning("File size exceeds limit after token refresh: {FileSize} bytes. Max allowed: {MaxFileSize} bytes", file.Length, maxFileSize);
//                                return Results.BadRequest(new { message = "La taille du fichier dépasse la limite de 5 Mo." });
//                            }

//                            string userID = user.Claims.First(x => x.Type == "userId").Value;
//                            var userDetails = await userManager.FindByIdAsync(userID);

//                            if (userDetails == null)
//                            {
//                                logger.LogWarning("User not found for ID: {UserId} after token refresh", userID);
//                                return Results.NotFound(new { message = "Utilisateur introuvable." });
//                            }

//                            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "images");
//                            logger.LogInformation("Creating directory if not exists after token refresh: {UploadsFolder}", uploadsFolder);
//                            Directory.CreateDirectory(uploadsFolder);

//                            if (!string.IsNullOrEmpty(userDetails.Photo))
//                            {
//                                var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", userDetails.Photo.TrimStart('/'));
//                                if (File.Exists(oldImagePath))
//                                {
//                                    logger.LogInformation("Deleting existing photo after token refresh: {OldImagePath}", oldImagePath);
//                                    File.Delete(oldImagePath);
//                                }
//                                else
//                                {
//                                    logger.LogWarning("Old photo not found at path after token refresh: {OldImagePath}", oldImagePath);
//                                }
//                            }

//                            var fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{Guid.NewGuid()}{extension}";
//                            var filePath = Path.Combine(uploadsFolder, fileName);
//                            logger.LogInformation("Saving file after token refresh to: {FilePath}", filePath);

//                            using (var stream = new FileStream(filePath, FileMode.Create))
//                            {
//                                await file.CopyToAsync(stream);
//                            }

//                            var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
//                            var fileUrl = $"{baseUrl}/Uploads/images/{fileName}";

//                            userDetails.Photo = $"/Uploads/images/{fileName}";
//                            var result = await userManager.UpdateAsync(userDetails);
//                            if (!result.Succeeded)
//                            {
//                                logger.LogWarning("Failed to update user profile with new photo after token refresh: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
//                                return Results.BadRequest(new { message = "Erreur lors de la mise à jour du profil", errors = result.Errors });
//                            }

//                            logger.LogInformation("Image uploaded successfully after token refresh for user ID: {UserId}. URL: {FileUrl}", userID, fileUrl);
//                            return Results.Ok(new
//                            {
//                                message = "Téléchargement réussi!",
//                                url = fileUrl
//                            });
//                        }
//                    }
//                    logger.LogWarning("Token refresh failed.");
//                    return Results.Unauthorized();
//                }
//                // Remplacement de Results.StatusCode par Results.Problem
//                return Results.Problem(
//                    detail: "Une erreur est survenue lors du téléchargement : " + ex.Message,
//                    statusCode: 500,
//                    title: "Erreur serveur"
//                );
//            }
//        }

//        [Authorize]
//        private static async Task<IResult> UploadCV(
//            ClaimsPrincipal user,
//            UserManager<AppUser> userManager,
//            HttpContext context,
//            IConfiguration configuration,
//            ILoggerFactory loggerFactory)
//        {
//            var logger = loggerFactory.CreateLogger("AccountEndpoints.UploadCV");
//            try
//            {
//                logger.LogInformation("UploadCV endpoint called for user ID: {UserId}", user.Claims.FirstOrDefault(x => x.Type == "userId")?.Value);

//                if (!user.Identity.IsAuthenticated)
//                {
//                    logger.LogWarning("User is not authenticated.");
//                    return Results.Unauthorized();
//                }

//                logger.LogInformation("Request Content-Type: {ContentType}", context.Request.ContentType);

//                if (string.IsNullOrEmpty(context.Request.ContentType) || !context.Request.ContentType.ToLower().Contains("multipart/form-data"))
//                {
//                    logger.LogWarning("Invalid Content-Type: {ContentType}. Expected multipart/form-data.", context.Request.ContentType);
//                    return Results.BadRequest(new { message = "Requête invalide : Content-Type doit être multipart/form-data." });
//                }

//                if (context.Request.Form.Files.Count == 0)
//                {
//                    logger.LogWarning("No files found in the request.");
//                    return Results.BadRequest(new { message = "Aucun fichier CV sélectionné. Assurez-vous que la clé du fichier est 'file'." });
//                }

//                var formKeys = context.Request.Form.Keys;
//                logger.LogInformation("Form keys received: {FormKeys}", string.Join(", ", formKeys));

//                var file = context.Request.Form.Files.GetFile("file");
//                if (file == null || file.Length == 0)
//                {
//                    logger.LogWarning("No file found with key 'file' or file is empty.");
//                    return Results.BadRequest(new { message = "Aucun fichier CV sélectionné. Assurez-vous que la clé du fichier est 'file'." });
//                }

//                logger.LogInformation("File received: Name={FileName}, Size={FileSize} bytes", file.FileName, file.Length);

//                var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
//                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
//                if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
//                {
//                    logger.LogWarning("Unsupported file extension: {Extension}. Allowed: {AllowedExtensions}", extension, string.Join(", ", allowedExtensions));
//                    return Results.BadRequest(new { message = $"Format de fichier non pris en charge. Formats autorisés : {string.Join(", ", allowedExtensions)}" });
//                }

//                const long maxFileSize = 5 * 1024 * 1024; // 5MB
//                if (file.Length > maxFileSize)
//                {
//                    logger.LogWarning("File size exceeds limit: {FileSize} bytes. Max allowed: {MaxFileSize} bytes", file.Length, maxFileSize);
//                    return Results.BadRequest(new { message = "La taille du fichier dépasse la limite de 5 Mo." });
//                }

//                string userID = user.Claims.First(x => x.Type == "userId").Value;
//                var userDetails = await userManager.FindByIdAsync(userID);
//                if (userDetails == null)
//                {
//                    logger.LogWarning("User not found for ID: {UserId}", userID);
//                    return Results.NotFound(new { message = "Utilisateur introuvable." });
//                }

//                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "cvs");
//                logger.LogInformation("Creating directory if not exists: {UploadsFolder}", uploadsFolder);
//                Directory.CreateDirectory(uploadsFolder);

//                if (!string.IsNullOrEmpty(userDetails.cv))
//                {
//                    var oldCvPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", userDetails.cv.TrimStart('/'));
//                    if (File.Exists(oldCvPath))
//                    {
//                        logger.LogInformation("Deleting existing CV: {OldCvPath}", oldCvPath);
//                        File.Delete(oldCvPath);
//                    }
//                    else
//                    {
//                        logger.LogWarning("Old CV not found at path: {OldCvPath}", oldCvPath);
//                    }
//                }

//                var fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{Guid.NewGuid()}{extension}";
//                var filePath = Path.Combine(uploadsFolder, fileName);
//                logger.LogInformation("Saving file to: {FilePath}", filePath);

//                using (var stream = new FileStream(filePath, FileMode.Create))
//                {
//                    await file.CopyToAsync(stream);
//                }

//                var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
//                var fileUrl = $"{baseUrl}/Uploads/cvs/{fileName}";
//                userDetails.cv = $"/Uploads/cvs/{fileName}";
//                var result = await userManager.UpdateAsync(userDetails);
//                if (!result.Succeeded)
//                {
//                    logger.LogWarning("Failed to update user profile with new CV: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
//                    return Results.BadRequest(new { message = "Erreur lors de la mise à jour du profil", errors = result.Errors });
//                }

//                logger.LogInformation("CV uploaded successfully for user ID: {UserId}. URL: {FileUrl}", userID, fileUrl);
//                return Results.Ok(new
//                {
//                    message = "Téléchargement réussi!",
//                    url = fileUrl
//                });
//            }
//            catch (Exception ex)
//            {
//                logger.LogError(ex, "Error occurred during CV upload for user ID: {UserId}", user.Claims.FirstOrDefault(x => x.Type == "userId")?.Value);
//                if (ex.Message.Contains("The token expired"))
//                {
//                    var refreshToken = context.Request.Headers["X-Refresh-Token"].FirstOrDefault();
//                    if (string.IsNullOrEmpty(refreshToken))
//                    {
//                        logger.LogWarning("Refresh token missing in header.");
//                        return Results.Unauthorized();
//                    }

//                    var refreshResult = await IdentityUserEndpoints.RefreshToken(
//                        userManager,
//                        configuration,
//                        new RefreshTokenRequest { RefreshToken = refreshToken });

//                    if (refreshResult is IResult okResult && okResult.GetType().GetProperty("Value")?.GetValue(okResult) is { } newTokenResponse)
//                    {
//                        var tokenResponse = (dynamic)newTokenResponse;
//                        if (tokenResponse.Success)
//                        {
//                            logger.LogInformation("Token refreshed successfully. New token issued.");
//                            context.Response.Headers.Add("X-New-Token", tokenResponse.Token);
//                            var file = context.Request.Form.Files.GetFile("file");
//                            if (file == null || file.Length == 0)
//                            {
//                                logger.LogWarning("No file found with key 'file' or file is empty after token refresh.");
//                                return Results.BadRequest(new { message = "Aucun fichier CV sélectionné. Assurez-vous que la clé du fichier est 'file'." });
//                            }

//                            var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
//                            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
//                            if (string.IsNullOrEmpty(extension) || !allowedExtensions.Contains(extension))
//                            {
//                                logger.LogWarning("Unsupported file extension after token refresh: {Extension}. Allowed: {AllowedExtensions}", extension, string.Join(", ", allowedExtensions));
//                                return Results.BadRequest(new { message = $"Format de fichier non pris en charge. Formats autorisés : {string.Join(", ", allowedExtensions)}" });
//                            }

//                            const long maxFileSize = 5 * 1024 * 1024; // 5MB
//                            if (file.Length > maxFileSize)
//                            {
//                                logger.LogWarning("File size exceeds limit after token refresh: {FileSize} bytes. Max allowed: {MaxFileSize} bytes", file.Length, maxFileSize);
//                                return Results.BadRequest(new { message = "La taille du fichier dépasse la limite de 5 Mo." });
//                            }

//                            string userID = user.Claims.First(x => x.Type == "userId").Value;
//                            var userDetails = await userManager.FindByIdAsync(userID);

//                            if (userDetails == null)
//                            {
//                                logger.LogWarning("User not found for ID: {UserId} after token refresh", userID);
//                                return Results.NotFound(new { message = "Utilisateur introuvable." });
//                            }

//                            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads", "cvs");
//                            logger.LogInformation("Creating directory if not exists after token refresh: {UploadsFolder}", uploadsFolder);
//                            Directory.CreateDirectory(uploadsFolder);

//                            if (!string.IsNullOrEmpty(userDetails.cv))
//                            {
//                                var oldCvPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", userDetails.cv.TrimStart('/'));
//                                if (File.Exists(oldCvPath))
//                                {
//                                    logger.LogInformation("Deleting existing CV after token refresh: {OldCvPath}", oldCvPath);
//                                    File.Delete(oldCvPath);
//                                }
//                                else
//                                {
//                                    logger.LogWarning("Old CV not found at path after token refresh: {OldCvPath}", oldCvPath);
//                                }
//                            }

//                            var fileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{Guid.NewGuid()}{extension}";
//                            var filePath = Path.Combine(uploadsFolder, fileName);
//                            logger.LogInformation("Saving file after token refresh to: {FilePath}", filePath);

//                            using (var stream = new FileStream(filePath, FileMode.Create))
//                            {
//                                await file.CopyToAsync(stream);
//                            }

//                            var baseUrl = $"{context.Request.Scheme}://{context.Request.Host}";
//                            var fileUrl = $"{baseUrl}/Uploads/cvs/{fileName}";

//                            userDetails.cv = $"/Uploads/cvs/{fileName}";
//                            var result = await userManager.UpdateAsync(userDetails);
//                            if (!result.Succeeded)
//                            {
//                                logger.LogWarning("Failed to update user profile with new CV after token refresh: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
//                                return Results.BadRequest(new { message = "Erreur lors de la mise à jour du profil", errors = result.Errors });
//                            }

//                            logger.LogInformation("CV uploaded successfully after token refresh for user ID: {UserId}. URL: {FileUrl}", userID, fileUrl);
//                            return Results.Ok(new
//                            {
//                                message = "Téléchargement réussi!",
//                                url = fileUrl
//                            });
//                        }
//                    }
//                    logger.LogWarning("Token refresh failed.");
//                    return Results.Unauthorized();
//                }
//                // Remplacement de Results.StatusCode par Results.Problem
//                return Results.Problem(
//                    detail: "Une erreur est survenue lors du téléchargement : " + ex.Message,
//                    statusCode: 500,
//                    title: "Erreur serveur"
//                );
//            }
//        }

//        public class EditProfileRequest
//        {
//            public string? FullName { get; set; }
//            public string? Nom { get; set; }
//            public string? Prenom { get; set; }
//            public DateTime? DateNaissance { get; set; }
//            public string? Adresse { get; set; }
//            public string? Ville { get; set; }
//            public string? Pays { get; set; }
//            public string? phone { get; set; }
//            public string? NiveauEtude { get; set; }
//            public string? Diplome { get; set; }
//            public string? Universite { get; set; }
//            public string? specialite { get; set; }
//            public string? linkedIn { get; set; }
//            public string? github { get; set; }
//            public string? portfolio { get; set; }
//            public string? Poste { get; set; }
//            public string? Statut { get; set; }
//            public string? Photo { get; set; }
//            public string? cv { get; set; }
//            public Guid? IdFiliale { get; set; }
//        }

//        // Add the missing DbSet property for 'Filiales' in the ApplicationDbContext class.  
//        // This will resolve the CS1061 error by ensuring that 'Filiales' is defined in the context.  

       
//    }

//}