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
using Backend_poulina_future_jobs.Extensions;
using Backend_poulina_future_jobs.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

 builder.Services.AddControllers();

// Configurer CORS au niveau des services (optionnel, mais recommand�)


///les principale  extention    auth,identity,dbcontext
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
var app = builder.Build();


app.ConfigureSwaggerExplorer()
   .ConfigureCors(builder.Configuration)
   .AddIdentityAuthMiddlewares();



app.MapControllers();
app.MapGroup("/api")
    .MapIdentityApi<AppUser>();
app.MapGroup("/api")
    .MapIdentityUserEndpoints()
    .MapAccountEndpoints()
    .MapAuthorizationDemoEndpoints();

app.UseStaticFiles();
app.UseCors("AllowAngular");


app.Run();

