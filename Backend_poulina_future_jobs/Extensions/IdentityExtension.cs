﻿//using System.Text;
//using Backend_poulina_future_jobs.Models;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.Extensions.Options;
//using Microsoft.IdentityModel.Tokens;

//namespace Backend_poulina_future_jobs.Extensions
//{
//    public  static class IdentityExtension
//    {
//        public static IServiceCollection AddIdentityHandlersAndStores(this IServiceCollection  services)
//        {
//            //services from identity core 
//            services.AddIdentityApiEndpoints<AppUser>()
//                     .AddRoles<IdentityRole>()
//                    .AddEntityFrameworkStores<AppDbContext>();
//            return services;

//        }
//        public static IServiceCollection ConfigureIdentityOptions( this  IServiceCollection services)
//        {
//            services.Configure<IdentityOptions>(options =>
//            {
//                options.Password.RequireDigit = false;
//                options.Password.RequireUppercase = false;
//                options.Password.RequireLowercase = false;
//                options.User.RequireUniqueEmail = true;
//            });
//            return services;
//        }
//        //auth=authentication+authorization
//        public static IServiceCollection AddIdentityAuth(
//          this IServiceCollection services,
//          IConfiguration config)
//        {
//            services
//            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//            .AddJwtBearer(y =>
//            {
//                y.SaveToken = false;
//                y.TokenValidationParameters = new TokenValidationParameters
//                {
//                    ValidateIssuerSigningKey = true,
//                    IssuerSigningKey = new SymmetricSecurityKey(
//                                  Encoding.UTF8.GetBytes(
//                                      config["AppSettings:JWTSecret"]!)),
//                    ValidateIssuer = false,
//                    ValidateAudience = false,
//                    ValidateLifetime = true,
//                    ClockSkew = TimeSpan.Zero
//                };
//            });
//            services.AddAuthorization(options =>
//            {
//                options.FallbackPolicy = new AuthorizationPolicyBuilder()
//                  .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
//                  .RequireAuthenticatedUser()
//                .Build();



//                options.AddPolicy("CandidateOnly", policy => policy.RequireRole("Candidate"));
//                options.AddPolicy("RecruiterOnly", policy => policy.RequireRole("Recruteur"));
//                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
//            });

//                return services;
//        }

//        public static WebApplication AddIdentityAuthMiddelwares(this WebApplication app)
//        {
//            app.UseAuthentication();
//            app.UseAuthorization();

//            return app;

//        }



//    }

//}
using System.Text;
using Backend_poulina_future_jobs.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Backend_poulina_future_jobs.Extensions
{
    public static class IdentityExtension
    {
        public static IServiceCollection AddIdentityHandlersAndStores(this IServiceCollection services)
        {
            services.AddIdentityApiEndpoints<AppUser>()
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>();
            return services;
        }

        public static IServiceCollection ConfigureIdentityOptions(this IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.User.RequireUniqueEmail = true;
            });
            return services;
        }

        public static IServiceCollection AddIdentityAuth(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(y =>
                {
                    y.SaveToken = true; // Changez à true pour conserver le token
                    y.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(config["AppSettings:JWTSecret"]!)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .Build();

                options.AddPolicy("CandidateOnly", policy => policy.RequireRole("Candidate"));
                options.AddPolicy("RecruiterOnly", policy => policy.RequireRole("Recruteur")); // Assurez-vous que c'est "Recruteur" ici
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            });

            return services;
        }

        public static WebApplication AddIdentityAuthMiddlewares(this WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }
    }
}
