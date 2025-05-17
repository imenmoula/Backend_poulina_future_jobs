using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_poulina_future_jobs.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOffreEmploiAndCompetence : Migration
    {
        /// <inheritdoc />



    
        
            protected override void Up(MigrationBuilder migrationBuilder)
            {
                // Step 1: Convert Statut from string to int
                migrationBuilder.Sql(@"
                UPDATE OffresEmploi
                SET Statut = CASE Statut
                    WHEN 'Ouvert' THEN 0
                    WHEN 'Ferme' THEN 1
                    ELSE 0 -- Default to Ouvert
                END
                WHERE Statut IS NOT NULL;
            ");

                migrationBuilder.AlterColumn<int>(
                    name: "Statut",
                    table: "OffresEmploi",
                    type: "int",
                    nullable: false,
                    oldClrType: typeof(string),
                    oldType: "nvarchar(max)");

                // Step 2: Convert ModeTravail from string to int
                migrationBuilder.Sql(@"
                UPDATE OffresEmploi
                SET ModeTravail = CASE ModeTravail
                    WHEN 'Presentiel' THEN 0
                    WHEN 'Hybride' THEN 1
                    WHEN 'Teletravail' THEN 2
                    ELSE 0 -- Default to Presentiel
                END
                WHERE ModeTravail IS NOT NULL;
            ");

                migrationBuilder.AlterColumn<int>(
                    name: "ModeTravail",
                    table: "OffresEmploi",
                    type: "int",
                    nullable: false,
                    oldClrType: typeof(string),
                    oldType: "nvarchar(max)");

                // Step 3: Add SalaireMin and SalaireMax
                migrationBuilder.AddColumn<decimal>(
                    name: "SalaireMin",
                    table: "OffresEmploi",
                    type: "decimal(18,2)",
                    nullable: false,
                    defaultValue: 0m);

                migrationBuilder.AddColumn<decimal>(
                    name: "SalaireMax",
                    table: "OffresEmploi",
                    type: "decimal(18,2)",
                    nullable: false,
                    defaultValue: 0m);

                // Step 4: Transfer data from Salaire to SalaireMin and SalaireMax
                migrationBuilder.Sql("UPDATE OffresEmploi SET SalaireMin = Salaire, SalaireMax = Salaire WHERE Salaire IS NOT NULL;");

                // Step 5: Drop Salaire column
                migrationBuilder.DropColumn(
                    name: "Salaire",
                    table: "OffresEmploi");

                // Step 6: Remove string length constraints
                migrationBuilder.AlterColumn<string>(
                    name: "Titre",
                    table: "OffresEmploi",
                    type: "nvarchar(max)",
                    nullable: false,
                    oldClrType: typeof(string),
                    oldType: "nvarchar(200)",
                    oldMaxLength: 200);

                migrationBuilder.AlterColumn<string>(
                    name: "Description",
                    table: "OffresEmploi",
                    type: "nvarchar(max)",
                    nullable: false,
                    oldClrType: typeof(string),
                    oldType: "nvarchar(2000)",
                    oldMaxLength: 2000);

                migrationBuilder.AlterColumn<string>(
                    name: "NiveauExperienceRequis",
                    table: "OffresEmploi",
                    type: "nvarchar(max)",
                    nullable: true,
                    oldClrType: typeof(string),
                    oldType: "nvarchar(50)",
                    oldMaxLength: 50,
                    oldNullable: true);

                migrationBuilder.AlterColumn<string>(
                    name: "DiplomeRequis",
                    table: "OffresEmploi",
                    type: "nvarchar(max)",
                    nullable: true,
                    oldClrType: typeof(string),
                    oldType: "nvarchar(100)",
                    oldMaxLength: 100,
                    oldNullable: true);

                migrationBuilder.AlterColumn<string>(
                    name: "Specialite",
                    table: "OffresEmploi",
                    type: "nvarchar(max)",
                    nullable: true,
                    oldClrType: typeof(string),
                    oldType: "nvarchar(max)",
                    oldNullable: true);

                migrationBuilder.AlterColumn<string>(
                    name: "Avantages",
                    table: "OffresEmploi",
                    type: "nvarchar(max)",
                    nullable: true,
                    oldClrType: typeof(string),
                    oldType: "nvarchar(max)",
                    oldNullable: true);

                migrationBuilder.AlterColumn<string>(
                    name: "Nom",
                    table: "Competences",
                    type: "nvarchar(max)",
                    nullable: false,
                    oldClrType: typeof(string),
                    oldType: "nvarchar(10000)",
                    oldMaxLength: 10000);

                migrationBuilder.AlterColumn<string>(
                    name: "Description",
                    table: "Competences",
                    type: "nvarchar(max)",
                    nullable: false,
                    oldClrType: typeof(string),
                    oldType: "nvarchar(1000)",
                    oldMaxLength: 1000);
            }

            protected override void Down(MigrationBuilder migrationBuilder)
            {
                // Restore string lengths
                migrationBuilder.AlterColumn<string>(
                    name: "Nom",
                    table: "Competences",
                    type: "nvarchar(10000)",
                    maxLength: 10000,
                    nullable: false,
                    oldClrType: typeof(string),
                    oldType: "nvarchar(max)");

                migrationBuilder.AlterColumn<string>(
                    name: "Description",
                    table: "Competences",
                    type: "nvarchar(1000)",
                    maxLength: 1000,
                    nullable: false,
                    oldClrType: typeof(string),
                    oldType: "nvarchar(max)");

                migrationBuilder.AlterColumn<string>(
                    name: "Titre",
                    table: "OffresEmploi",
                    type: "nvarchar(200)",
                    maxLength: 200,
                    nullable: false,
                    oldClrType: typeof(string),
                    oldType: "nvarchar(max)");

                migrationBuilder.AlterColumn<string>(
                    name: "Description",
                    table: "OffresEmploi",
                    type: "nvarchar(2000)",
                    maxLength: 2000,
                    nullable: false,
                    oldClrType: typeof(string),
                    oldType: "nvarchar(max)");

                migrationBuilder.AlterColumn<string>(
                    name: "NiveauExperienceRequis",
                    table: "OffresEmploi",
                    type: "nvarchar(50)",
                    maxLength: 50,
                    nullable: true,
                    oldClrType: typeof(string),
                    oldType: "nvarchar(max)",
                    oldNullable: true);

                migrationBuilder.AlterColumn<string>(
                    name: "DiplomeRequis",
                    table: "OffresEmploi",
                    type: "nvarchar(100)",
                    maxLength: 100,
                    nullable: true,
                    oldClrType: typeof(string),
                    oldType: "nvarchar(max)",
                    oldNullable: true);

                // Restore Salaire
                migrationBuilder.AddColumn<decimal>(
                    name: "Salaire",
                    table: "OffresEmploi",
                    type: "decimal(18,2)",
                    nullable: false,
                    defaultValue: 0m);

                migrationBuilder.Sql("UPDATE OffresEmploi SET Salaire = SalaireMin WHERE SalaireMin IS NOT NULL;");

                migrationBuilder.DropColumn(name: "SalaireMin", table: "OffresEmploi");
                migrationBuilder.DropColumn(name: "SalaireMax", table: "OffresEmploi");

                // Revert ModeTravail to string
                migrationBuilder.AlterColumn<string>(
                    name: "ModeTravail",
                    table: "OffresEmploi",
                    type: "nvarchar(max)",
                    nullable: false,
                    oldClrType: typeof(int),
                    oldType: "int");

                migrationBuilder.Sql(@"
                UPDATE OffresEmploi
                SET ModeTravail = CASE ModeTravail
                    WHEN 0 THEN 'Presentiel'
                    WHEN 1 THEN 'Hybride'
                    WHEN 2 THEN 'Teletravail'
                    ELSE 'Presentiel'
                END;
            ");

                // Revert Statut to string
                migrationBuilder.AlterColumn<string>(
                    name: "Statut",
                    table: "OffresEmploi",
                    type: "nvarchar(max)",
                    nullable: false,
                    oldClrType: typeof(int),
                    oldType: "int");

                migrationBuilder.Sql(@"
                UPDATE OffresEmploi
                SET Statut = CASE Statut
                    WHEN 0 THEN 'Ouvert'
                    WHEN 1 THEN 'Ferme'
                    ELSE 'Ouvert'
                END;
            ");
            }
        }
    }

