using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AotMemoryServer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "mem");

            migrationBuilder.CreateTable(
                name: "MemoryFacts",
                schema: "mem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    Key = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    Scope = table.Column<string>(type: "TEXT", nullable: false),
                    Confidence = table.Column<double>(type: "REAL", nullable: false),
                    Source = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<string>(type: "TEXT", nullable: false),
                    IsDeprecated = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemoryFacts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MemoryFacts_Category_Key_Scope",
                schema: "mem",
                table: "MemoryFacts",
                columns: new[] { "Category", "Key", "Scope" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MemoryFacts_Scope",
                schema: "mem",
                table: "MemoryFacts",
                column: "Scope");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MemoryFacts",
                schema: "mem");
        }
    }
}
