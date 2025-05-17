using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_poulina_future_jobs.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelWithNewRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NombrePostes",
                table: "OffresEmploi");

            migrationBuilder.RenameColumn(
                name: "DiplomeRequis",
                table: "OffresEmploi",
                newName: "profilrecherche");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "OffresEmploi",
                newName: "LieuTravail");

            migrationBuilder.AddColumn<Guid>(
                name: "AppUserId",
                table: "ResultatsQuiz",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "QuizId",
                table: "ResultatsQuiz",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "estActif",
                table: "OffresEmploi",
                type: "bit",
                nullable: false,
                defaultValue: false);

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
                    Priorite = table.Column<int>(type: "int", nullable: false),
                    CompetencesRequises = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                    IdDepartement = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdOffreEmploi = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Postes", x => x.IdPoste);
                    table.ForeignKey(
                        name: "FK_Postes_Departements_IdDepartement",
                        column: x => x.IdDepartement,
                        principalTable: "Departements",
                        principalColumn: "IdDepartement",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Postes_OffresEmploi_IdOffreEmploi",
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
                name: "IX_Postes_IdDepartement",
                table: "Postes",
                column: "IdDepartement");

            migrationBuilder.CreateIndex(
                name: "IX_Postes_IdOffreEmploi",
                table: "Postes",
                column: "IdOffreEmploi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OffreEmploiDiplomes");

            migrationBuilder.DropTable(
                name: "OffreLangues");

            migrationBuilder.DropTable(
                name: "OffreMissions");

            migrationBuilder.DropTable(
                name: "Postes");

            migrationBuilder.DropTable(
                name: "Diplomes");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "ResultatsQuiz");

            migrationBuilder.DropColumn(
                name: "QuizId",
                table: "ResultatsQuiz");

            migrationBuilder.DropColumn(
                name: "estActif",
                table: "OffresEmploi");

            migrationBuilder.RenameColumn(
                name: "profilrecherche",
                table: "OffresEmploi",
                newName: "DiplomeRequis");

            migrationBuilder.RenameColumn(
                name: "LieuTravail",
                table: "OffresEmploi",
                newName: "Description");

            migrationBuilder.AddColumn<int>(
                name: "NombrePostes",
                table: "OffresEmploi",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
