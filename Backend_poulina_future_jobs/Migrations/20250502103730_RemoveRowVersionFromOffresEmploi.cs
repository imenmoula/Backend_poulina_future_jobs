using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_poulina_future_jobs.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRowVersionFromOffresEmploi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "OffresEmploi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "OffresEmploi",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
