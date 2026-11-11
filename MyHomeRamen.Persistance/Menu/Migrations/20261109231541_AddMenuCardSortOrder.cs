using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHomeRamen.Persistance.Menu.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuCardSortOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                schema: "menu",
                table: "MenuCards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql(
                """
                WITH OrderedMenuCards AS
                (
                    SELECT Id,
                           ROW_NUMBER() OVER (PARTITION BY RestaurantId ORDER BY CreatedOn, Id) - 1 AS SortOrder
                    FROM menu.MenuCards
                )
                UPDATE card
                SET SortOrder = ordered.SortOrder
                FROM menu.MenuCards AS card
                INNER JOIN OrderedMenuCards AS ordered ON card.Id = ordered.Id;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_MenuCards_RestaurantId_SortOrder",
                schema: "menu",
                table: "MenuCards",
                columns: new[] { "RestaurantId", "SortOrder" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_MenuCards_RestaurantId_SortOrder",
                schema: "menu",
                table: "MenuCards");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                schema: "menu",
                table: "MenuCards");
        }
    }
}
