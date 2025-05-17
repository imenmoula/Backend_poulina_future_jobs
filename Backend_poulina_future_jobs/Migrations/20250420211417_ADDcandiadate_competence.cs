using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_poulina_future_jobs.Migrations
{
    /// <inheritdoc />
    public partial class ADDcandiadate_competence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUserCompetences_AspNetUsers_AppUserId",
                table: "AppUserCompetences");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserCompetences_AspNetUsers_AppUserId",
                table: "AppUserCompetences",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppUserCompetences_AspNetUsers_AppUserId",
                table: "AppUserCompetences");

            migrationBuilder.AddForeignKey(
                name: "FK_AppUserCompetences_AspNetUsers_AppUserId",
                table: "AppUserCompetences",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
