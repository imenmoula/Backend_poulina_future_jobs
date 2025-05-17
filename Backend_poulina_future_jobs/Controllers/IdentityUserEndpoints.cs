using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend_poulina_future_jobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Backend_poulina_future_jobs.Controllers
{
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
               .AllowAnonymous();

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
            [FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!Validator.TryValidateObject(resetPasswordDto, new ValidationContext(resetPasswordDto), null, true))
            {
                return Results.BadRequest(new { message = "Requête invalide, données manquantes ou incorrectes." });
            }

            var user = await userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null)
            {
                return Results.BadRequest(new { message = "Aucun utilisateur trouvé avec cet e-mail." });
            }

            // Decode token if sent via email (URL encoding)
            string decodedToken = Uri.UnescapeDataString(resetPasswordDto.Token);

            var resetPassResult = await userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDto.Password);
            if (!resetPassResult.Succeeded)
            {
                var errors = resetPassResult.Errors.Select(e => e.Description).ToList();
                return Results.BadRequest(new { message = "Échec de la réinitialisation du mot de passe", errors });
            }

            return Results.Ok(new { message = "Mot de passe réinitialisé avec succès" });
        }
     
        // Conversion de la méthode d'instance en méthode statique
        [AllowAnonymous]
        public static async Task<IResult> RequestResetPassword(
            UserManager<AppUser> userManager,
            [FromBody] string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Results.BadRequest(new { message = "Aucun utilisateur trouvé avec cet e-mail." });
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token); // important for URLs

            // Send this token via email or return it for testing
            return Results.Ok(new { message = "Token généré", token = encodedToken });
        }
    }
}