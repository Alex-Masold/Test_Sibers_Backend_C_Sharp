using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Surrogate_Key_and_UX : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PROJECT_MEMBERS",
                table: "PROJECT_MEMBERS");

            migrationBuilder.AddColumn<int>(
                name: "PROJECT_MEMBER_ID",
                table: "PROJECT_MEMBERS",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0)
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PROJECT_MEMBERS",
                table: "PROJECT_MEMBERS",
                column: "PROJECT_MEMBER_ID");

            migrationBuilder.CreateIndex(
                name: "UX_PROJECT_MEMBERS_PROJECT_ID_EMPLOYEE_ID",
                table: "PROJECT_MEMBERS",
                columns: new[] { "PROJECT_ID", "EMPLOYEE_ID" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PROJECT_MEMBERS",
                table: "PROJECT_MEMBERS");

            migrationBuilder.DropIndex(
                name: "UX_PROJECT_MEMBERS_PROJECT_ID_EMPLOYEE_ID",
                table: "PROJECT_MEMBERS");

            migrationBuilder.DropColumn(
                name: "PROJECT_MEMBER_ID",
                table: "PROJECT_MEMBERS");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PROJECT_MEMBERS",
                table: "PROJECT_MEMBERS",
                columns: new[] { "PROJECT_ID", "EMPLOYEE_ID" });
        }
    }
}
