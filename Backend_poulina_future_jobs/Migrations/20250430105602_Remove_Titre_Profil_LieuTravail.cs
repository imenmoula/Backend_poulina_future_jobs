using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_poulina_future_jobs.Migrations
{
    /// <inheritdoc />
    public partial class Remove_Titre_Profil_LieuTravail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LieuTravail",
                table: "OffresEmploi");

            migrationBuilder.DropColumn(
                name: "Titre",
                table: "OffresEmploi");

            migrationBuilder.DropColumn(
                name: "profilrecherche",
                table: "OffresEmploi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LieuTravail",
                table: "OffresEmploi",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Titre",
                table: "OffresEmploi",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "profilrecherche",
                table: "OffresEmploi",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
