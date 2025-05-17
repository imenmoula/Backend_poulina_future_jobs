using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_poulina_future_jobs.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOffreEmploi : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "specialite",
                table: "OffresEmploi",
                newName: "Specialite");

            migrationBuilder.AlterColumn<string>(
                name: "Specialite",
                table: "OffresEmploi",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "TypeContrat",
                table: "OffresEmploi",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)");

            migrationBuilder.AlterColumn<string>(
                name: "Statut",
                table: "OffresEmploi",
                type: "nvarchar(10)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)");

            migrationBuilder.AddColumn<string>(
                name: "Avantages",
                table: "OffresEmploi",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ModeTravail",
                table: "OffresEmploi",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NombrePostes",
                table: "OffresEmploi",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avantages",
                table: "OffresEmploi");

            migrationBuilder.DropColumn(
                name: "ModeTravail",
                table: "OffresEmploi");

            migrationBuilder.DropColumn(
                name: "NombrePostes",
                table: "OffresEmploi");

            migrationBuilder.RenameColumn(
                name: "Specialite",
                table: "OffresEmploi",
                newName: "specialite");

            migrationBuilder.AlterColumn<string>(
                name: "TypeContrat",
                table: "OffresEmploi",
                type: "nvarchar(50)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Statut",
                table: "OffresEmploi",
                type: "nvarchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)");

            migrationBuilder.AlterColumn<string>(
                name: "specialite",
                table: "OffresEmploi",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
