using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_poulina_future_jobs.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCompetenceAndOffreCompetences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Competences");

            migrationBuilder.AddColumn<string>(
                name: "HardSkills",
                table: "Competences",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "SoftSkills",
                table: "Competences",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HardSkills",
                table: "Competences");

            migrationBuilder.DropColumn(
                name: "SoftSkills",
                table: "Competences");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Competences",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
