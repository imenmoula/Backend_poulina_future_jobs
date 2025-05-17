using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend_poulina_future_jobs.Migrations
{
    /// <inheritdoc />
    public partial class RenameDepartmentIdToIdDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Departements_DepartmentId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "AspNetUsers",
                newName: "IdDepartement");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_DepartmentId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_IdDepartement");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Departements_IdDepartement",
                table: "AspNetUsers",
                column: "IdDepartement",
                principalTable: "Departements",
                principalColumn: "IdDepartement");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Departements_IdDepartement",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "IdDepartement",
                table: "AspNetUsers",
                newName: "DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_IdDepartement",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Departements_DepartmentId",
                table: "AspNetUsers",
                column: "DepartmentId",
                principalTable: "Departements",
                principalColumn: "IdDepartement",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
