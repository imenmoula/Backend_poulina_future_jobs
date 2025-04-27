using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_poulina_future_jobs.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDepartmentAddFilialeRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Departements_IdDepartement",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "IdDepartement",
                table: "AspNetUsers",
                newName: "IdFiliale");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_IdDepartement",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_IdFiliale");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Filiales_IdFiliale",
                table: "AspNetUsers",
                column: "IdFiliale",
                principalTable: "Filiales",
                principalColumn: "IdFiliale",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Filiales_IdFiliale",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "IdFiliale",
                table: "AspNetUsers",
                newName: "IdDepartement");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_IdFiliale",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_IdDepartement");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Departements_IdDepartement",
                table: "AspNetUsers",
                column: "IdDepartement",
                principalTable: "Departements",
                principalColumn: "IdDepartement");
        }
    }
}
