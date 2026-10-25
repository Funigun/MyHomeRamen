using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHomeRamen.Persistance.Restaurants.Migrations
{
    /// <inheritdoc />
    public partial class CompanyDetailsToCompanyModelRename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanySocialMedia_Companies_CompanyDetailsId",
                schema: "restaurants",
                table: "CompanySocialMedia");

            migrationBuilder.RenameColumn(
                name: "CompanyDetailsId",
                schema: "restaurants",
                table: "CompanySocialMedia",
                newName: "CompanyId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanySocialMedia_CompanyDetailsId",
                schema: "restaurants",
                table: "CompanySocialMedia",
                newName: "IX_CompanySocialMedia_CompanyId");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedName",
                schema: "restaurants",
                table: "Companies",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_NormalizedName",
                schema: "restaurants",
                table: "Companies",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanySocialMedia_Companies_CompanyId",
                schema: "restaurants",
                table: "CompanySocialMedia",
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
                name: "FK_CompanySocialMedia_Companies_CompanyId",
                schema: "restaurants",
                table: "CompanySocialMedia");

            migrationBuilder.DropIndex(
                name: "IX_Companies_NormalizedName",
                schema: "restaurants",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "NormalizedName",
                schema: "restaurants",
                table: "Companies");

            migrationBuilder.RenameColumn(
                name: "CompanyId",
                schema: "restaurants",
                table: "CompanySocialMedia",
                newName: "CompanyDetailsId");

            migrationBuilder.RenameIndex(
                name: "IX_CompanySocialMedia_CompanyId",
                schema: "restaurants",
                table: "CompanySocialMedia",
                newName: "IX_CompanySocialMedia_CompanyDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanySocialMedia_Companies_CompanyDetailsId",
                schema: "restaurants",
                table: "CompanySocialMedia",
                column: "CompanyDetailsId",
                principalSchema: "restaurants",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
