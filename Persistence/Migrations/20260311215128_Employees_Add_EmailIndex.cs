using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Employees_Add_EmailIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EMPLOYEE_FAMILY",
                table: "EMPLOYEES",
                newName: "EMPLOYEE_LAST_NAME");

            migrationBuilder.CreateIndex(
                name: "UX_EMPLOYEES_EMPLOYEE_EMAIL",
                table: "EMPLOYEES",
                column: "EMPLOYEE_EMAIL",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_EMPLOYEES_EMPLOYEE_EMAIL",
                table: "EMPLOYEES");

            migrationBuilder.RenameColumn(
                name: "EMPLOYEE_LAST_NAME",
                table: "EMPLOYEES",
                newName: "EMPLOYEE_FAMILY");
        }
    }
}
