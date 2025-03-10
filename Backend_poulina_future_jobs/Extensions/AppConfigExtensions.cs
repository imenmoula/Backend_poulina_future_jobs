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

        public static IServiceCollection AddAppConfig(
                                                       this IServiceCollection services,
                                                       IConfiguration config)
        {

            services.Configure<AppSettings>(config.GetSection("AppSettings"));

            return services;

        }
    }
}
