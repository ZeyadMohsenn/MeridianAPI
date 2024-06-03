using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class imgforCategoryandSub : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Photo",
                table: "SubCategorys");

            migrationBuilder.RenameColumn(
                name: "Photo",
                table: "Category",
                newName: "StoredFileName");

            migrationBuilder.AddColumn<string>(
                name: "StoredFileName",
                table: "SubCategorys",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StoredFileName",
                table: "SubCategorys");

            migrationBuilder.RenameColumn(
                name: "StoredFileName",
                table: "Category",
                newName: "Photo");

            migrationBuilder.AddColumn<string>(
                name: "Photo",
                table: "SubCategorys",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
