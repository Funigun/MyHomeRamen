using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHomeRamen.Persistance.Menu.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuCardsAndSections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MenuCards",
                schema: "menu",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuCards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MenuSections",
                schema: "menu",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MenuCardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuSections_MenuCards_MenuCardId",
                        column: x => x.MenuCardId,
                        principalSchema: "menu",
                        principalTable: "MenuCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuSectionItems",
                schema: "menu",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MenuSectionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestaurantProductOfferId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuSectionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MenuSectionItems_MenuSections_MenuSectionId",
                        column: x => x.MenuSectionId,
                        principalSchema: "menu",
                        principalTable: "MenuSections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MenuSectionItems_RestaurantProductOffers_RestaurantProductOfferId",
                        column: x => x.RestaurantProductOfferId,
                        principalSchema: "menu",
                        principalTable: "RestaurantProductOffers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MenuSectionItems_MenuSectionId_RestaurantProductOfferId",
                schema: "menu",
                table: "MenuSectionItems",
                columns: new[] { "MenuSectionId", "RestaurantProductOfferId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuSectionItems_RestaurantProductOfferId",
                schema: "menu",
                table: "MenuSectionItems",
                column: "RestaurantProductOfferId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuSections_MenuCardId",
                schema: "menu",
                table: "MenuSections",
                column: "MenuCardId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuSectionItems",
                schema: "menu");

            migrationBuilder.DropTable(
                name: "MenuSections",
                schema: "menu");

            migrationBuilder.DropTable(
                name: "MenuCards",
                schema: "menu");
        }
    }
}
