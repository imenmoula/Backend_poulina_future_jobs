using Backend_poulina_future_jobs.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_poulina_future_jobs.Extensions
{
    public static class AppConfigExtensions
    {

        public static WebApplication ConfigureCors(
                                                    this WebApplication app,
                                                    IConfiguration config)
        {
            app.UseCors(options =>
            options.WithOrigins("http://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    );


            return app;

        }
    //    public static WebApplication ConfigureCors(this WebApplication app, IConfiguration config)
    //{
    //    var allowedOrigins = config.GetSection("Cors:AllowedOrigins").Get<string[]>() 
    //        ?? new[] { "http://localhost:4200" };

    //    app.UseCors(builder =>
    //    {
    //        builder.WithOrigins(allowedOrigins)
    //               .AllowAnyMethod()
    //               .AllowAnyHeader()
    //               .AllowCredentials();
    //    });

    //    return app;
    //}



        public static IServiceCollection AddAppConfig(
                                                       this IServiceCollection services,
                                                       IConfiguration config)
        {

            services.Configure<AppSettings>(config.GetSection("AppSettings"));

            return services;

        }
    }
}
