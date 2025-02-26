using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using Backend_poulina_future_jobs.Models;

namespace Backend_poulina_future_jobs.Controllers
{
    public static class AccountEndpoints
    {
        public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/UserProfile", GetUserProfile);
            return app;
        }

        [Authorize]
        private static async Task<IResult> GetUserProfile(
    ClaimsPrincipal user,
    UserManager<AppUser> userManager)
        {
            var userIdClaim = user.Claims.FirstOrDefault(x => x.Type == "userID");
            if (userIdClaim == null)
            {
                return Results.BadRequest(new { message = "User ID claim not found" });
            }

            string userID = userIdClaim.Value;
            var userDetails = await userManager.FindByIdAsync(userID);
            if (userDetails == null)
            {
                return Results.NotFound(new { message = "User not found" });
            }

            return Results.Ok(
                new
                {
                    Email = userDetails.Email,
                    FullName = userDetails.FullName,
                });
        }

    }
}
