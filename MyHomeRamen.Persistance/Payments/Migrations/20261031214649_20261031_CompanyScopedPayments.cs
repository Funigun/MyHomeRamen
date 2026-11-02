using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHomeRamen.Persistance.Payments.Migrations
{
    /// <inheritdoc />
    public partial class _20261031_CompanyScopedPayments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                schema: "payments",
                table: "PaymentMethods",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                schema: "payments",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_CompanyId_Name",
                schema: "payments",
                table: "PaymentMethods",
                columns: new[] { "CompanyId", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PaymentMethods_CompanyId_Name",
                schema: "payments",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "payments",
                table: "PaymentMethods");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                schema: "payments",
                table: "Orders");
        }
    }
}
