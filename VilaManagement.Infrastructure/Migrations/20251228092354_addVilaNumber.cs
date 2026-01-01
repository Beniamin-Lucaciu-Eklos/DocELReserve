using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VilaManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addVilaNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VilaNumbers",
                columns: table => new
                {
                    Vila_Number = table.Column<int>(type: "int", nullable: false),
                    VilaId = table.Column<int>(type: "int", nullable: false),
                    SpecialDetails = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VilaNumbers", x => x.Vila_Number);
                    table.ForeignKey(
                        name: "FK_VilaNumbers_Vilas_VilaId",
                        column: x => x.VilaId,
                        principalTable: "Vilas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "VilaNumbers",
                columns: new[] { "Vila_Number", "SpecialDetails", "VilaId" },
                values: new object[,]
                {
                    { 101, null, 2 },
                    { 102, null, 2 },
                    { 103, null, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_VilaNumbers_VilaId",
                table: "VilaNumbers",
                column: "VilaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VilaNumbers");
        }
    }
}
