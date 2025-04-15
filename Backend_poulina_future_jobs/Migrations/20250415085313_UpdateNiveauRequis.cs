using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend_poulina_future_jobs.Migrations
{
    public partial class UpdateNiveauRequis : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Convert NiveauRequis from string to int (if needed)
            migrationBuilder.Sql(@"
                UPDATE OffreCompetences
                SET NiveauRequis = CASE NiveauRequis
                    WHEN 'Debutant' THEN 0
                    WHEN 'Intermediaire' THEN 1
                    WHEN 'Avance' THEN 2
                    WHEN 'Expert' THEN 3
                    ELSE 0 -- Default to Debutant
                END
                WHERE NiveauRequis IS NOT NULL;
            ");

            // Step 2: Ensure NiveauRequis is int and required
            migrationBuilder.AlterColumn<int>(
                name: "NiveauRequis",
                table: "OffreCompetences",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            // Step 3: Set default for existing null values (if any)
            migrationBuilder.Sql("UPDATE OffreCompetences SET NiveauRequis = 0 WHERE NiveauRequis IS NULL;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert NiveauRequis to string
            migrationBuilder.AlterColumn<string>(
                name: "NiveauRequis",
                table: "OffreCompetences",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.Sql(@"
                UPDATE OffreCompetences
                SET NiveauRequis = CASE NiveauRequis
                    WHEN 0 THEN 'Debutant'
                    WHEN 1 THEN 'Intermediaire'
                    WHEN 2 THEN 'Avance'
                    WHEN 3 THEN 'Expert'
                    ELSE NULL
                END;
            ");
        }
    }
}