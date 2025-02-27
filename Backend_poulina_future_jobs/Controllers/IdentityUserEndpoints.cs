//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using Backend_poulina_future_jobs.Models;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Authorization.Infrastructure;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Options;
//using Microsoft.IdentityModel.Tokens;

//namespace Backend_poulina_future_jobs.Controllers
//{
//    public class UserRegistrationModel
//    {
//        public string Email { get; set; }
//        public string Password { get; set; }
//        public string FullName { get; set; }
//        public String Role { get; set; }
//        public string Poste { get; set; }
//        public int? filialeId { get; set; }

//    }

//    public class LoginModel
//    {
//        public string Email { get; set; }
//        public string Password { get; set; }
//    }
//    public static class IdentityUserEndpoints
//    {

//        public static IEndpointRouteBuilder MapIdentityUserEndpoints(this IEndpointRouteBuilder app)
//        {
//            app.MapPost("/signup", CreatUser);

//            app.MapPost("/signin", SignIn);

//            return app;

//        }


//        //[AllowAnonymous]
//        //public static async Task<IResult> CreatUser(
//        //                      UserManager<AppUser> userManager,
//        //                      RoleManager<IdentityRole> roleManager, // Ajout du RoleManager
//        //                      [FromBody] UserRegistrationModel userRegistrationModel)
//        //{
//        //    // Vérifier si le rôle existe avant d'ajouter l'utilisateur
//        //    if (!await roleManager.RoleExistsAsync(userRegistrationModel.Role))
//        //    {
//        //        return Results.BadRequest(new { message = "Role does not exist" });
//        //    }

//        //    // Création d'un nouvel utilisateur
//        //    AppUser user = new AppUser()
//        //    {
//        //        UserName = userRegistrationModel.Email,
//        //        Email = userRegistrationModel.Email,
//        //        FullName = userRegistrationModel.FullName,
//        //        Poste = userRegistrationModel.Poste,
//        //        filialeId = userRegistrationModel.filialeId,
//        //    };

//        //    var result = await userManager.CreateAsync(
//        //        user, userRegistrationModel.Password);

//        //    // Vérifier si l'utilisateur a bien été créé
//        //    if (!result.Succeeded)
//        //    {
//        //        return Results.BadRequest(result);
//        //    }

//        //    // Recharger l'utilisateur après la création
//        //    user = await userManager.FindByEmailAsync(userRegistrationModel.Email);
//        //    if (user == null)
//        //    {
//        //        return Results.BadRequest(new { message = "User creation failed" });
//        //    }

//        //    // Ajouter l'utilisateur au rôle
//        //    var roleResult = await userManager.AddToRoleAsync(user, userRegistrationModel.Role);
//        //    if (!roleResult.Succeeded)
//        //    {
//        //        return Results.BadRequest(new { message = "Failed to assign role" });
//        //    }

//        //    return Results.Ok(new { message = "User registered successfully" });
//        //}
//        [AllowAnonymous]
//        public static async Task<IResult> CreatUser(
//                        UserManager<AppUser> userManager,
//                        RoleManager<IdentityRole> roleManager,
//                        [FromBody] UserRegistrationModel userRegistrationModel)
//        {
//            // Définir le rôle par défaut si aucun n'est spécifié
//            if (string.IsNullOrEmpty(userRegistrationModel.Role))
//            {
//                userRegistrationModel.Role = "Candidate";
//            }

//            // Vérifier si le rôle existe avant d'ajouter l'utilisateur
//            if (!await roleManager.RoleExistsAsync(userRegistrationModel.Role))
//            {
//                // Créer le rôle s'il n'existe pas
//                await roleManager.CreateAsync(new IdentityRole(userRegistrationModel.Role));
//            }

//            // Création d'un nouvel utilisateur
//            AppUser user = new AppUser()
//            {
//                UserName = userRegistrationModel.Email,
//                Email = userRegistrationModel.Email,
//                FullName = userRegistrationModel.FullName,
//                Poste = userRegistrationModel.Poste,
//                filialeId = userRegistrationModel.filialeId,
//            };

//            var result = await userManager.CreateAsync(
//                user, userRegistrationModel.Password);

//            // Vérifier si l'utilisateur a bien été créé
//            if (!result.Succeeded)
//            {
//                return Results.BadRequest(result);
//            }

//            // Recharger l'utilisateur après la création
//            user = await userManager.FindByEmailAsync(userRegistrationModel.Email);
//            if (user == null)
//            {
//                return Results.BadRequest(new { message = "Échec de la création de l'utilisateur" });
//            }

//            // Ajouter l'utilisateur au rôle
//            var roleResult = await userManager.AddToRoleAsync(user, userRegistrationModel.Role);
//            if (!roleResult.Succeeded)
//            {
//                return Results.BadRequest(new { message = "Échec de l'attribution du rôle" });
//            }

//            return Results.Ok(new { message = "Utilisateur enregistré avec succès" });
//        }

//        [AllowAnonymous]
//        public static async Task<IResult> SignIn(
//                                      UserManager<AppUser> userManager,
//                                      IConfiguration configuration, // Ajout de IConfiguration pour accéder aux paramètres
//                                      [FromBody] LoginModel loginModel,
//                                      IOptions<AppSettings> appSettings)
//        {

//            var user = await userManager.FindByEmailAsync(loginModel.Email);
//            if (user != null && await userManager.CheckPasswordAsync(user, loginModel.Password))
//            {
//                var roles = await userManager.GetRolesAsync(user);
//                var signInKey = new SymmetricSecurityKey(
//                    Encoding.UTF8.GetBytes(appSettings.Value.JwtSecret)
//                );
//                ClaimsIdentity claims = new ClaimsIdentity(new Claim[]
//                 {
//                new Claim("userId", user.Id.ToString()),
//                new Claim("poste", user.Poste.ToString()),

//                new Claim(ClaimTypes.Role, roles.First()), // Correction ici
//                  });

//                if (user.filialeId != null)
//                {
//                    claims.AddClaim(new Claim("filialeId", user.filialeId.ToString()));
//                }

//                var tokenDescriptor = new SecurityTokenDescriptor
//                {
//                    Subject = new ClaimsIdentity(new Claim[]
//                    {
//                    new Claim("userID", user.Id.ToString()) // Assurez-vous que cette ligne est présente
//                    }),
//                    Expires = DateTime.UtcNow.AddDays(10), // Correction de "Expire" ? "Expires"
//                    SigningCredentials = new SigningCredentials(
//                        signInKey,
//                        SecurityAlgorithms.HmacSha256Signature
//                    )
//                };

//                var tokenHandler = new JwtSecurityTokenHandler();
//                var token = tokenHandler.CreateToken(tokenDescriptor);
//                var tokenString = tokenHandler.WriteToken(token);

//                return Results.Ok(new { Token = tokenString });
//            }
//            else
//            {
//                return Results.BadRequest(new { message = "Username or password is incorrect" });
//            }
//        }
//    }


//    }
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend_poulina_future_jobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Backend_poulina_future_jobs.Controllers
{
    public class UserRegistrationModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = "Candidate"; // Rôle par défaut
        public string Poste { get; set; } = string.Empty;
        public int? FilialeId { get; set; }
    }

    public class LoginModel
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public static class IdentityUserEndpoints
    {
        public static IEndpointRouteBuilder MapIdentityUserEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/signup", CreatUser)
               .AllowAnonymous();

            app.MapPost("/signin", SignIn)
               .AllowAnonymous();

            return app;
        }

        [AllowAnonymous]
        public static async Task<IResult> CreatUser(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager,
            [FromBody] UserRegistrationModel userRegistrationModel)
        {
            // Définir le rôle par défaut si aucun n'est spécifié
            if (string.IsNullOrEmpty(userRegistrationModel.Role))
            {
                userRegistrationModel.Role = "Candidate";
            }

            // Vérifier si le rôle existe, sinon le créer
            if (!await roleManager.RoleExistsAsync(userRegistrationModel.Role))
            {
                var createRoleResult = await roleManager.CreateAsync(new IdentityRole(userRegistrationModel.Role));
                if (!createRoleResult.Succeeded)
                {
                    var errors = string.Join(", ", createRoleResult.Errors.Select(e => e.Description));
                    return Results.BadRequest(new { message = "Échec de la création du rôle", errors });
                }
            }

            // Création de l'utilisateur
            var user = new AppUser
            {
                UserName = userRegistrationModel.Email,
                Email = userRegistrationModel.Email,
                FullName = userRegistrationModel.FullName,
                Poste = userRegistrationModel.Poste,
                filialeId = userRegistrationModel.FilialeId
            };

            var result = await userManager.CreateAsync(user, userRegistrationModel.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Results.BadRequest(new { message = "Échec de la création de l'utilisateur", errors });
            }

            // Recharger l'utilisateur après la création pour s'assurer qu'il est bien enregistré
            user = await userManager.FindByEmailAsync(userRegistrationModel.Email);
            if (user == null)
            {
                return Results.BadRequest(new { message = "Utilisateur introuvable après création" });
            }

            // Ajouter l'utilisateur au rôle
            var roleResult = await userManager.AddToRoleAsync(user, userRegistrationModel.Role);
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                return Results.BadRequest(new { message = "Échec de l'attribution du rôle", errors });
            }

            return Results.Ok(new { message = "Utilisateur enregistré avec succès" });
        }

        [AllowAnonymous]
        public static async Task<IResult> SignIn(
            UserManager<AppUser> userManager,
            IConfiguration configuration,
            [FromBody] LoginModel loginModel)
        {
            var user = await userManager.FindByEmailAsync(loginModel.Email);
            if (user != null && await userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                var userRoles = await userManager.GetRolesAsync(user);

                // Créer les claims
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim("userId", user.Id),
                    new Claim("poste", user.Poste ?? "Non défini"),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                // Ajouter tous les rôles
                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                if (user.filialeId.HasValue)
                {
                    authClaims.Add(new Claim("filialeId", user.filialeId.Value.ToString()));
                }

                // Générer le token JWT
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AppSettings:JWTSecret"]!));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    claims: authClaims,
                    expires: DateTime.UtcNow.AddDays(10),
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Results.Ok(new { Token = tokenString });
            }

            return Results.Unauthorized();
        }
    }
}

