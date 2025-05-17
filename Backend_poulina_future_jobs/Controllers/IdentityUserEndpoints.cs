using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Net.Mail;
using System.Net;
using Backend_poulina_future_jobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Backend_poulina_future_jobs.Controllers
{

    public class RequestResetPasswordDto 
    {
        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Email invalide")]
        public string email { get; set; } = string.Empty;
    }
    // Ajout de la classe ResetPasswordDto manquante
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Email invalide")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le token est requis")]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Le mot de passe doit contenir entre 6 et 100 caractères")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "La confirmation du mot de passe est requise")]
        [Compare("Password", ErrorMessage = "Le mot de passe et la confirmation doivent correspondre")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class UserRegistrationModel
    {
        [Required(ErrorMessage = "L'email est requis")]
        [EmailAddress(ErrorMessage = "Email invalide")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le mot de passe est requis")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Le mot de passe doit contenir entre 6 et 100 caractères")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "La confirmation du mot de passe est requise")]
        [Compare("Password", ErrorMessage = "Le mot de passe et la confirmation doivent correspondre")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom complet est requis")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le nom est requis")]
        public string Nom { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le prénom est requis")]
        public string Prenom { get; set; } = string.Empty;

        public string Role { get; set; } = "Candidate";
    }

    public class LoginModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    public static class IdentityUserEndpoints
    {
        public static IEndpointRouteBuilder MapIdentityUserEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/signup", CreatUser)
               .AllowAnonymous();

            app.MapPost("/signin", SignIn)
               .AllowAnonymous();

            app.MapPost("/refresh-token", RefreshToken)
               .AllowAnonymous();

            app.MapPost("/reset-password", ResetPassword)
               .AllowAnonymous();
               
            // Ajout de l'endpoint de demande de réinitialisation de mot de passe
            app.MapPost("/request-reset-password", RequestResetPassword)
               .AllowAnonymous()
               .WithName("RequestResetPassword")
               .WithDisplayName("Demander une réinitialisation de mot de passe")
               .WithDescription("Envoie un email avec un token pour réinitialiser le mot de passe");

            return app;
        }

        [AllowAnonymous]
        public static async Task<IResult> CreatUser(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            [FromBody] UserRegistrationModel userRegistrationModel,
            ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger("IdentityUserEndpoints");
            try
            {
                logger.LogInformation("CreatUser endpoint called for email: {Email}", userRegistrationModel.Email);

                var validationResults = new List<ValidationResult>();
                var validationContext = new ValidationContext(userRegistrationModel);
                if (!Validator.TryValidateObject(userRegistrationModel, validationContext, validationResults, true))
                {
                    var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();
                    logger.LogWarning("Validation failed for user registration: {Errors}", string.Join(", ", errors));
                    return Results.BadRequest(new { message = "Validation échouée", errors });
                }

                if (string.IsNullOrEmpty(userRegistrationModel.Role))
                {
                    userRegistrationModel.Role = "Candidate";
                    logger.LogInformation("No role specified, defaulting to: {Role}", userRegistrationModel.Role);
                }

                if (!await roleManager.RoleExistsAsync(userRegistrationModel.Role))
                {
                    logger.LogInformation("Role {Role} does not exist, creating it.", userRegistrationModel.Role);
                    var newRole = new IdentityRole<Guid> { Name = userRegistrationModel.Role, Id = Guid.NewGuid() };
                    var createRoleResult = await roleManager.CreateAsync(newRole);
                    if (!createRoleResult.Succeeded)
                    {
                        var errors = string.Join(", ", createRoleResult.Errors.Select(e => e.Description));
                        logger.LogWarning("Failed to create role {Role}: {Errors}", userRegistrationModel.Role, errors);
                        return Results.BadRequest(new { message = "Échec de la création du rôle", errors });
                    }
                }

                var user = new AppUser
                {
                    UserName = userRegistrationModel.Email,
                    Email = userRegistrationModel.Email,
                    FullName = userRegistrationModel.FullName,
                    Nom = userRegistrationModel.Nom,
                    Prenom = userRegistrationModel.Prenom
                };

                logger.LogInformation("Creating user with email: {Email}", user.Email);
                var result = await userManager.CreateAsync(user, userRegistrationModel.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    logger.LogWarning("Failed to create user with email {Email}: {Errors}", user.Email, errors);
                    return Results.BadRequest(new { message = "Échec de la création de l'utilisateur", errors });
                }

                logger.LogInformation("Assigning role {Role} to user with email: {Email}", userRegistrationModel.Role, user.Email);
                var roleResult = await userManager.AddToRoleAsync(user, userRegistrationModel.Role);
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    logger.LogWarning("Failed to assign role {Role} to user with email {Email}: {Errors}", userRegistrationModel.Role, user.Email, errors);
                    return Results.BadRequest(new { message = "Échec de l'attribution du rôle", errors });
                }

                logger.LogInformation("User created successfully with email: {Email}", user.Email);
                return Results.Ok(new { message = "Utilisateur enregistré avec succès" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred during user creation for email: {Email}", userRegistrationModel.Email);
                return Results.Problem(
                    detail: $"Une erreur est survenue lors de la création de l'utilisateur: {ex.Message}",
                    statusCode: 500);
            }
        }

        public static async Task<IResult> SignIn(
           UserManager<AppUser> userManager,
           IConfiguration configuration,
           [FromBody] LoginModel loginModel)
        {
            var user = await userManager.FindByEmailAsync(loginModel.Email);
            if (user != null && await userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                var userRoles = await userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName ?? ""),
                    new Claim("userId", user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("FullName", user.FullName ?? "Non défini"),
                    new Claim("Nom", user.Nom ?? "Non défini"),
                    new Claim("Prenom", user.Prenom ?? "Non défini")
                };

                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AppSettings:JWTSecret"] ?? throw new InvalidOperationException("JWTSecret non configuré")));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    issuer: configuration["AppSettings:Issuer"],
                    audience: configuration["AppSettings:Audience"],
                    claims: authClaims,
                    expires: DateTime.Now.AddHours(3), // Increased from 1 hour to 7 days
                    signingCredentials: creds
                );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                var refreshToken = Guid.NewGuid().ToString();
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30); // Increased from 7 days to 30 days
                await userManager.UpdateAsync(user);

                return Results.Ok(new
                {
                    Token = tokenString,
                    RefreshToken = refreshToken,
                    Email = user.Email,
                    Roles = userRoles,
                    Success = true
                });
            }
            return Results.Unauthorized();
        }

        public static async Task<IResult> RefreshToken(
            UserManager<AppUser> userManager,
            IConfiguration configuration,
            [FromBody] RefreshTokenRequest request)
        {
            var user = await userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);
            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return Results.BadRequest(new { message = "Invalid or expired refresh token" });
            }

            var userRoles = await userManager.GetRolesAsync(user);
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim("userId", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("FullName", user.FullName ?? "Non défini"),
                new Claim("Nom", user.Nom ?? "Non défini"),
                new Claim("Prenom", user.Prenom ?? "Non défini")
            };

            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AppSettings:JWTSecret"] ?? throw new InvalidOperationException("JWTSecret non configuré")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: configuration["AppSettings:Issuer"],
                audience: configuration["AppSettings:Audience"],
                claims: authClaims,
                expires: DateTime.Now.AddDays(7), // Ensure the new token also lasts 7 days
                signingCredentials: creds
            );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Results.Ok(new
            {
                Token = tokenString,
                Email = user.Email,
                Roles = userRoles,
                Success = true
            });
        }

        [AllowAnonymous]
        public static async Task<IResult> ResetPassword(
            UserManager<AppUser> userManager,
            ILoggerFactory loggerFactory,
            [FromBody] ResetPasswordDto resetPasswordDto)
        {
            var logger = loggerFactory.CreateLogger("IdentityUserEndpoints");
            logger.LogInformation("ResetPassword appelé pour l'email: {Email}", resetPasswordDto.Email);

            if (!Validator.TryValidateObject(resetPasswordDto, new ValidationContext(resetPasswordDto), null, true))
            {
                logger.LogWarning("Validation échouée pour la réinitialisation du mot de passe: {Email}", resetPasswordDto.Email);
                return Results.BadRequest(new { message = "Veuillez remplir tous les champs correctement." });
            }

            var user = await userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                logger.LogWarning("Utilisateur non trouvé pour la réinitialisation du mot de passe: {Email}", resetPasswordDto.Email);
                return Results.BadRequest(new { message = "Aucun utilisateur trouvé avec cette adresse email." });
            }

            try
            {
                // Décodage du token
                string decodedToken = Uri.UnescapeDataString(resetPasswordDto.Token);

                var resetPassResult = await userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDto.Password);
                if (!resetPassResult.Succeeded)
                {
                    var errors = resetPassResult.Errors.Select(e => e.Description).ToList();
                    logger.LogWarning("Échec de réinitialisation du mot de passe pour {Email}: {Errors}", 
                        resetPasswordDto.Email, string.Join(", ", errors));
                    
                    // Traduire les messages d'erreur courants en français
                    var frenchErrors = errors.Select(TranslateError).ToList();
                    
                    return Results.BadRequest(new { 
                        message = "La réinitialisation du mot de passe a échoué.", 
                        errors = frenchErrors 
                    });
                }

                logger.LogInformation("Mot de passe réinitialisé avec succès pour: {Email}", resetPasswordDto.Email);
                return Results.Ok(new { 
                    message = "Votre mot de passe a été réinitialisé avec succès. Vous pouvez maintenant vous connecter avec votre nouveau mot de passe.",
                    success = true 
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erreur lors de la réinitialisation du mot de passe pour: {Email}", resetPasswordDto.Email);
                return Results.Problem(
                    detail: "Une erreur est survenue lors de la réinitialisation du mot de passe.",
                    statusCode: 500);
            }
        }
        
        // Méthode pour traduire les messages d'erreur d'Identity en français
        private static string TranslateError(string errorMessage)
        {
            return errorMessage switch
            {
                "Invalid token." => "Le lien de réinitialisation est invalide ou a expiré. Veuillez faire une nouvelle demande.",
                "Passwords must have at least one non alphanumeric character." => "Le mot de passe doit contenir au moins un caractère spécial.",
                "Passwords must have at least one digit ('0'-'9')." => "Le mot de passe doit contenir au moins un chiffre.",
                "Passwords must have at least one uppercase ('A'-'Z')." => "Le mot de passe doit contenir au moins une lettre majuscule.",
                "Passwords must have at least one lowercase ('a'-'z')." => "Le mot de passe doit contenir au moins une lettre minuscule.",
                _ => errorMessage
            };
        }
     
        // Conversion de la méthode d'instance en méthode statique
        [AllowAnonymous]
        public static async Task<IResult> RequestResetPassword(
            UserManager<AppUser> userManager,
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            [FromBody] RequestResetPasswordDto requestResetPasswordDto)
        {
            var logger = loggerFactory.CreateLogger("IdentityUserEndpoints");
            logger.LogInformation("RequestResetPassword appelé pour l'email: {Email}", requestResetPasswordDto.email);

            var user = await userManager.FindByEmailAsync(requestResetPasswordDto.email);
            if (user == null)
            {
                logger.LogWarning("Aucun utilisateur trouvé avec l'email: {Email}", requestResetPasswordDto.email);
                // Message délibérément vague pour des raisons de sécurité (ne pas révéler qu'un compte existe ou non)
                return Results.Ok(new { message = "Si votre adresse email est enregistrée dans notre système, vous recevrez un email avec les instructions pour réinitialiser votre mot de passe." });
            }

            try
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);
                var encodedToken = Uri.EscapeDataString(token); // important pour les URLs
                
                // Récupération des paramètres de configuration pour l'envoi d'email
                var frontendUrl = configuration["AppSettings:FrontendUrl"] ?? "http://localhost:4200";
                var resetPasswordUrl = $"{frontendUrl}/reset-password?email={Uri.EscapeDataString(requestResetPasswordDto.email)}&token={encodedToken}";
                
                // Contenu de l'email en français
                var subject = "Réinitialisation de votre mot de passe - Poulina Future Jobs";
                var message = $@"
                <html>
                <body>
                    <h1>Réinitialisation de votre mot de passe</h1>
                    <p>Bonjour {user.FullName},</p>
                    <p>Vous avez demandé à réinitialiser votre mot de passe sur la plateforme Poulina Future Jobs.</p>
                    <p>Pour procéder à la réinitialisation, veuillez cliquer sur le lien ci-dessous :</p>
                    <p><a href='{resetPasswordUrl}'>Réinitialiser mon mot de passe</a></p>
                    <p>Ce lien expirera dans 24 heures.</p>
                    <p>Si vous n'êtes pas à l'origine de cette demande, vous pouvez ignorer cet email en toute sécurité.</p>
                    <p>Cordialement,<br>L'équipe Poulina Future Jobs</p>
                </body>
                </html>";

                // Envoi de l'email avec le lien de réinitialisation
                await SendEmailAsync(configuration, user.Email, subject, message, logger);

                logger.LogInformation("Email de réinitialisation envoyé avec succès à: {Email}", requestResetPasswordDto.email);
                return Results.Ok(new { 
                    message = "Si votre adresse email est enregistrée dans notre système, vous recevrez un email avec les instructions pour réinitialiser votre mot de passe.",
                    success = true 
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erreur lors de la génération du token pour: {Email}", requestResetPasswordDto.email);
                return Results.Problem(
                    detail: "Une erreur est survenue lors de la demande de réinitialisation du mot de passe.",
                    statusCode: 500);
            }
        }

        // Méthode d'aide pour envoyer des emails en utilisant System.Net.Mail (intégré à .NET)
        private static async Task SendEmailAsync(IConfiguration configuration, string email, string subject, string htmlMessage, ILogger logger)
        {
            try
            {
                // Récupération des paramètres de configuration
                var mailServer = configuration["EmailSettings:Server"];
                var mailPort = int.Parse(configuration["EmailSettings:Port"] ?? "587");
                var mailSender = configuration["EmailSettings:Sender"];
                var mailPassword = configuration["EmailSettings:Password"];
                var enableSsl = bool.Parse(configuration["EmailSettings:UseSsl"] ?? "true");

                // Création du message
                var message = new MailMessage
                {
                    From = new MailAddress(mailSender, "Poulina Future Jobs"),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };
                message.To.Add(new MailAddress(email));

                // Configuration du client SMTP
                using var client = new SmtpClient(mailServer, mailPort)
                {
                    EnableSsl = enableSsl,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(mailSender, mailPassword)
                };

                // Envoi du message de façon asynchrone
                await client.SendMailAsync(message);
                
                logger.LogInformation("Email envoyé avec succès à: {Email}", email);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erreur lors de l'envoi de l'email à: {Email}", email);
                throw; // Re-lancer l'exception pour la gestion en amont
            }
        }
    }
}