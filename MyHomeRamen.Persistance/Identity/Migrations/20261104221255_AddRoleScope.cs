using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHomeRamen.Persistance.Identity.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleScope : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Scope",
                schema: "identity",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "Company");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Scope",
                schema: "identity",
                table: "Roles");
        }
    }
}
