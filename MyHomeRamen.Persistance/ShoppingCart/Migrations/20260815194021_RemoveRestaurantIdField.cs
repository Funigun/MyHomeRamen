using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHomeRamen.Persistance.ShoppingCart.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRestaurantIdField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RestaurantId",
                schema: "basket",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RestaurantId",
                schema: "basket",
                table: "ShoppingCarts");

            migrationBuilder.DropColumn(
                name: "RestaurantId",
                schema: "basket",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "RestaurantId",
                schema: "basket",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "RestaurantId",
                schema: "basket",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "RestaurantId",
                schema: "basket",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "RestaurantId",
                schema: "basket",
                table: "BasketItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RestaurantId",
                schema: "basket",
                table: "Users",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RestaurantId",
                schema: "basket",
                table: "ShoppingCarts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RestaurantId",
                schema: "basket",
                table: "Roles",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RestaurantId",
                schema: "basket",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RestaurantId",
                schema: "basket",
                table: "Permissions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RestaurantId",
                schema: "basket",
                table: "Ingredients",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RestaurantId",
                schema: "basket",
                table: "BasketItems",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
