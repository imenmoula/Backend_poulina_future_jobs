using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Backend_poulina_future_jobs.Models;

namespace Backend_poulina_future_jobs.Models
{

    public class AppDbContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>,Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        // public DbSet<Backend_poulina_future_jobs.Models.AppRole> Role { get; set; } = default!;
        public DbSet<IdentityRole<Guid>> AppRole { get; set; } // Pour accéder aux rôles
        public DbSet<Filiale> Filiales { get; set; }
        public DbSet<Departement> Departements { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Filiale>()
                .Property(f => f.IdFiliale)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<Departement>()
                .Property(d => d.IdDepartement)
                .HasDefaultValueSql("NEWID()");
;

            // Configuration optionnelle si besoin
            modelBuilder.Entity<Departement>()
         .HasOne(d => d.Filiale)
         .WithMany(f => f.Departements)
         .HasForeignKey(d => d.IdFiliale)
         .OnDelete(DeleteBehavior.Cascade);
            /********************************************************/

            // Définition de la relation Many-to-Many entre AppUser et IdentityRole
            modelBuilder.Entity<IdentityUserRole<Guid>>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });

            modelBuilder.Entity<IdentityUserRole<Guid>>()
                .HasOne<AppUser>()
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<IdentityUserRole<Guid>>()
                .HasOne<IdentityRole<Guid>>()
                .WithMany()
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Cascade);







        }

public DbSet<Backend_poulina_future_jobs.Models.AppUser> AppUser { get; set; } = default!;









    }
}
