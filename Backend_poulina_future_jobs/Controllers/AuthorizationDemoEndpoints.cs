using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Backend_poulina_future_jobs.Controllers
{
    public static class AuthorizationDemoEndpoints
    {
        public static IEndpointRouteBuilder MapAuthorizationDemoEndpoints(this IEndpointRouteBuilder app)
        {
            

            app.MapGet("/AdminOnly", AdminOnly).RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
            app.MapGet("/Recruteur", [Authorize(Roles = "Recruteur")] () => "Recruteur");
            app.MapGet("/Candidate", [Authorize(Roles = "Candidate")] () => "Candidate");

            return app;
        }

        [Authorize(Roles = "Admin")]
        private static string AdminOnly()
        {
            return "Admin Only";
        }
    }
}