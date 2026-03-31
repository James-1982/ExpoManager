using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Expo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStandCategoriesRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StandCategories",
                columns: table => new
                {
                    StandId = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StandCategories", x => new { x.StandId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_StandCategories_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StandCategories_Stands_StandId",
                        column: x => x.StandId,
                        principalTable: "Stands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stands_Name",
                table: "Stands",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StandCategories_CategoryId",
                table: "StandCategories",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StandCategories");

            migrationBuilder.DropIndex(
                name: "IX_Stands_Name",
                table: "Stands");
        }
    }
}
