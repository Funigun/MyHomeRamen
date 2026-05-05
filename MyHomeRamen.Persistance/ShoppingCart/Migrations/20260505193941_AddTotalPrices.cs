using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHomeRamen.Persistance.ShoppingCart.Migrations
{
    /// <inheritdoc />
    public partial class AddTotalPrices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                schema: "basket",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                schema: "basket",
                table: "Ingredients",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPrice",
                schema: "basket",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Quantity",
                schema: "basket",
                table: "Ingredients");
        }
    }
}
