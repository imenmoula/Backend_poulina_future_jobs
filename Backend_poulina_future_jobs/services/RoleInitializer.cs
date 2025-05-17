using Microsoft.AspNetCore.Identity;

namespace Backend_poulina_future_jobs.Services
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            var roles = new[]
            {
                new { Name = "Admin", Id = Guid.NewGuid() },
                new { Name = "Candidate", Id = Guid.NewGuid() },
                new { Name = "Recruteur", Id = Guid.NewGuid() }
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role.Name))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>
                    {
                        Id = role.Id,
                        Name = role.Name,
                        NormalizedName = role.Name.ToUpper()
                    });
                }
            }
        }
    }
}