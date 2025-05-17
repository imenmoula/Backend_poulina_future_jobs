using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_poulina_future_jobs.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCompetencesRequisesFromOffreMission : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Ajout de la colonne IdFiliale à la table OffresEmploi
            migrationBuilder.AddColumn<Guid>(
                name: "IdFiliale",
                table: "OffresEmploi",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000")); // Valeur temporaire
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Suppression de la colonne IdFiliale (pour revenir en arrière)
            migrationBuilder.DropColumn(
                name: "IdFiliale",
                table: "OffresEmploi");
        }

    }
}
