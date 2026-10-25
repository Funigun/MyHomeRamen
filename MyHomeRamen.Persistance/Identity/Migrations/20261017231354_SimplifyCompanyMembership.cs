using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHomeRamen.Persistance.Identity.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyCompanyMembership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CompanyMemberships_IdempotencyKey",
                schema: "identity",
                table: "CompanyMemberships");

            migrationBuilder.DropIndex(
                name: "IX_CompanyMemberships_UserId_CompanyId",
                schema: "identity",
                table: "CompanyMemberships");

            migrationBuilder.DropColumn(
                name: "IdempotencyKey",
                schema: "identity",
                table: "CompanyMemberships");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "identity",
                table: "CompanyMemberships");

            migrationBuilder.DropColumn(
                name: "RoleId",
                schema: "identity",
                table: "CompanyMemberships");

            migrationBuilder.AddColumn<Guid>(
                name: "RestaurantId",
                schema: "identity",
                table: "CompanyMemberships",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyMemberships_UserId_CompanyId_RestaurantId",
                schema: "identity",
                table: "CompanyMemberships",
                columns: new[] { "UserId", "CompanyId", "RestaurantId" },
                unique: true,
                filter: "[RestaurantId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CompanyMemberships_UserId_CompanyId_RestaurantId",
                schema: "identity",
                table: "CompanyMemberships");

            migrationBuilder.DropColumn(
                name: "RestaurantId",
                schema: "identity",
                table: "CompanyMemberships");

            migrationBuilder.AddColumn<string>(
                name: "IdempotencyKey",
                schema: "identity",
                table: "CompanyMemberships",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "identity",
                table: "CompanyMemberships",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                schema: "identity",
                table: "CompanyMemberships",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_CompanyMemberships_IdempotencyKey",
                schema: "identity",
                table: "CompanyMemberships",
                column: "IdempotencyKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyMemberships_UserId_CompanyId",
                schema: "identity",
                table: "CompanyMemberships",
                columns: new[] { "UserId", "CompanyId" },
                unique: true);
        }
    }
}
