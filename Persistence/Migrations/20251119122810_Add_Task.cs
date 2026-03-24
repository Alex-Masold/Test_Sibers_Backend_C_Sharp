using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_Task : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "PROJECT_START_DATE",
                table: "PROJECTS",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "TEXT",
                oldDefaultValueSql: "date('now')"
            );

            migrationBuilder.CreateTable(
                name: "TASKS",
                columns: table => new
                {
                    TASK_ID = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TASK_TITLE = table.Column<string>(type: "TEXT", nullable: false),
                    TASK_PRIORITY = table.Column<int>(type: "INTEGER", nullable: false),
                    TASK_STATUS = table.Column<int>(type: "INTEGER", nullable: false),
                    TASK_COMMENT = table.Column<string>(type: "TEXT", nullable: false),
                    TASK_CRATE_AT = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TASK_UPDATE_AT = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TASK_AUTHOR_ID = table.Column<int>(type: "INTEGER", nullable: false),
                    TASK_EXECUTOR_ID = table.Column<int>(type: "INTEGER", nullable: true),
                    TASK_PROJECT_ID = table.Column<int>(type: "INTEGER", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TASKS", x => x.TASK_ID);
                    table.ForeignKey(
                        name: "FK_TASKS_AUTHOR",
                        column: x => x.TASK_AUTHOR_ID,
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_TASKS_EXECUTOR",
                        column: x => x.TASK_EXECUTOR_ID,
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.SetNull
                    );
                    table.ForeignKey(
                        name: "TASKS_PROJECTS",
                        column: x => x.TASK_PROJECT_ID,
                        principalTable: "PROJECTS",
                        principalColumn: "PROJECT_ID",
                        onDelete: ReferentialAction.SetNull
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_TASKS_TASK_AUTHOR_ID",
                table: "TASKS",
                column: "TASK_AUTHOR_ID"
            );

            migrationBuilder.CreateIndex(
                name: "IX_TASKS_TASK_EXECUTOR_ID",
                table: "TASKS",
                column: "TASK_EXECUTOR_ID"
            );

            migrationBuilder.CreateIndex(
                name: "IX_TASKS_TASK_PRIORITY",
                table: "TASKS",
                column: "TASK_PRIORITY"
            );

            migrationBuilder.CreateIndex(
                name: "IX_TASKS_TASK_PROJECT_ID",
                table: "TASKS",
                column: "TASK_PROJECT_ID"
            );

            migrationBuilder.CreateIndex(
                name: "IX_TASKS_TASK_STATUS",
                table: "TASKS",
                column: "TASK_STATUS"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "TASKS");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "PROJECT_START_DATE",
                table: "PROJECTS",
                type: "TEXT",
                nullable: false,
                defaultValueSql: "date('now')",
                oldClrType: typeof(DateOnly),
                oldType: "TEXT"
            );
        }
    }
}
