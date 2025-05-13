using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend_poulina_future_jobs.Migrations
{
    public partial class ValidateAppUserCertificatDiplome : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the old foreign key for Certificats (if it exists)
            migrationBuilder.Sql("IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Certificats_Experiences_ExperienceId') " +
                                "ALTER TABLE [Certificats] DROP CONSTRAINT [FK_Certificats_Experiences_ExperienceId]");

            // Rename ExperienceId to AppUserId in Certificats
            migrationBuilder.RenameColumn(
                name: "ExperienceId",
                table: "Certificats",
                newName: "AppUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Certificats_ExperienceId",
                table: "Certificats",
                newName: "IX_Certificats_AppUserId");

            // Update AppUserId values in Certificats by joining with Experiences
            migrationBuilder.Sql(@"
                UPDATE c
                SET c.AppUserId = e.AppUserId
                FROM Certificats c
                INNER JOIN Experiences e ON c.AppUserId = e.IdExperience
            ");

            // Delete orphaned Certificats (optional, if some cannot be mapped)
            migrationBuilder.Sql(@"
                DELETE FROM Certificats
                WHERE AppUserId NOT IN (SELECT Id FROM AspNetUsers)
            ");

            // Add the new foreign key for Certificats
            migrationBuilder.AddForeignKey(
                name: "FK_Certificats_AspNetUsers_AppUserId",
                table: "Certificats",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // Add LettreMotivation column to AspNetUsers
            migrationBuilder.AddColumn<string>(
                name: "LettreMotivation",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            // Create DiplomesCandidate table
            migrationBuilder.CreateTable(
                name: "DiplomesCandidate",
                columns: table => new
                {
                    IdDiplome = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AppUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NomDiplome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Institution = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DateObtention = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Specialite = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UrlDocument = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiplomesCandidate", x => x.IdDiplome);
                    table.ForeignKey(
                        name: "FK_DiplomesCandidate_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiplomesCandidate_AppUserId",
                table: "DiplomesCandidate",
                column: "AppUserId");

            // Check if IdDepartement column exists in OffresEmploi before adding (though it likely exists)
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE name = 'IdDepartement' AND object_id = OBJECT_ID('OffresEmploi')) " +
                                "ALTER TABLE [OffresEmploi] ADD [IdDepartement] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000'");

            // Check if the index exists before creating it for OffresEmploi
            migrationBuilder.Sql("IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_OffresEmploi_IdDepartement' AND object_id = OBJECT_ID('OffresEmploi')) " +
                                "CREATE INDEX [IX_OffresEmploi_IdDepartement] ON [OffresEmploi] ([IdDepartement])");

            // Add foreign key for OffresEmploi
            migrationBuilder.AddForeignKey(
                name: "FK_OffresEmploi_Departements_IdDepartement",
                table: "OffresEmploi",
                column: "IdDepartement",
                principalTable: "Departements",
                principalColumn: "IdDepartement",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OffresEmploi_Departements_IdDepartement",
                table: "OffresEmploi");

            migrationBuilder.DropForeignKey(
                name: "FK_Certificats_AspNetUsers_AppUserId",
                table: "Certificats");

            migrationBuilder.DropTable(
                name: "DiplomesCandidate");

            migrationBuilder.DropIndex(
                name: "IX_OffresEmploi_IdDepartement",
                table: "OffresEmploi");

            // Remove IdDepartement only if it was added by this migration (optional, depending on your schema)
            migrationBuilder.Sql("IF EXISTS (SELECT 1 FROM sys.columns WHERE name = 'IdDepartement' AND object_id = OBJECT_ID('OffresEmploi')) " +
                                "AND NOT EXISTS (SELECT 1 FROM sys.columns WHERE name = 'IdDepartement' AND object_id = OBJECT_ID('OffresEmploi') AND is_nullable = 1) " +
                                "ALTER TABLE [OffresEmploi] DROP COLUMN [IdDepartement]");

            migrationBuilder.DropColumn(
                name: "LettreMotivation",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "AppUserId",
                table: "Certificats",
                newName: "ExperienceId");

            migrationBuilder.RenameIndex(
                name: "IX_Certificats_AppUserId",
                table: "Certificats",
                newName: "IX_Certificats_ExperienceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificats_Experiences_ExperienceId",
                table: "Certificats",
                column: "ExperienceId",
                principalTable: "Experiences",
                principalColumn: "IdExperience",
                onDelete: ReferentialAction.Cascade);
        }
    }
}