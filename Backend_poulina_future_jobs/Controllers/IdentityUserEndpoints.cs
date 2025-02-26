using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend_poulina_future_jobs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Backend_poulina_future_jobs.Controllers
{
    public class UserRegistrationModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public String Role { get; set; }
        public string Poste { get; set; }
        public int? filialeId { get; set; } 

    }

    public class LoginModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public static class IdentityUserEndpoints
    {

        public static IEndpointRouteBuilder MapIdentityUserEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/signup", CreatUser);

            app.MapPost("/signin", SignIn);

            return app;

        }


        [AllowAnonymous]
        public static async Task<IResult> CreatUser(
                              UserManager<AppUser> userManager,
                              RoleManager<IdentityRole> roleManager, // Ajout du RoleManager
                              [FromBody] UserRegistrationModel userRegistrationModel)
        {
            // Vérifier si le rôle existe avant d'ajouter l'utilisateur
            if (!await roleManager.RoleExistsAsync(userRegistrationModel.Role))
            {
                return Results.BadRequest(new { message = "Role does not exist" });
            }

            // Création d'un nouvel utilisateur
            AppUser user = new AppUser()
            {
                UserName = userRegistrationModel.Email,
                Email = userRegistrationModel.Email,
                FullName = userRegistrationModel.FullName,
                Poste = userRegistrationModel.Poste,
                filialeId = userRegistrationModel.filialeId,
            };

            var result = await userManager.CreateAsync(user, userRegistrationModel.Password);

            // Vérifier si l'utilisateur a bien été créé
            if (!result.Succeeded)
            {
                return Results.BadRequest(result);
            }

            // Recharger l'utilisateur après la création
            user = await userManager.FindByEmailAsync(userRegistrationModel.Email);
            if (user == null)
            {
                return Results.BadRequest(new { message = "User creation failed" });
            }

            // Ajouter l'utilisateur au rôle
            var roleResult = await userManager.AddToRoleAsync(user, userRegistrationModel.Role);
            if (!roleResult.Succeeded)
            {
                return Results.BadRequest(new { message = "Failed to assign role" });
            }

            return Results.Ok(new { message = "User registered successfully" });
        }


        [AllowAnonymous]
        public static async Task<IResult> SignIn(
                                      UserManager<AppUser> userManager,
                                      IConfiguration configuration, // Ajout de IConfiguration pour accéder aux paramètres
                                      [FromBody] LoginModel loginModel,
                                      IOptions<AppSettings> appSettings)
        {

            var user = await userManager.FindByEmailAsync(loginModel.Email);
            if (user != null && await userManager.CheckPasswordAsync(user, loginModel.Password))
            {
                var signInKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(appSettings.Value.JwtSecret)
                );

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                new Claim("userID", user.Id.ToString()) // Assurez-vous que cette ligne est présente
                    }),
                    Expires = DateTime.UtcNow.AddDays(10), // Correction de "Expire" ? "Expires"
                    SigningCredentials = new SigningCredentials(
                        signInKey,
                        SecurityAlgorithms.HmacSha256Signature
                    )
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Results.Ok(new { Token = tokenString });
            }
            else
            {
                return Results.BadRequest(new { message = "Username or password is incorrect" });
            }
        } 
    }
}
    



