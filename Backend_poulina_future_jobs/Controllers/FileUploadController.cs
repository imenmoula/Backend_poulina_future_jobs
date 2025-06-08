using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Backend_poulina_future_jobs.Models;
using System.IdentityModel.Tokens.Jwt;

namespace Backend_poulina_future_jobs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Assurez-vous que seuls les utilisateurs authentifiés peuvent uploader des fichiers
    public class FileUploadController : ControllerBase
    {
        private readonly ILogger<FileUploadController> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _uploadsBasePath;
        private readonly UserManager<AppUser> _userManager; // Added


        // Types de fichiers autorisés
        private readonly string[] _allowedExtensions = { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };
        // Taille maximale de fichier (10 MB par défaut)
        private readonly long _maxFileSize = 10 * 1024 * 1024;

        public FileUploadController(ILogger<FileUploadController> logger, IConfiguration configuration, UserManager<AppUser> userManager)
        {
            _logger = logger;
            _configuration = configuration;
            _userManager = userManager;

            // Récupérer le chemin de base pour les uploads depuis la configuration
            // Vous devez ajouter cette clé dans votre appsettings.json
            _uploadsBasePath = _configuration["FileStorage:UploadsPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

            // S'assurer que le répertoire existe
            if (!Directory.Exists(_uploadsBasePath))
            {
                Directory.CreateDirectory(_uploadsBasePath);
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file, [FromQuery] string folder = "general")
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("Upload attempt with empty file");
                    return BadRequest(new { error = "Aucun fichier n'a été fourni." });
                }

                // Vérifier la taille du fichier
                if (file.Length > _maxFileSize)
                {
                    _logger.LogWarning($"File size exceeds limit: {file.Length} bytes");
                    return BadRequest(new { error = $"La taille du fichier dépasse la limite autorisée ({_maxFileSize / (1024 * 1024)} MB)." });
                }

                // Vérifier l'extension du fichier
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(fileExtension))
                {
                    _logger.LogWarning($"Invalid file extension: {fileExtension}");
                    return BadRequest(new { error = $"Type de fichier non autorisé. Extensions autorisées: {string.Join(", ", _allowedExtensions)}" });
                }

                // Nettoyer le nom du dossier pour éviter les injections de chemin
                folder = CleanFolderName(folder);

                // Créer le dossier spécifique s'il n'existe pas
                var targetFolder = Path.Combine(_uploadsBasePath, folder);
                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                // Générer un nom de fichier unique pour éviter les écrasements
                var uniqueFileName = GetUniqueFileName(file.FileName);
                var filePath = Path.Combine(targetFolder, uniqueFileName);

                // Sauvegarder le fichier
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Récupérer l'ID utilisateur depuis le token JWT (si nécessaire pour le tracking)
                var userId = User.FindFirst("userId")?.Value;
                _logger.LogInformation($"File uploaded successfully by user {userId}: {filePath}");

                // Construire l'URL relative pour accéder au fichier
                var fileUrl = $"/api/FileUpload/files/{folder}/{uniqueFileName}";

                return Ok(new
                {
                    fileName = uniqueFileName,
                    originalName = file.FileName,
                    fileSize = file.Length,
                    fileUrl = fileUrl,
                    filePath = filePath // Optionnel, selon vos besoins
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during file upload");
                return StatusCode(500, new { error = "Une erreur est survenue lors de l'upload du fichier." });
            }
        }

        [HttpPost("upload-multiple")]
        public async Task<IActionResult> UploadMultipleFiles([FromForm] List<IFormFile> files, [FromQuery] string folder = "general")
        {
            try
            {
                if (files == null || !files.Any())
                {
                    _logger.LogWarning("Upload attempt with no files");
                    return BadRequest(new { error = "Aucun fichier n'a été fourni." });
                }

                var results = new List<object>();
                var errors = new List<string>();

                foreach (var file in files)
                {
                    // Vérifier la taille du fichier
                    if (file.Length > _maxFileSize)
                    {
                        errors.Add($"Le fichier {file.FileName} dépasse la taille limite.");
                        continue;
                    }

                    // Vérifier l'extension du fichier
                    var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (!_allowedExtensions.Contains(fileExtension))
                    {
                        errors.Add($"Le fichier {file.FileName} a une extension non autorisée.");
                        continue;
                    }

                    // Nettoyer le nom du dossier
                    folder = CleanFolderName(folder);

                    // Créer le dossier spécifique s'il n'existe pas
                    var targetFolder = Path.Combine(_uploadsBasePath, folder);
                    if (!Directory.Exists(targetFolder))
                    {
                        Directory.CreateDirectory(targetFolder);
                    }

                    // Générer un nom de fichier unique
                    var uniqueFileName = GetUniqueFileName(file.FileName);
                    var filePath = Path.Combine(targetFolder, uniqueFileName);

                    // Sauvegarder le fichier
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Construire l'URL relative
                    var fileUrl = $"/api/FileUpload/files/{folder}/{uniqueFileName}";

                    results.Add(new
                    {
                        fileName = uniqueFileName,
                        originalName = file.FileName,
                        fileSize = file.Length,
                        fileUrl = fileUrl
                    });
                }

                return Ok(new
                {
                    files = results,
                    errors = errors
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during multiple files upload");
                return StatusCode(500, new { error = "Une erreur est survenue lors de l'upload des fichiers." });
            }
        }

        [HttpGet("files/{folder}/{fileName}")]
        [AllowAnonymous] // Permettre l'accès public aux fichiers (vous pouvez restreindre selon vos besoins)
        public IActionResult GetFile(string folder, string fileName)
        {
            try
            {
                // Nettoyer les paramètres pour éviter les attaques par traversée de chemin
                folder = CleanFolderName(folder);
                fileName = CleanFileName(fileName);

                var filePath = Path.Combine(_uploadsBasePath, folder, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    _logger.LogWarning($"File not found: {filePath}");
                    return NotFound(new { error = "Fichier non trouvé." });
                }

                // Déterminer le type MIME en fonction de l'extension
                var contentType = GetContentType(Path.GetExtension(fileName));

                // Lire et retourner le fichier
                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving file {folder}/{fileName}");
                return StatusCode(500, new { error = "Une erreur est survenue lors de la récupération du fichier." });
            }
        }

        [HttpDelete("files/{folder}/{fileName}")]
        public IActionResult DeleteFile(string folder, string fileName)
        {
            try
            {
                // Nettoyer les paramètres
                folder = CleanFolderName(folder);
                fileName = CleanFileName(fileName);

                var filePath = Path.Combine(_uploadsBasePath, folder, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    _logger.LogWarning($"Attempt to delete non-existent file: {filePath}");
                    return NotFound(new { error = "Fichier non trouvé." });
                }

                // Vérifier si l'utilisateur a le droit de supprimer ce fichier
                // Implémentez votre logique d'autorisation ici

                // Supprimer le fichier
                System.IO.File.Delete(filePath);
                _logger.LogInformation($"File deleted: {filePath}");

                return Ok(new { message = "Fichier supprimé avec succès." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting file {folder}/{fileName}");
                return StatusCode(500, new { error = "Une erreur est survenue lors de la suppression du fichier." });
            }
        }

        #region Helper Methods

        private string GetUniqueFileName(string fileName)
        {
            // Générer un nom de fichier unique en préfixant avec un timestamp et un GUID
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var guid = Guid.NewGuid().ToString("N").Substring(0, 8);
            var extension = Path.GetExtension(fileName);
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

            // Nettoyer le nom du fichier pour éviter les caractères problématiques
            nameWithoutExtension = CleanFileName(nameWithoutExtension);

            return $"{timestamp}_{guid}_{nameWithoutExtension}{extension}";
        }

        private string CleanFolderName(string folder)
        {
            // Nettoyer le nom du dossier pour éviter les injections de chemin
            if (string.IsNullOrWhiteSpace(folder))
            {
                return "general";
            }

            // Remplacer les caractères non alphanumériques par des underscores
            var cleanFolder = new string(folder.Where(c => char.IsLetterOrDigit(c) || c == '_' || c == '-').ToArray());

            // Éviter les dossiers qui commencent par des points (dossiers cachés)
            if (cleanFolder.StartsWith("."))
            {
                cleanFolder = "f" + cleanFolder;
            }

            return string.IsNullOrWhiteSpace(cleanFolder) ? "general" : cleanFolder;
        }

        private string CleanFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return "file";
            }

            // Garder l'extension mais nettoyer le nom
            var extension = Path.GetExtension(fileName);
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

            // Remplacer les caractères non alphanumériques par des underscores
            var cleanName = new string(nameWithoutExtension.Where(c => char.IsLetterOrDigit(c) || c == '_' || c == '-').ToArray());

            return string.IsNullOrWhiteSpace(cleanName) ? "file" + extension : cleanName + extension;
        }
        // NEW METHOD: Upload profile photo
        [HttpPost("upload-profile-photo")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadProfilePhoto(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("Profile photo upload attempt with empty file");
                    return BadRequest(new { error = "Aucun fichier n'a été fourni." });
                }

                // Validate file size
                if (file.Length > _maxFileSize)
                {
                    _logger.LogWarning($"Profile photo size exceeds limit: {file.Length} bytes");
                    return BadRequest(new { error = $"La taille de la photo dépasse la limite autorisée (5 MB)." });
                }

                // Validate file type
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(fileExtension))
                {
                    _logger.LogWarning($"Invalid profile photo extension: {fileExtension}");
                    return BadRequest(new { error = "Type de fichier non autorisé. Seules les images JPG, PNG, GIF et WEBP sont acceptées." });
                }

                // Get current user
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ??
                             User.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                             User.FindFirstValue("sub") ??
                             User.FindFirstValue("uid") ??
                             User.FindFirstValue("userId") ??
                             User.FindFirstValue("nameid");

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("User ID not found in token for profile photo upload");
                    return Unauthorized(new { error = "Utilisateur non identifié." });
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found for profile photo upload: {UserId}", userId);
                    return NotFound(new { error = "Utilisateur non trouvé." });
                }

                // Create profile photos directory
                var profilePhotosFolder = Path.Combine(_uploadsBasePath, "profile-photos");
                if (!Directory.Exists(profilePhotosFolder))
                {
                    Directory.CreateDirectory(profilePhotosFolder);
                }

                // Generate unique filename
                var uniqueFileName = GetUniqueFileName(file.FileName);
                var filePath = Path.Combine(profilePhotosFolder, uniqueFileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Delete old photo if exists
                if (!string.IsNullOrEmpty(user.Photo))
                {
                    var oldPhotoPath = Path.Combine(_uploadsBasePath, user.Photo);
                    if (System.IO.File.Exists(oldPhotoPath))
                    {
                        try
                        {
                            System.IO.File.Delete(oldPhotoPath);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to delete old profile photo: {PhotoPath}", user.Photo);
                        }
                    }
                }

                // Update user profile with relative path
                var relativePath = $"profile-photos/{uniqueFileName}";
                user.Photo = relativePath;
                var updateResult = await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    // Delete the new photo if update failed
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                    return BadRequest(new
                    {
                        error = "Échec de la mise à jour du profil",
                        errors = updateResult.Errors.Select(e => e.Description)
                    });
                }

                // Build accessible URL
                var fileUrl = $"{Request.Scheme}://{Request.Host}/api/FileUpload/files/{relativePath}";

                return Ok(new
                {
                    message = "Photo de profil mise à jour avec succès",
                    filePath = relativePath,
                    fileUrl = fileUrl
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during profile photo upload");
                return StatusCode(500, new { error = "Une erreur est survenue lors de l'upload de la photo de profil." });
            }
        }

        [HttpGet("files/{*filePath}")]
        [AllowAnonymous]
        public IActionResult GetFile(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                    return BadRequest();

                // Nettoyer le chemin
                filePath = filePath.Replace("..", "")
                                  .Replace("//", "/")
                                  .TrimStart('/');

                var fullPath = Path.Combine(_uploadsBasePath, filePath);

                if (!System.IO.File.Exists(fullPath))
                {
                    _logger.LogWarning($"File not found: {fullPath}");
                    return NotFound(new { error = "Fichier non trouvé." });
                }

                var contentType = GetContentType(Path.GetExtension(fullPath));
                var fileBytes = System.IO.File.ReadAllBytes(fullPath);
                return File(fileBytes, contentType, Path.GetFileName(fullPath));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving file {filePath}");
                return StatusCode(500, new { error = "Erreur lors de la récupération du fichier." });
            }
        }
        private string GetContentType(string extension)
        {
            switch (extension.ToLowerInvariant())
            {
                case ".pdf":
                    return "application/pdf";
                case ".doc":
                    return "application/msword";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                default:
                    return "application/octet-stream";
            }
        }

        #endregion
    }
}
