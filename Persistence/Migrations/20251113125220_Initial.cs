using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EMPLOYEES",
                columns: table => new
                {
                    EMPLOYEE_ID = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EMPLOYEE_FIRST_NAME = table.Column<string>(type: "TEXT", nullable: false),
                    EMPLOYEE_MIDDLE_NAME = table.Column<string>(type: "TEXT", nullable: true),
                    EMPLOYEE_FAMILY = table.Column<string>(type: "TEXT", nullable: false),
                    EMPLOYEE_EMAIL = table.Column<string>(type: "TEXT", nullable: false),
                    EMPLOYEE_ROLE = table.Column<int>(type: "INTEGER", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EMPLOYEES", x => x.EMPLOYEE_ID);
                }
            );

            migrationBuilder.CreateTable(
                name: "PROJECTS",
                columns: table => new
                {
                    PROJECT_ID = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PROJECT_NAME = table.Column<string>(type: "TEXT", nullable: false),
                    PROJECT_PRIORITY = table.Column<int>(type: "INTEGER", nullable: false),
                    PROJECT_COMPANY_ORDERING_NAME = table.Column<string>(
                        type: "TEXT",
                        nullable: false
                    ),
                    PROJECT_COMPANY_EXECUTING_NAME = table.Column<string>(
                        type: "TEXT",
                        nullable: false
                    ),
                    PROJECT_START_DATE = table.Column<DateOnly>(
                        type: "TEXT",
                        nullable: false,
                        defaultValueSql: "date('now')"
                    ),
                    PROJECT_END_DATE = table.Column<DateOnly>(type: "TEXT", nullable: true),
                    PROJECT_MANAGER_ID = table.Column<int>(type: "INTEGER", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PROJECTS", x => x.PROJECT_ID);
                    table.ForeignKey(
                        name: "FK_PROJECTS_EMPLOYEES",
                        column: x => x.PROJECT_MANAGER_ID,
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.SetNull
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "PROJECT_MEMBERS",
                columns: table => new
                {
                    PROJECT_ID = table.Column<int>(type: "INTEGER", nullable: false),
                    EMPLOYEE_ID = table.Column<int>(type: "INTEGER", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_PROJECT_MEMBERS",
                        x => new { x.PROJECT_ID, x.EMPLOYEE_ID }
                    );
                    table.ForeignKey(
                        name: "FK_EMPLOYEES_MEMBERS",
                        column: x => x.EMPLOYEE_ID,
                        principalTable: "EMPLOYEES",
                        principalColumn: "EMPLOYEE_ID",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_PROJECTS_MEMBERS",
                        column: x => x.PROJECT_ID,
                        principalTable: "PROJECTS",
                        principalColumn: "PROJECT_ID",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_PROJECT_MEMBERS_EMPLOYEE_ID",
                table: "PROJECT_MEMBERS",
                column: "EMPLOYEE_ID"
            );

            migrationBuilder.CreateIndex(
                name: "IX_PROJECTS_PROJECT_MANAGER_ID",
                table: "PROJECTS",
                column: "PROJECT_MANAGER_ID"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "PROJECT_MEMBERS");

            migrationBuilder.DropTable(name: "PROJECTS");

            migrationBuilder.DropTable(name: "EMPLOYEES");
        }
    }
}
