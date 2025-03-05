using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Backend_poulina_future_jobs.Models
{

    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Departement> Departements { get; set; }
        public DbSet<Filiale> Filiales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Departement>()
                .HasOne(d => d.Filiale)
                .WithMany(f => f.Departements)
                .HasForeignKey(d => d.IdFiliale)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
