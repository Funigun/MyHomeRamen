using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHomeRamen.Persistance.Identity.Migrations
{
    /// <inheritdoc />
    public partial class _20260714_FixUserToAddressRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Users_UserId",
                schema: "identity",
                table: "Addresses");

            migrationBuilder.DropTable(
                name: "UserAddresses",
                schema: "identity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Addresses",
                schema: "identity",
                table: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_UserId",
                schema: "identity",
                table: "Addresses");

            migrationBuilder.RenameTable(
                name: "Addresses",
                schema: "identity",
                newName: "Address",
                newSchema: "identity");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "identity",
                table: "Address",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Address",
                schema: "identity",
                table: "Address",
                columns: new[] { "UserId", "Id" });

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Users_UserId",
                schema: "identity",
                table: "Address",
                column: "UserId",
                principalSchema: "identity",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_Users_UserId",
                schema: "identity",
                table: "Address");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Address",
                schema: "identity",
                table: "Address");

            migrationBuilder.RenameTable(
                name: "Address",
                schema: "identity",
                newName: "Addresses",
                newSchema: "identity");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                schema: "identity",
                table: "Addresses",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Addresses",
                schema: "identity",
                table: "Addresses",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "UserAddresses",
                schema: "identity",
                columns: table => new
                {
                    AddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAddresses", x => new { x.AddressId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserAddresses_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalSchema: "identity",
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAddresses_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "identity",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UserId",
                schema: "identity",
                table: "Addresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_UserId",
                schema: "identity",
                table: "UserAddresses",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Users_UserId",
                schema: "identity",
                table: "Addresses",
                column: "UserId",
                principalSchema: "identity",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
