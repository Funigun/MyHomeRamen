using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHomeRamen.Persistance.Identity.Migrations;

public partial class AddRestaurantAdminCompanyMembership : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<Guid>("RestaurantId", "CompanyMemberships", "identity", nullable: false, defaultValue: Guid.Empty);
        migrationBuilder.DropIndex("IX_CompanyMemberships_UserId_CompanyId", "identity", "CompanyMemberships");
        migrationBuilder.CreateIndex("IX_CompanyMemberships_UserId_CompanyId_RestaurantId", "CompanyMemberships", new[] { "UserId", "CompanyId", "RestaurantId" }, "identity", unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex("IX_CompanyMemberships_UserId_CompanyId_RestaurantId", "identity", "CompanyMemberships");
        migrationBuilder.CreateIndex("IX_CompanyMemberships_UserId_CompanyId", "CompanyMemberships", new[] { "UserId", "CompanyId" }, "identity", unique: true);
        migrationBuilder.DropColumn("RestaurantId", "CompanyMemberships", "identity");
    }
}
