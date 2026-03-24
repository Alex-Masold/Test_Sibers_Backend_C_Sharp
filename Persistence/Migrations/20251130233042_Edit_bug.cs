using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Edit_bug : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "TASKS_PROJECTS",
                table: "TASKS");

            migrationBuilder.RenameColumn(
                name: "TASK_CRATE_AT",
                table: "TASKS",
                newName: "TASK_CRAETE_AT");

            migrationBuilder.AlterColumn<string>(
                name: "TASK_COMMENT",
                table: "TASKS",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_TASKS_PROJECTS",
                table: "TASKS",
                column: "TASK_PROJECT_ID",
                principalTable: "PROJECTS",
                principalColumn: "PROJECT_ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TASKS_PROJECTS",
                table: "TASKS");

            migrationBuilder.RenameColumn(
                name: "TASK_CRAETE_AT",
                table: "TASKS",
                newName: "TASK_CRATE_AT");

            migrationBuilder.AlterColumn<string>(
                name: "TASK_COMMENT",
                table: "TASKS",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "TASKS_PROJECTS",
                table: "TASKS",
                column: "TASK_PROJECT_ID",
                principalTable: "PROJECTS",
                principalColumn: "PROJECT_ID",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
