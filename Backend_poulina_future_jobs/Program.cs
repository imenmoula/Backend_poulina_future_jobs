using Backend_poulina_future_jobs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.Eventing.Reader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//services from identity core 
builder.Services
.AddIdentityApiEndpoints<AppUser>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.Configure<IdentityOptions>(options=>
 { 
options.Password.RequireDigit = false;
options.Password.RequireUppercase = false;
options.Password.RequireLowercase = false;
options.User.RequireUniqueEmail = true;
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevDB")));


builder.Services.AddAuthentication(X =>
            {
                X.DefaultAuthenticateScheme =
                X.DefaultChallengeScheme =
                X.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(Y =>
            {
              Y.SaveToken=false;
                Y.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey=new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["AppSettings:JWTSecret"]!))

                };
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();



#region Config.CORS
app.UseCors(options =>
options.WithOrigins("http://localhost:4200")
        .AllowAnyMethod()
        .AllowAnyHeader()
        );
#endregion



app
    .MapGroup("/api")
    .MapIdentityApi<AppUser>();



app.MapPost("/api/signup", async (
    UserManager<AppUser> userManager,
    [FromBody] UserRegistrationModel userRegistrationModel
    ) =>
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
});



app.MapPost("/api/signin", async (
    UserManager<AppUser> userManager,
    IConfiguration configuration, // Ajout de IConfiguration pour accéder aux paramètres
    [FromBody] LoginModel loginModel) =>
{
    var user = await userManager.FindByEmailAsync(loginModel.Email);
    if (user != null && await userManager.CheckPasswordAsync(user, loginModel.Password))
    {
        var signInKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["AppSettings:JWTSecret"]!)
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
});

app.Run();

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