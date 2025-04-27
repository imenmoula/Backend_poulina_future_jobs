using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Backend_poulina_future_jobs.Models;
using Backend_poulina_future_jobs.Migrations;
using System.Text;
using System.Globalization;
using Backend_poulina_future_jobs.Controllers;

namespace Backend_poulina_future_jobs.Models
{

    public class AppDbContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        // public DbSet<Backend_poulina_future_jobs.Models.AppRole> Role { get; set; } = default!;
        public DbSet<IdentityRole<Guid>> AppRole { get; set; } // Pour accéder aux rôles
        public DbSet<Filiale> Filiales { get; set; }
        public DbSet<Departement> Departements { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<OffreEmploi> OffresEmploi { get; set; }
        public DbSet<Competence> Competences { get; set; }
        public DbSet<OffreCompetences> OffreCompetences { get; set; }        //public DbSet<Competence> Competences { get; set; }

        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Certificat> Certificats { get; set; }
        public DbSet<Candidature> Candidatures { get; set; }

        public DbSet<AppUserCompetence> AppUserCompetences { get; set; }
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
            /********************************************************************/
            // Configuration des relations pour OffreEmploi
            // Configure many-to-many relationship

            modelBuilder.Entity<OffreCompetences>()
                .HasKey(oc => new { oc.IdOffreEmploi, oc.IdCompetence });

            modelBuilder.Entity<OffreCompetences>()
                .HasOne(oc => oc.OffreEmploi)
                .WithMany(o => o.OffreCompetences)
                .HasForeignKey(oc => oc.IdOffreEmploi)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OffreCompetences>()
                .HasOne(oc => oc.Competence)
                .WithMany(c => c.OffreCompetences)
                .HasForeignKey(oc => oc.IdCompetence)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure StatutOffre enum to be stored as string
            modelBuilder.Entity<OffreEmploi>()
         .Property(r => r.TypeContrat)
         .HasColumnType("int");

            modelBuilder.Entity<OffreEmploi>()
                .Property(o => o.Statut)
                .HasConversion<int>();

            modelBuilder.Entity<OffreEmploi>()
                .Property(o => o.ModeTravail)
                .HasConversion<int>();

            modelBuilder.Entity<OffreCompetences>()
                    .Property(o => o.NiveauRequis)
                    .HasConversion<int>();


            modelBuilder.Entity<OffreEmploi>()
                .Property(o => o.NombrePostes)
                .IsRequired()
                .HasColumnType("int");



            modelBuilder.Entity<OffreEmploi>()
           .Property(o => o.Avantages)
           .HasMaxLength(500);


            modelBuilder.Entity<OffreEmploi>()
     .Property(o => o.SalaireMin)
     .HasPrecision(18, 2);

            modelBuilder.Entity<OffreEmploi>()
                .Property(o => o.SalaireMax)
                .HasPrecision(18, 2);
            modelBuilder.Entity<OffreEmploi>().ToTable("OffresEmploi");
            modelBuilder.Entity<Competence>().ToTable("Competences");
            modelBuilder.Entity<OffreCompetences>().ToTable("OffreCompetences");
            /************************postuler**********************************************/
            // Configuration des clés primaires et génération automatique des Guid
            modelBuilder.Entity<Experience>()
                .Property(e => e.IdExperience)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Certificat>()
                .Property(c => c.IdCertificat)
                .ValueGeneratedOnAdd();


            modelBuilder.Entity<Candidature>()
                .Property(c => c.IdCandidature)
                .ValueGeneratedOnAdd();



            modelBuilder.Entity<AppUserCompetence>() // Updated
                 .Property(uc => uc.Id)
                 .ValueGeneratedOnAdd();



            // Configuration des relations
            modelBuilder.Entity<Experience>()
                .HasOne(e => e.AppUser)
                .WithMany(u => u.Experiences)
                .HasForeignKey(e => e.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Certificat>()
                .HasOne(c => c.Experience)
                .WithMany(e => e.Certificats)
                .HasForeignKey(c => c.ExperienceId)
                .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<Candidature>()
                .HasOne(c => c.AppUser)
                .WithMany(u => u.Candidatures)
                .HasForeignKey(c => c.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Candidature>()
                .HasOne(c => c.Offre)
                .WithMany(o => o.Candidatures)
                .HasForeignKey(c => c.OffreId)
            .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<AppUserCompetence>()
           .HasOne(cc => cc.AppUser) // Updated to specify the navigation property
           .WithMany(u => u.AppUserCompetences)
           .HasForeignKey(cc => cc.AppUserId);


            modelBuilder.Entity<AppUserCompetence>()
       .HasOne(cc => cc.Competence) // Updated
       .WithMany(c => c.AppUserCompetences)
       .HasForeignKey(cc => cc.CompetenceId);

            // Explicitly set the table name to AppUserCompetences
            modelBuilder.Entity<AppUserCompetence>() // Updated
                .ToTable("AppUserCompetences");

            // Configure NiveauPossede with a custom value converter to handle accents
            // In OnModelCreating method
            modelBuilder.Entity<AppUserCompetence>()
                .Property(cc => cc.NiveauPossede)
                .HasConversion<int>();


            modelBuilder.Entity<AppUserCompetence>()
                .Property(cc => cc.NiveauPossede)
                .HasConversion<int>();
            modelBuilder.Entity<AppUserCompetence>()
                 .HasIndex(cc => new { cc.AppUserId, cc.CompetenceId })
                 .IsUnique();

            // Add index for Candidature
            modelBuilder.Entity<Candidature>()
                .HasIndex(c => new { c.AppUserId, c.OffreId })
                   .IsUnique();


            // Exemple : Configuration de la relation entre Filiale et AppUser (optionnel, car déjà géré par les annotations)
            modelBuilder.Entity<AppUser>()
                .HasOne(u => u.Filiale)
                .WithMany(f => f.Users)
                .HasForeignKey(u => u.IdFiliale)
                .OnDelete(DeleteBehavior.SetNull); // Si la Filiale est supprimée, IdFiliale sera défini à null
        }



        /*****relation user ---departement ***************************/

        








        // Add the following helper method to handle string normalization:
        private static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new System.Text.StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }



        public DbSet<Backend_poulina_future_jobs.Models.AppUser> AppUser { get; set; } = default!;
    }


}
