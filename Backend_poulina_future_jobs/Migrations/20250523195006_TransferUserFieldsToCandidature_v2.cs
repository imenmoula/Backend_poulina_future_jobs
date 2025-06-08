using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_poulina_future_jobs.Migrations
{
    /// <inheritdoc />
    public partial class TransferUserFieldsToCandidature_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LettreMotivation",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Statut",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "cv",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "github",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "linkedIn",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "portfolio",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "CvFilePath",
                table: "Candidatures",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Github",
                table: "Candidatures",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LettreMotivation",
                table: "Candidatures",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LinkedIn",
                table: "Candidatures",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Portfolio",
                table: "Candidatures",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StatutCandidate",
                table: "Candidatures",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CvFilePath",
                table: "Candidatures");

            migrationBuilder.DropColumn(
                name: "Github",
                table: "Candidatures");

            migrationBuilder.DropColumn(
                name: "LettreMotivation",
                table: "Candidatures");

            migrationBuilder.DropColumn(
                name: "LinkedIn",
                table: "Candidatures");

            migrationBuilder.DropColumn(
                name: "Portfolio",
                table: "Candidatures");

            migrationBuilder.DropColumn(
                name: "StatutCandidate",
                table: "Candidatures");

            migrationBuilder.AddColumn<string>(
                name: "LettreMotivation",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Statut",
                table: "AspNetUsers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "cv",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "github",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "linkedIn",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "portfolio",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
