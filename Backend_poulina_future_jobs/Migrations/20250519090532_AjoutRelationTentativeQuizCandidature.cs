using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_poulina_future_jobs.Migrations
{
    /// <inheritdoc />
    public partial class AjoutRelationTentativeQuizCandidature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CandidatureId",
                table: "TentativesQuiz",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Institution",
                table: "Diplomes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_TentativesQuiz_CandidatureId",
                table: "TentativesQuiz",
                column: "CandidatureId");

            migrationBuilder.AddForeignKey(
                name: "FK_TentativesQuiz_Candidatures_CandidatureId",
                table: "TentativesQuiz",
                column: "CandidatureId",
                principalTable: "Candidatures",
                principalColumn: "IdCandidature",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TentativesQuiz_Candidatures_CandidatureId",
                table: "TentativesQuiz");

            migrationBuilder.DropIndex(
                name: "IX_TentativesQuiz_CandidatureId",
                table: "TentativesQuiz");

            migrationBuilder.DropColumn(
                name: "CandidatureId",
                table: "TentativesQuiz");

            migrationBuilder.AlterColumn<string>(
                name: "Institution",
                table: "Diplomes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
