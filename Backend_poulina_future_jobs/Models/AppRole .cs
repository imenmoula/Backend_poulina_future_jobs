using Microsoft.AspNetCore.Identity;
using System;

namespace Backend_poulina_future_jobs.Models
{
    public class AppRole : IdentityRole<Guid>
    {
        public AppRole() : base()
        {
            ConcurrencyStamp = null;
        }

        public AppRole(string roleName) : base(roleName)
        {
            NormalizedName = roleName.ToUpper();
            ConcurrencyStamp = null;
        }

        public string? ConcurrencyStamp { get; set; } 

    }
}
