using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_Documents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PROJECT_DOCUMENTS",
                columns: table => new
                {
                    PROJECT_DOCUMENT_ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PROJECT_DOCUMENT_ORIGINAL_FILE_NAME = table.Column<string>(type: "TEXT", nullable: false),
                    PROJECT_DOCUMENT_STORED_FILE_NAME = table.Column<string>(type: "TEXT", nullable: false),
                    PROJECT_DOCUMENT_CONTENT_TYPE = table.Column<string>(type: "TEXT", nullable: false),
                    PROJECT_ID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PROJECT_DOCUMENTS", x => x.PROJECT_DOCUMENT_ID);
                    table.ForeignKey(
                        name: "FK_PROJECTS_DOCUMENTS",
                        column: x => x.PROJECT_ID,
                        principalTable: "PROJECTS",
                        principalColumn: "PROJECT_ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PROJECT_DOCUMENTS_PROJECT_ID",
                table: "PROJECT_DOCUMENTS",
                column: "PROJECT_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PROJECT_DOCUMENTS");
        }
    }
}
