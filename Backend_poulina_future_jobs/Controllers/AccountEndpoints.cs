using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Client;

namespace Backend_poulina_future_jobs.Controllers
{
    public static  class AccountEndpoints
    {

        public static IEndpointRouteBuilder MapAccountEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/UserProfile", GetUserProfiele);
            return app;
        }
        [Authorize]
        private static string GetUserProfiele()
        {
            return "User Profile";
        }
    }
}
