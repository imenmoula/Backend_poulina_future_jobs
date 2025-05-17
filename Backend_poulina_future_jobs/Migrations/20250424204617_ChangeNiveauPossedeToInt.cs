using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_poulina_future_jobs.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNiveauPossedeToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ajouter une colonne temporaire pour stocker les nouvelles valeurs int
            migrationBuilder.AddColumn<int>(
                name: "NiveauPossede_Int",
                table: "AppUserCompetences",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Convertir les valeurs existantes de string à int
            migrationBuilder.Sql(@"
                UPDATE AppUserCompetences SET NiveauPossede_Int = 0 WHERE NiveauPossede = 'Debutant';
                UPDATE AppUserCompetences SET NiveauPossede_Int = 1 WHERE NiveauPossede = 'Intermediaire';
                UPDATE AppUserCompetences SET NiveauPossede_Int = 2 WHERE NiveauPossede = 'Avance';
                UPDATE AppUserCompetences SET NiveauPossede_Int = 3 WHERE NiveauPossede = 'Expert';
            ");

            // Supprimer l'ancienne colonne string
            migrationBuilder.DropColumn(
                name: "NiveauPossede",
                table: "AppUserCompetences");

            // Renommer la nouvelle colonne int
            migrationBuilder.RenameColumn(
                name: "NiveauPossede_Int",
                table: "AppUserCompetences",
                newName: "NiveauPossede");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Ajouter une colonne temporaire pour stocker les valeurs string
            migrationBuilder.AddColumn<string>(
                name: "NiveauPossede_String",
                table: "AppUserCompetences",
                type: "nvarchar(50)",
                nullable: false,
                defaultValue: "");

            // Convertir les valeurs existantes de int à string
            migrationBuilder.Sql(@"
                UPDATE AppUserCompetences SET NiveauPossede_String = 'Debutant' WHERE NiveauPossede = 0;
                UPDATE AppUserCompetences SET NiveauPossede_String = 'Intermediaire' WHERE NiveauPossede = 1;
                UPDATE AppUserCompetences SET NiveauPossede_String = 'Avance' WHERE NiveauPossede = 2;
                UPDATE AppUserCompetences SET NiveauPossede_String = 'Expert' WHERE NiveauPossede = 3;
            ");

            // Supprimer l'ancienne colonne int
            migrationBuilder.DropColumn(
                name: "NiveauPossede",
                table: "AppUserCompetences");

            // Renommer la nouvelle colonne string
            migrationBuilder.RenameColumn(
                name: "NiveauPossede_String",
                table: "AppUserCompetences",
                newName: "NiveauPossede");

            // Définir le type de la colonne
            migrationBuilder.AlterColumn<string>(
                name: "NiveauPossede",
                table: "AppUserCompetences",
                type: "nvarchar(50)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}