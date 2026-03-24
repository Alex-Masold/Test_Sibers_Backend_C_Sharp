using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Task_ColumnName_CRAETE_AT_to_CREATE_AT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TASK_CRAETED_AT",
                table: "TASKS",
                newName: "TASK_CREATED_AT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TASK_CREATED_AT",
                table: "TASKS",
                newName: "TASK_CRAETED_AT");
        }
    }
}
