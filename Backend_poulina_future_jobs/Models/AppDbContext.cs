using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend_poulina_future_jobs.Models
{

    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Filiale> Filiales { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration pour générer automatiquement les GUID
            modelBuilder.Entity<Filiale>()
                .Property(f => f.IdFiliale)
                .HasDefaultValueSql("NEWID()"); // Utilise la fonction SQL NEWID() pour générer un GUID
        }

    }
}
