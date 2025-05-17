using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_poulina_future_jobs.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCandidateCompetence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUserCompetences_AspNetUsers_AppUserId",
                table: "AppUserCompetences");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUserCompetences_Competences_CompetenceId",
                table: "AppUserCompetences");

            migrationBuilder.AlterColumn<string>(
                name: "NiveauPossede",
                table: "AppUserCompetences",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserCompetences_AspNetUsers_AppUserId",
                table: "AppUserCompetences",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserCompetences_Competences_CompetenceId",
                table: "AppUserCompetences",
                column: "CompetenceId",
                principalTable: "Competences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUserCompetences_AspNetUsers_AppUserId",
                table: "AppUserCompetences");

            migrationBuilder.DropForeignKey(
                name: "FK_AppUserCompetences_Competences_CompetenceId",
                table: "AppUserCompetences");

            migrationBuilder.AlterColumn<string>(
                name: "NiveauPossede",
                table: "AppUserCompetences",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserCompetences_AspNetUsers_AppUserId",
                table: "AppUserCompetences",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserCompetences_Competences_CompetenceId",
                table: "AppUserCompetences",
                column: "CompetenceId",
                principalTable: "Competences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
