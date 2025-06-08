

using Backend_poulina_future_jobs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using Backend_poulina_future_jobs.Extensions;
using Backend_poulina_future_jobs.Services;
using Backend_poulina_future_jobs.Controllers;
using Microsoft.Extensions.FileProviders;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddScoped<UserManager<AppUser>>(); // Fixed: AddScoped was incomplete
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

// Principal extensions: auth, identity, dbcontext
builder.Services.AddSwaggerExplorer()
                .InjectDbContext(builder.Configuration)
                .AddAppConfig(builder.Configuration)
                .AddIdentityHandlersAndStores()
                .ConfigureIdentityOptions()
                .AddIdentityAuth(builder.Configuration);

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", builder =>
    {
        builder.WithOrigins("http://localhost:4200")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials();
    });
});
// Configuration personnalisée des services
builder.Services.AddTransient<IEmailService, EmailService>(); // Doit être avant Build()

//ajouter les controlller
var app = builder.Build();

// Activer le middleware pour gérer les exceptions et afficher les détails (en développement uniquement)
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.WebRootPath, "uploads")),
    RequestPath = "/uploads"
});

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await RoleInitializer.InitializeAsync(services);
}

app.ConfigureSwaggerExplorer()
   .ConfigureCors(builder.Configuration)
   .AddIdentityAuthMiddlewares();

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole(); // Permet d'afficher les logs dans la console
});
var logger = loggerFactory.CreateLogger<Program>();

app.UseCors("AllowAngular");

app.MapControllers();

app.MapGroup("/api")
    .MapIdentityApi<AppUser>();
app.MapGroup("/api")
    .MapIdentityUserEndpoints()
    .MapAuthorizationDemoEndpoints();
app.MapAccountEndpoints(); // Ajout des endpoints définis dans AccountEndpoints.cs


app.Run();



