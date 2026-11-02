using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHomeRamen.Persistance.Menu.Migrations
{
    /// <inheritdoc />
    public partial class _20261031_CompanyCatalogAndRestaurantOffers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                schema: "menu",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Price",
                schema: "menu",
                table: "Ingredients");

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                schema: "menu",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "menu",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                schema: "menu",
                table: "Ingredients",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "CategoryType",
                schema: "menu",
                table: "Categories",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                schema: "menu",
                table: "Categories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "RestaurantIngredientPrices",
                schema: "menu",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantIngredientPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestaurantIngredientPrices_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalSchema: "menu",
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RestaurantProductOffers",
                schema: "menu",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantProductOffers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestaurantProductOffers_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "menu",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_CompanyId_Name",
                schema: "menu",
                table: "Products",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_CompanyId_Name",
                schema: "menu",
                table: "Ingredients",
                columns: new[] { "CompanyId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CompanyId_Name_CategoryType",
                schema: "menu",
                table: "Categories",
                columns: new[] { "CompanyId", "Name", "CategoryType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantIngredientPrices_IngredientId",
                schema: "menu",
                table: "RestaurantIngredientPrices",
                column: "IngredientId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantIngredientPrices_RestaurantId_IngredientId",
                schema: "menu",
                table: "RestaurantIngredientPrices",
                columns: new[] { "RestaurantId", "IngredientId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantProductOffers_CompanyId_Status",
                schema: "menu",
                table: "RestaurantProductOffers",
                columns: new[] { "CompanyId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantProductOffers_ProductId",
                schema: "menu",
                table: "RestaurantProductOffers",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantProductOffers_RestaurantId_ProductId",
                schema: "menu",
                table: "RestaurantProductOffers",
                columns: new[] { "RestaurantId", "ProductId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RestaurantIngredientPrices",
                schema: "menu");

            migrationBuilder.DropTable(
                name: "RestaurantProductOffers",
                schema: "menu");

            migrationBuilder.DropIndex(
                name: "IX_Products_CompanyId_Name",
                schema: "menu",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Ingredients_CompanyId_Name",
                schema: "menu",
                table: "Ingredients");

            migrationBuilder.DropIndex(
                name: "IX_Categories_CompanyId_Name_CategoryType",
                schema: "menu",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "menu",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "menu",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "menu",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "menu",
                table: "Categories");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                schema: "menu",
                table: "Products",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                schema: "menu",
                table: "Ingredients",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "CategoryType",
                schema: "menu",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
