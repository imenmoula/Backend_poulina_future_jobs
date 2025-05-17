using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_poulina_future_jobs.Migrations
{
    /// <inheritdoc />
    public partial class AddOffreEmploiOffreCompetenceRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "IdRecruteur",
                table: "Filiales",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Competences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nom = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TypeCompetence = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NiveauRequis = table.Column<int>(type: "int", nullable: false),
                    DateModification = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Competences", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OffresEmploi",
                columns: table => new
                {
                    IdOffreEmploi = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    specialite = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Titre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    DatePublication = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateExpiration = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Salaire = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TypeContrat = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Statut = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NiveauExperienceRequis = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DiplomeRequis = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdRecruteur = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdFiliale = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
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
                        name: "FK_OffresEmploi_Filiales_IdFiliale",
                        column: x => x.IdFiliale,
                        principalTable: "Filiales",
                        principalColumn: "IdFiliale",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OffreCompetences",
                columns: table => new
                {
                    IdOffreEmploi = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdCompetence = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OffreCompetences", x => new { x.IdOffreEmploi, x.IdCompetence });
                    table.ForeignKey(
                        name: "FK_OffreCompetences_Competences_IdCompetence",
                        column: x => x.IdCompetence,
                        principalTable: "Competences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OffreCompetences_OffresEmploi_IdOffreEmploi",
                        column: x => x.IdOffreEmploi,
                        principalTable: "OffresEmploi",
                        principalColumn: "IdOffreEmploi",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Filiales_IdRecruteur",
                table: "Filiales",
                column: "IdRecruteur");

            migrationBuilder.CreateIndex(
                name: "IX_OffreCompetences_IdCompetence",
                table: "OffreCompetences",
                column: "IdCompetence");

            migrationBuilder.CreateIndex(
                name: "IX_OffresEmploi_IdFiliale",
                table: "OffresEmploi",
                column: "IdFiliale");

            migrationBuilder.CreateIndex(
                name: "IX_OffresEmploi_IdRecruteur",
                table: "OffresEmploi",
                column: "IdRecruteur");

            migrationBuilder.AddForeignKey(
                name: "FK_Filiales_AspNetUsers_IdRecruteur",
                table: "Filiales",
                column: "IdRecruteur",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Filiales_AspNetUsers_IdRecruteur",
                table: "Filiales");

            migrationBuilder.DropTable(
                name: "OffreCompetences");

            migrationBuilder.DropTable(
                name: "Competences");

            migrationBuilder.DropTable(
                name: "OffresEmploi");

            migrationBuilder.DropIndex(
                name: "IX_Filiales_IdRecruteur",
                table: "Filiales");

            migrationBuilder.DropColumn(
                name: "IdRecruteur",
                table: "Filiales");
        }
    }
}
