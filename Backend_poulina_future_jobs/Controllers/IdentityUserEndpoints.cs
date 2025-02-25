using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend_poulina_future_jobs.Models;
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

        public static async Task<IResult> CreatUser(
                                          UserManager<AppUser> userManager,
                                          [FromBody] UserRegistrationModel userRegistrationModel)
        {

            AppUser user = new AppUser()
            {
                UserName = userRegistrationModel.Email,
                Email = userRegistrationModel.Email,
                FullName = userRegistrationModel.FullName,
            };
            var result = await userManager.CreateAsync(
                user,
                userRegistrationModel.Password);

            if (result.Succeeded)
                return Results.Ok(result);
            else
                return Results.BadRequest(result);


        }




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
                    new Claim("User Id", user.Id.ToString())
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
    



