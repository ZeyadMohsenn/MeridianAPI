using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class alterOrderTablePriceBeforeTax : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PriceBeforeTax",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceBeforeTax",
                table: "Orders");
        }
    }
}
