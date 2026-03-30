using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Expo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SchemaAndIndexOptimization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stand_Length",
                table: "Stands");

            migrationBuilder.DropColumn(
                name: "Stand_Width",
                table: "Stands");

            migrationBuilder.CreateIndex(
                name: "IX_Pavilions_Name",
                table: "Pavilions",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExhibitionAreas_Name",
                table: "ExhibitionAreas",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Pavilions_Name",
                table: "Pavilions");

            migrationBuilder.DropIndex(
                name: "IX_ExhibitionAreas_Name",
                table: "ExhibitionAreas");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Name",
                table: "Categories");

            migrationBuilder.AddColumn<int>(
                name: "Stand_Length",
                table: "Stands",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Stand_Width",
                table: "Stands",
                type: "integer",
                nullable: true);
        }
    }
}
