using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_poulina_future_jobs.Migrations
{
    /// <inheritdoc />
    public partial class AjoutProprietesCompetence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HardSkills",
                table: "Competences");

            migrationBuilder.DropColumn(
                name: "SoftSkills",
                table: "Competences");

            migrationBuilder.RenameColumn(
                name: "specialite",
                table: "OffresEmploi",
                newName: "Specialite");

            migrationBuilder.AddColumn<DateTime>(
                name: "dateAjout",
                table: "Competences",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "estSoftSkill",
                table: "Competences",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "estTechnique",
                table: "Competences",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "dateAjout",
                table: "Competences");

            migrationBuilder.DropColumn(
                name: "estSoftSkill",
                table: "Competences");

            migrationBuilder.DropColumn(
                name: "estTechnique",
                table: "Competences");

            migrationBuilder.RenameColumn(
                name: "Specialite",
                table: "OffresEmploi",
                newName: "specialite");

            migrationBuilder.AddColumn<string>(
                name: "HardSkills",
                table: "Competences",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SoftSkills",
                table: "Competences",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
