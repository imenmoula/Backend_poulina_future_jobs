using Backend_poulina_future_jobs.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend_poulina_future_jobs.Extensions
{
    public static class EFCoreExtentions
    {
        public static IServiceCollection InjectDbContext(
            this IServiceCollection services,
            IConfiguration config)
        {

         services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(config.GetConnectionString("DevDB")));
            return services;

        }
    }
}
