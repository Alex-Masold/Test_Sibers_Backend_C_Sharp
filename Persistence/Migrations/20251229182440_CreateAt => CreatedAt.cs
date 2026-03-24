using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CreateAtCreatedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TASK_UPDATE_AT",
                table: "TASKS",
                newName: "TASK_UPDATED_AT");

            migrationBuilder.RenameColumn(
                name: "TASK_CRAETE_AT",
                table: "TASKS",
                newName: "TASK_CRAETED_AT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TASK_UPDATED_AT",
                table: "TASKS",
                newName: "TASK_UPDATE_AT");

            migrationBuilder.RenameColumn(
                name: "TASK_CRAETED_AT",
                table: "TASKS",
                newName: "TASK_CRAETE_AT");
        }
    }
}
