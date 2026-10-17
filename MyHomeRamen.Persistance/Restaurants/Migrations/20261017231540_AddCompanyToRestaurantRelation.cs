using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHomeRamen.Persistance.Restaurants.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyToRestaurantRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                schema: "restaurants",
                table: "Restaurants",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Restaurants_CompanyId",
                schema: "restaurants",
                table: "Restaurants",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurants_Companies_CompanyId",
                schema: "restaurants",
                table: "Restaurants",
                column: "CompanyId",
                principalSchema: "restaurants",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Restaurants_Companies_CompanyId",
                schema: "restaurants",
                table: "Restaurants");

            migrationBuilder.DropIndex(
                name: "IX_Restaurants_CompanyId",
                schema: "restaurants",
                table: "Restaurants");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "restaurants",
                table: "Restaurants");
        }
    }
}
