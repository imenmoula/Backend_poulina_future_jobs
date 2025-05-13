

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Globalization;

namespace Backend_poulina_future_jobs.Models
{
    public class AppDbContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // DbSets pour les tables de la base de données
        public DbSet<IdentityRole<Guid>> AppRole { get; set; }
        public DbSet<Filiale> Filiales { get; set; }
        public DbSet<Departement> Departements { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<OffreEmploi> OffresEmploi { get; set; }
        public DbSet<Poste> Postes { get; set; }
        public DbSet<OffreMission> OffreMissions { get; set; }
        public DbSet<OffreLangue> OffreLangues { get; set; }
        public DbSet<Diplome> Diplomes { get; set; }
        public DbSet<Competence> Competences { get; set; }
        public DbSet<OffreCompetences> OffreCompetences { get; set; }
        public DbSet<Experience> Experiences { get; set; }
        public DbSet<Certificat> Certificats { get; set; }
        public DbSet<Candidature> Candidatures { get; set; }
        public DbSet<AppUserCompetence> AppUserCompetences { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Reponse> Reponses { get; set; }
        public DbSet<TentativeQuiz> TentativesQuiz { get; set; }
        public DbSet<ReponseUtilisateur> ReponsesUtilisateur { get; set; }
        public DbSet<ResultatQuiz> ResultatsQuiz { get; set; }
        public DbSet<AppUser> AppUser { get; set; } = default!;
        // New DbSet for Diplome
        public DbSet<DiplomeCandidate> DiplomesCandidate { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuration pour Filiale et Département
            modelBuilder.Entity<Filiale>()
                .Property(f => f.IdFiliale)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<Departement>()
                .Property(d => d.IdDepartement)
                .HasDefaultValueSql("NEWID()");

            modelBuilder.Entity<Departement>()
                .HasOne(d => d.Filiale)
                .WithMany(f => f.Departements)
                .HasForeignKey(d => d.IdFiliale)
                .OnDelete(DeleteBehavior.Cascade);

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

            // Configuration pour OffreEmploi et compétences
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

            // Configuration des propriétés d'OffreEmploi
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
                .Property(o => o.Avantages)
                .HasMaxLength(500);

            modelBuilder.Entity<OffreEmploi>()
                .Property(o => o.SalaireMin)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OffreEmploi>()
                .Property(o => o.SalaireMax)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OffreEmploi>()
                .HasMany(o => o.DiplomesRequis)
                .WithMany(d => d.OffresEmploi)
                .UsingEntity(j => j.ToTable("OffreEmploiDiplomes"));

            modelBuilder.Entity<OffreEmploi>()
                .HasMany(o => o.Postes)
                .WithOne(p => p.OffreEmploi)
                .HasForeignKey(p => p.IdOffreEmploi)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OffreEmploi>()
                .HasMany(o => o.OffreMissions)
                .WithOne(om => om.OffreEmploi)
                .HasForeignKey(om => om.IdOffreEmploi)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OffreEmploi>()
                .HasMany(o => o.OffreLangues)
                .WithOne(ol => ol.OffreEmploi)
                .HasForeignKey(ol => ol.IdOffreEmploi)
                .OnDelete(DeleteBehavior.Cascade);
            // Configuration de la relation OffreEmploi -> Filiale

            modelBuilder.Entity<OffreEmploi>()
               .HasOne(o => o.Filiale)
               .WithMany(f => f.OffresEmploi)
               .HasForeignKey(o => o.IdFiliale)
               .OnDelete(DeleteBehavior.Restrict);

            // Configuration de la relation entre OffreEmploi et Departement
            // Configuration de la relation OffreEmploi -> Departement
            modelBuilder.Entity<OffreEmploi>()
            .HasOne(o => o.Departement)
            .WithMany(d => d.OffresEmploi)
            .HasForeignKey(o => o.IdDepartement);
            // Assurez-vous que Departement est mappé à la table correcte
            modelBuilder.Entity<Departement>()
                .ToTable("Departements"); // Utilisez le nom exact de la table dans la base de données



            // Définition des noms de tables
            modelBuilder.Entity<OffreEmploi>().ToTable("OffresEmploi");
            modelBuilder.Entity<Competence>().ToTable("Competences");
            modelBuilder.Entity<OffreCompetences>().ToTable("OffreCompetences");

            // Configuration pour les expériences et certificats
            modelBuilder.Entity<Experience>()
                .Property(e => e.IdExperience)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Certificat>()
                .Property(c => c.IdCertificat)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Candidature>()
                .Property(c => c.IdCandidature)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<AppUserCompetence>()
                .Property(uc => uc.Id)
                .ValueGeneratedOnAdd();

            // Relations pour les expériences et certificats
            modelBuilder.Entity<Experience>()
                .HasOne(e => e.AppUser)
                .WithMany(u => u.Experiences)
                .HasForeignKey(e => e.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DiplomeCandidate>()
                 .HasOne(d => d.AppUser)
                 .WithMany(u => u.DiplomesCandidate)
                 .HasForeignKey(d => d.AppUserId)
                 .OnDelete(DeleteBehavior.Cascade);

            // Configure AppUser-Certificat relationship
            // In AppDbContext.cs, update the TentativeQuiz configuration
            modelBuilder.Entity<TentativeQuiz>()
                .HasOne(t => t.AppUser)
                .WithMany(u => u.Tentatives)
                .HasForeignKey(t => t.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ensure Certificat configuration is correct
            modelBuilder.Entity<Certificat>()
                .HasOne(c => c.AppUser)
                .WithMany(u => u.Certificats)
                .HasForeignKey(c => c.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relations pour les candidatures
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

            // Relations pour les compétences des utilisateurs
            modelBuilder.Entity<AppUserCompetence>()
                .HasOne(cc => cc.AppUser)
                .WithMany(u => u.AppUserCompetences)
                .HasForeignKey(cc => cc.AppUserId);

            modelBuilder.Entity<AppUserCompetence>()
                .HasOne(cc => cc.Competence)
                .WithMany(c => c.AppUserCompetences)
                .HasForeignKey(cc => cc.CompetenceId);

            modelBuilder.Entity<AppUserCompetence>()
                .ToTable("AppUserCompetences");

            modelBuilder.Entity<AppUserCompetence>()
                .Property(cc => cc.NiveauPossede)
                .HasConversion<int>();

            modelBuilder.Entity<AppUserCompetence>()
                .HasIndex(cc => new { cc.AppUserId, cc.CompetenceId })
                .IsUnique();

            // Index pour Candidature
            modelBuilder.Entity<Candidature>()
                .HasIndex(c => new { c.AppUserId, c.OffreId })
                .IsUnique();

            // Relation Filiale-AppUser
            modelBuilder.Entity<AppUser>()
                .HasOne(u => u.Filiale)
                .WithMany(f => f.Users)
                .HasForeignKey(u => u.IdFiliale)
                .OnDelete(DeleteBehavior.SetNull);

            // === RELATIONS POUR LES QUIZ ===

            // Relations OffreEmploi-Quiz
            modelBuilder.Entity<OffreEmploi>()
                .HasMany(o => o.Quizzes)
                .WithOne(q => q.OffreEmploi)
                .HasForeignKey(q => q.OffreEmploiId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relations Quiz
            modelBuilder.Entity<Quiz>()
                .HasOne(q => q.OffreEmploi)
                .WithMany(o => o.Quizzes)
                .HasForeignKey(q => q.OffreEmploiId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Quiz>()
                .HasMany(q => q.Questions)
                .WithOne(qu => qu.Quiz)
                .HasForeignKey(qu => qu.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Quiz>()
                .HasMany(q => q.Tentatives)
                .WithOne(t => t.Quiz)
                .HasForeignKey(t => t.QuizId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relations Question
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Quiz)
                .WithMany(qz => qz.Questions)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Question>()
                .HasMany(q => q.Reponses)
                .WithOne(r => r.Question)
                .HasForeignKey(r => r.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Question>()
                .HasMany(q => q.ReponsesUtilisateur)
                .WithOne(ru => ru.Question)
                .HasForeignKey(ru => ru.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relations Reponse
            modelBuilder.Entity<Reponse>()
                .HasOne(r => r.Question)
                .WithMany(q => q.Reponses)
                .HasForeignKey(r => r.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reponse>()
                .HasMany(r => r.ReponsesUtilisateur)
                .WithOne(ru => ru.Reponse)
                .HasForeignKey(ru => ru.ReponseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relations TentativeQuiz
            modelBuilder.Entity<TentativeQuiz>()
                .HasOne(t => t.Quiz)
                .WithMany(q => q.Tentatives)
                .HasForeignKey(t => t.QuizId)
                .OnDelete(DeleteBehavior.Restrict);
            /***********************************************/
    //        modelBuilder.Entity<TentativeQuiz>()
    //.HasOne(t => t.AppUser)
    //.WithMany(u => u.Tentatives) // Specify the navigation property
    //.HasForeignKey(t => t.AppUserId)
    //.OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TentativeQuiz>()
                .HasOne(t => t.Resultat)
                .WithOne(r => r.Tentative)
                .HasForeignKey<ResultatQuiz>(r => r.TentativeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TentativeQuiz>()
                .HasMany(t => t.ReponsesUtilisateur)
                .WithOne(ru => ru.Tentative)
                .HasForeignKey(ru => ru.TentativeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relations ReponseUtilisateur
            modelBuilder.Entity<ReponseUtilisateur>()
                .HasOne(ru => ru.Tentative)
                .WithMany(t => t.ReponsesUtilisateur)
                .HasForeignKey(ru => ru.TentativeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReponseUtilisateur>()
                .HasOne(ru => ru.Question)
                .WithMany(q => q.ReponsesUtilisateur)
                .HasForeignKey(ru => ru.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReponseUtilisateur>()
                .HasOne(ru => ru.Reponse)
                .WithMany(r => r.ReponsesUtilisateur)
                .HasForeignKey(ru => ru.ReponseId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // Relations ResultatQuiz
            modelBuilder.Entity<ResultatQuiz>()
                .HasOne(r => r.Tentative)
                .WithOne(t => t.Resultat)
                .HasForeignKey<ResultatQuiz>(r => r.TentativeId)
                .OnDelete(DeleteBehavior.Cascade);

           
        }

        // Méthode auxiliaire pour gérer la normalisation des chaînes
        private static string RemoveDiacritics(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

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
    }
}