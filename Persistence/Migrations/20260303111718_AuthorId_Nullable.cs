using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AuthorId_Nullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TASKS_AUTHOR",
                table: "TASKS");

            migrationBuilder.AddForeignKey(
                name: "FK_TASKS_AUTHOR",
                table: "TASKS",
                column: "TASK_AUTHOR_ID",
                principalTable: "EMPLOYEES",
                principalColumn: "EMPLOYEE_ID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TASKS_AUTHOR",
                table: "TASKS");

            migrationBuilder.AddForeignKey(
                name: "FK_TASKS_AUTHOR",
                table: "TASKS",
                column: "TASK_AUTHOR_ID",
                principalTable: "EMPLOYEES",
                principalColumn: "EMPLOYEE_ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
