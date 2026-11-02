using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHomeRamen.Persistance.Orders.Migrations
{
    /// <inheritdoc />
    public partial class _20261102_OrderPurchaseFacts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "FinalChargedLinePrice",
                schema: "orders",
                table: "Products",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                schema: "orders",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "RestaurantProductOfferId",
                schema: "orders",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                schema: "orders",
                table: "Orders",
                type: "nchar(3)",
                fixedLength: true,
                maxLength: 3,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "RestaurantId",
                schema: "orders",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinalChargedLinePrice",
                schema: "orders",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Quantity",
                schema: "orders",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "RestaurantProductOfferId",
                schema: "orders",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Currency",
                schema: "orders",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "RestaurantId",
                schema: "orders",
                table: "Orders");
        }
    }
}
