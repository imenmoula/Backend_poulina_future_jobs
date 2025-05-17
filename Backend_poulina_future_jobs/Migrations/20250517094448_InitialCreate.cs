using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_poulina_future_jobs.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Competences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    dateAjout = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModification = table.Column<DateTime>(type: "datetime2", nullable: false),
                    estTechnique = table.Column<bool>(type: "bit", nullable: false),
                    estSoftSkill = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Competences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Diplomes",
                columns: table => new
                {
                    IdDiplome = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NomDiplome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Niveau = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Domaine = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Institution = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diplomes", x => x.IdDiplome);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AppUserCompetences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompetenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NiveauPossede = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserCompetences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppUserCompetences_Competences_CompetenceId",
                        column: x => x.CompetenceId,
                        principalTable: "Competences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    Nom = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    Prenom = table.Column<string>(type: "nvarchar(150)", nullable: true),
                    Photo = table.Column<string>(type: "nvarchar(255)", nullable: true),
                    DateNaissance = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Adresse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ville = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pays = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NiveauEtude = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Diplome = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Universite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    specialite = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cv = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    linkedIn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    github = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    portfolio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Entreprise = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Poste = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Statut = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiryTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LettreMotivation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdFiliale = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Certificats",
                columns: table => new
                {
                    IdCertificat = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateObtention = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Organisme = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    UrlDocument = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificats", x => x.IdCertificat);
                    table.ForeignKey(
                        name: "FK_Certificats_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiplomesCandidate",
                columns: table => new
                {
                    IdDiplome = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NomDiplome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Institution = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DateObtention = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Specialite = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UrlDocument = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiplomesCandidate", x => x.IdDiplome);
                    table.ForeignKey(
                        name: "FK_DiplomesCandidate_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Experiences",
                columns: table => new
                {
                    IdExperience = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Poste = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    NomEntreprise = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CompetenceAcquise = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DateDebut = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateFin = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Experiences", x => x.IdExperience);
                    table.ForeignKey(
                        name: "FK_Experiences_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Filiales",
                columns: table => new
                {
                    IdFiliale = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Nom = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Adresse = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Photo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Fax = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SiteWeb = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IdRecruteur = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Filiales", x => x.IdFiliale);
                    table.ForeignKey(
                        name: "FK_Filiales_AspNetUsers_IdRecruteur",
                        column: x => x.IdRecruteur,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Departements",
                columns: table => new
                {
                    IdDepartement = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "NEWID()"),
                    Nom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: 10000, nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdFiliale = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departements", x => x.IdDepartement);
                    table.ForeignKey(
                        name: "FK_Departements_Filiales_IdFiliale",
                        column: x => x.IdFiliale,
                        principalTable: "Filiales",
                        principalColumn: "IdFiliale",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OffresEmploi",
                columns: table => new
                {
                    IdOffreEmploi = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Specialite = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DatePublication = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateExpiration = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SalaireMin = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SalaireMax = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    NiveauExperienceRequis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeContrat = table.Column<int>(type: "int", nullable: false),
                    Statut = table.Column<int>(type: "int", nullable: false),
                    ModeTravail = table.Column<int>(type: "int", nullable: false),
                    estActif = table.Column<bool>(type: "bit", nullable: false),
                    Avantages = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IdRecruteur = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdFiliale = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdDepartement = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OffresEmploi", x => x.IdOffreEmploi);
                    table.ForeignKey(
                        name: "FK_OffresEmploi_AspNetUsers_IdRecruteur",
                        column: x => x.IdRecruteur,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OffresEmploi_Departements_IdDepartement",
                        column: x => x.IdDepartement,
                        principalTable: "Departements",
                        principalColumn: "IdDepartement",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OffresEmploi_Filiales_IdFiliale",
                        column: x => x.IdFiliale,
                        principalTable: "Filiales",
                        principalColumn: "IdFiliale",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Candidatures",
                columns: table => new
                {
                    IdCandidature = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OffreId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MessageMotivation = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DateSoumission = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidatures", x => x.IdCandidature);
                    table.ForeignKey(
                        name: "FK_Candidatures_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Candidatures_OffresEmploi_OffreId",
                        column: x => x.OffreId,
                        principalTable: "OffresEmploi",
                        principalColumn: "IdOffreEmploi",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OffreCompetences",
                columns: table => new
                {
                    IdOffreEmploi = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdCompetence = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NiveauRequis = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OffreCompetences", x => new { x.IdOffreEmploi, x.IdCompetence });
                    table.ForeignKey(
                        name: "FK_OffreCompetences_Competences_IdCompetence",
                        column: x => x.IdCompetence,
                        principalTable: "Competences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OffreCompetences_OffresEmploi_IdOffreEmploi",
                        column: x => x.IdOffreEmploi,
                        principalTable: "OffresEmploi",
                        principalColumn: "IdOffreEmploi",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OffreEmploiDiplomes",
                columns: table => new
                {
                    DiplomesRequisIdDiplome = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OffresEmploiIdOffreEmploi = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OffreEmploiDiplomes", x => new { x.DiplomesRequisIdDiplome, x.OffresEmploiIdOffreEmploi });
                    table.ForeignKey(
                        name: "FK_OffreEmploiDiplomes_Diplomes_DiplomesRequisIdDiplome",
                        column: x => x.DiplomesRequisIdDiplome,
                        principalTable: "Diplomes",
                        principalColumn: "IdDiplome",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OffreEmploiDiplomes_OffresEmploi_OffresEmploiIdOffreEmploi",
                        column: x => x.OffresEmploiIdOffreEmploi,
                        principalTable: "OffresEmploi",
                        principalColumn: "IdOffreEmploi",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OffreLangues",
                columns: table => new
                {
                    IdOffreLangue = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdOffreEmploi = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Langue = table.Column<int>(type: "int", nullable: false),
                    NiveauRequis = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OffreLangues", x => x.IdOffreLangue);
                    table.ForeignKey(
                        name: "FK_OffreLangues_OffresEmploi_IdOffreEmploi",
                        column: x => x.IdOffreEmploi,
                        principalTable: "OffresEmploi",
                        principalColumn: "IdOffreEmploi",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OffreMissions",
                columns: table => new
                {
                    IdOffreMission = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdOffreEmploi = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DescriptionMission = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Priorite = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OffreMissions", x => x.IdOffreMission);
                    table.ForeignKey(
                        name: "FK_OffreMissions_OffresEmploi_IdOffreEmploi",
                        column: x => x.IdOffreEmploi,
                        principalTable: "OffresEmploi",
                        principalColumn: "IdOffreEmploi",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Postes",
                columns: table => new
                {
                    IdPoste = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TitrePoste = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombrePostes = table.Column<int>(type: "int", nullable: false),
                    ExperienceSouhaitee = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NiveauHierarchique = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdOffreEmploi = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Postes", x => x.IdPoste);
                    table.ForeignKey(
                        name: "FK_Postes_OffresEmploi_IdOffreEmploi",
                        column: x => x.IdOffreEmploi,
                        principalTable: "OffresEmploi",
                        principalColumn: "IdOffreEmploi",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quizzes",
                columns: table => new
                {
                    QuizId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstActif = table.Column<bool>(type: "bit", nullable: false),
                    Duree = table.Column<int>(type: "int", nullable: false),
                    ScoreMinimum = table.Column<int>(type: "int", nullable: false),
                    OffreEmploiId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quizzes", x => x.QuizId);
                    table.ForeignKey(
                        name: "FK_Quizzes_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Quizzes_OffresEmploi_OffreEmploiId",
                        column: x => x.OffreEmploiId,
                        principalTable: "OffresEmploi",
                        principalColumn: "IdOffreEmploi",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Texte = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    Ordre = table.Column<int>(type: "int", nullable: false),
                    TempsRecommande = table.Column<int>(type: "int", nullable: true),
                    QuizId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.QuestionId);
                    table.ForeignKey(
                        name: "FK_Questions_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "QuizId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TentativesQuiz",
                columns: table => new
                {
                    TentativeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuizId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateDebut = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Statut = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TentativesQuiz", x => x.TentativeId);
                    table.ForeignKey(
                        name: "FK_TentativesQuiz_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TentativesQuiz_Quizzes_QuizId",
                        column: x => x.QuizId,
                        principalTable: "Quizzes",
                        principalColumn: "QuizId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reponses",
                columns: table => new
                {
                    ReponseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Texte = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EstCorrecte = table.Column<bool>(type: "bit", nullable: false),
                    Ordre = table.Column<int>(type: "int", nullable: false),
                    Explication = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reponses", x => x.ReponseId);
                    table.ForeignKey(
                        name: "FK_Reponses_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResultatsQuiz",
                columns: table => new
                {
                    ResultatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TentativeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Score = table.Column<double>(type: "float", nullable: false),
                    QuestionsCorrectes = table.Column<int>(type: "int", nullable: false),
                    NombreQuestions = table.Column<int>(type: "int", nullable: false),
                    TempsTotal = table.Column<int>(type: "int", nullable: false),
                    Reussi = table.Column<bool>(type: "bit", nullable: false),
                    Commentaire = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DateResultat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QuizId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResultatsQuiz", x => x.ResultatId);
                    table.ForeignKey(
                        name: "FK_ResultatsQuiz_TentativesQuiz_TentativeId",
                        column: x => x.TentativeId,
                        principalTable: "TentativesQuiz",
                        principalColumn: "TentativeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReponsesUtilisateur",
                columns: table => new
                {
                    ReponseUtilisateurId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TentativeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReponseId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TexteReponse = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    TempsReponse = table.Column<int>(type: "int", nullable: true),
                    EstCorrecte = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReponsesUtilisateur", x => x.ReponseUtilisateurId);
                    table.ForeignKey(
                        name: "FK_ReponsesUtilisateur_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "QuestionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReponsesUtilisateur_Reponses_ReponseId",
                        column: x => x.ReponseId,
                        principalTable: "Reponses",
                        principalColumn: "ReponseId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReponsesUtilisateur_TentativesQuiz_TentativeId",
                        column: x => x.TentativeId,
                        principalTable: "TentativesQuiz",
                        principalColumn: "TentativeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppUserCompetences_AppUserId_CompetenceId",
                table: "AppUserCompetences",
                columns: new[] { "AppUserId", "CompetenceId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUserCompetences_CompetenceId",
                table: "AppUserCompetences",
                column: "CompetenceId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IdFiliale",
                table: "AspNetUsers",
                column: "IdFiliale");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Candidatures_AppUserId_OffreId",
                table: "Candidatures",
                columns: new[] { "AppUserId", "OffreId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidatures_OffreId",
                table: "Candidatures",
                column: "OffreId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificats_AppUserId",
                table: "Certificats",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Departements_IdFiliale",
                table: "Departements",
                column: "IdFiliale");

            migrationBuilder.CreateIndex(
                name: "IX_DiplomesCandidate_AppUserId",
                table: "DiplomesCandidate",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Experiences_AppUserId",
                table: "Experiences",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Filiales_IdRecruteur",
                table: "Filiales",
                column: "IdRecruteur");

            migrationBuilder.CreateIndex(
                name: "IX_OffreCompetences_IdCompetence",
                table: "OffreCompetences",
                column: "IdCompetence");

            migrationBuilder.CreateIndex(
                name: "IX_OffreEmploiDiplomes_OffresEmploiIdOffreEmploi",
                table: "OffreEmploiDiplomes",
                column: "OffresEmploiIdOffreEmploi");

            migrationBuilder.CreateIndex(
                name: "IX_OffreLangues_IdOffreEmploi",
                table: "OffreLangues",
                column: "IdOffreEmploi");

            migrationBuilder.CreateIndex(
                name: "IX_OffreMissions_IdOffreEmploi",
                table: "OffreMissions",
                column: "IdOffreEmploi");

            migrationBuilder.CreateIndex(
                name: "IX_OffresEmploi_IdDepartement",
                table: "OffresEmploi",
                column: "IdDepartement");

            migrationBuilder.CreateIndex(
                name: "IX_OffresEmploi_IdFiliale",
                table: "OffresEmploi",
                column: "IdFiliale");

            migrationBuilder.CreateIndex(
                name: "IX_OffresEmploi_IdRecruteur",
                table: "OffresEmploi",
                column: "IdRecruteur");

            migrationBuilder.CreateIndex(
                name: "IX_Postes_IdOffreEmploi",
                table: "Postes",
                column: "IdOffreEmploi");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuizId",
                table: "Questions",
                column: "QuizId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_AppUserId",
                table: "Quizzes",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Quizzes_OffreEmploiId",
                table: "Quizzes",
                column: "OffreEmploiId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reponses_QuestionId",
                table: "Reponses",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReponsesUtilisateur_QuestionId",
                table: "ReponsesUtilisateur",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_ReponsesUtilisateur_ReponseId",
                table: "ReponsesUtilisateur",
                column: "ReponseId");

            migrationBuilder.CreateIndex(
                name: "IX_ReponsesUtilisateur_TentativeId",
                table: "ReponsesUtilisateur",
                column: "TentativeId");

            migrationBuilder.CreateIndex(
                name: "IX_ResultatsQuiz_TentativeId",
                table: "ResultatsQuiz",
                column: "TentativeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TentativesQuiz_AppUserId",
                table: "TentativesQuiz",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TentativesQuiz_QuizId",
                table: "TentativesQuiz",
                column: "QuizId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserCompetences_AspNetUsers_AppUserId",
                table: "AppUserCompetences",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Filiales_IdFiliale",
                table: "AspNetUsers",
                column: "IdFiliale",
                principalTable: "Filiales",
                principalColumn: "IdFiliale",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Filiales_AspNetUsers_IdRecruteur",
                table: "Filiales");

            migrationBuilder.DropTable(
                name: "AppUserCompetences");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Candidatures");

            migrationBuilder.DropTable(
                name: "Certificats");

            migrationBuilder.DropTable(
                name: "DiplomesCandidate");

            migrationBuilder.DropTable(
                name: "Experiences");

            migrationBuilder.DropTable(
                name: "OffreCompetences");

            migrationBuilder.DropTable(
                name: "OffreEmploiDiplomes");

            migrationBuilder.DropTable(
                name: "OffreLangues");

            migrationBuilder.DropTable(
                name: "OffreMissions");

            migrationBuilder.DropTable(
                name: "Postes");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "ReponsesUtilisateur");

            migrationBuilder.DropTable(
                name: "ResultatsQuiz");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Competences");

            migrationBuilder.DropTable(
                name: "Diplomes");

            migrationBuilder.DropTable(
                name: "Reponses");

            migrationBuilder.DropTable(
                name: "TentativesQuiz");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Quizzes");

            migrationBuilder.DropTable(
                name: "OffresEmploi");

            migrationBuilder.DropTable(
                name: "Departements");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Filiales");
        }
    }
}
