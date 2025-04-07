using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_poulina_future_jobs.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCompetenceAndOffreRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OffreCompetences_Competences_IdCompetence",
                table: "OffreCompetences");

            migrationBuilder.DropColumn(
                name: "TypeCompetence",
                table: "Competences");

            migrationBuilder.RenameColumn(
                name: "NiveauRequis",
                table: "Competences",
                newName: "Type");

            migrationBuilder.AddColumn<int>(
                name: "NiveauRequis",
                table: "OffreCompetences",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_OffreCompetences_Competences_IdCompetence",
                table: "OffreCompetences",
                column: "IdCompetence",
                principalTable: "Competences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OffreCompetences_Competences_IdCompetence",
                table: "OffreCompetences");

            migrationBuilder.DropColumn(
                name: "NiveauRequis",
                table: "OffreCompetences");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Competences",
                newName: "NiveauRequis");

            migrationBuilder.AddColumn<string>(
                name: "TypeCompetence",
                table: "Competences",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_OffreCompetences_Competences_IdCompetence",
                table: "OffreCompetences",
                column: "IdCompetence",
                principalTable: "Competences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
