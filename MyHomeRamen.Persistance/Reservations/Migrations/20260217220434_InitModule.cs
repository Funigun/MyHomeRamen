using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHomeRamen.Persistance.Reservations.Migrations
{
    /// <inheritdoc />
    public partial class InitModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "reservations");

            migrationBuilder.CreateTable(
                name: "Permissions",
                schema: "reservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "reservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tables",
                schema: "reservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TableNumber = table.Column<int>(type: "int", nullable: false),
                    MinNumberOfSeats = table.Column<int>(type: "int", nullable: false),
                    MaxNumberOfSeats = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "reservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermissionRole",
                schema: "reservations",
                columns: table => new
                {
                    PermissionsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionRole", x => new { x.PermissionsId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_PermissionRole_Permissions_PermissionsId",
                        column: x => x.PermissionsId,
                        principalSchema: "reservations",
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PermissionRole_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "reservations",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                schema: "reservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "reservations",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PermissionUser",
                schema: "reservations",
                columns: table => new
                {
                    PermissionsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionUser", x => new { x.PermissionsId, x.UserId });
                    table.ForeignKey(
                        name: "FK_PermissionUser_Permissions_PermissionsId",
                        column: x => x.PermissionsId,
                        principalSchema: "reservations",
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PermissionUser_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "reservations",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleUser",
                schema: "reservations",
                columns: table => new
                {
                    RolesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleUser", x => new { x.RolesId, x.UserId });
                    table.ForeignKey(
                        name: "FK_RoleUser_Roles_RolesId",
                        column: x => x.RolesId,
                        principalSchema: "reservations",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleUser_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "reservations",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingTable",
                schema: "reservations",
                columns: table => new
                {
                    BookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TablesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingTable", x => new { x.BookingId, x.TablesId });
                    table.ForeignKey(
                        name: "FK_BookingTable_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalSchema: "reservations",
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingTable_Tables_TablesId",
                        column: x => x.TablesId,
                        principalSchema: "reservations",
                        principalTable: "Tables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                schema: "reservations",
                table: "Bookings",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingTable_TablesId",
                schema: "reservations",
                table: "BookingTable",
                column: "TablesId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionRole_RoleId",
                schema: "reservations",
                table: "PermissionRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_PermissionUser_UserId",
                schema: "reservations",
                table: "PermissionUser",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleUser_UserId",
                schema: "reservations",
                table: "RoleUser",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingTable",
                schema: "reservations");

            migrationBuilder.DropTable(
                name: "PermissionRole",
                schema: "reservations");

            migrationBuilder.DropTable(
                name: "PermissionUser",
                schema: "reservations");

            migrationBuilder.DropTable(
                name: "RoleUser",
                schema: "reservations");

            migrationBuilder.DropTable(
                name: "Bookings",
                schema: "reservations");

            migrationBuilder.DropTable(
                name: "Tables",
                schema: "reservations");

            migrationBuilder.DropTable(
                name: "Permissions",
                schema: "reservations");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "reservations");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "reservations");
        }
    }
}
