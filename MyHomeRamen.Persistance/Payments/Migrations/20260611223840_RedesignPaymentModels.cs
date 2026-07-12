using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyHomeRamen.Persistance.Payments.Migrations
{
    /// <inheritdoc />
    public partial class RedesignPaymentModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Payments_DefaultMethodId",
                schema: "payments",
                table: "Users");

            migrationBuilder.DropTable(
                name: "PaymentGroupProviders",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "PaymentProviderPayments",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "PaymentUser",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "PaymentGroups",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "PaymentProviders",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "Payments",
                schema: "payments");

            migrationBuilder.DropIndex(
                name: "IX_Users_DefaultMethodId",
                schema: "payments",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DefaultMethodId",
                schema: "payments",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "PaymentGateways",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentGateways", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentChannels",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    PaymentGatewayId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentGatewayId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PaymentMethodId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PaymentMethodId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentChannels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentChannels_PaymentGateways_PaymentGatewayId",
                        column: x => x.PaymentGatewayId,
                        principalSchema: "payments",
                        principalTable: "PaymentGateways",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentChannels_PaymentGateways_PaymentGatewayId1",
                        column: x => x.PaymentGatewayId1,
                        principalSchema: "payments",
                        principalTable: "PaymentGateways",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PaymentChannels_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalSchema: "payments",
                        principalTable: "PaymentMethods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PaymentChannels_PaymentMethods_PaymentMethodId1",
                        column: x => x.PaymentMethodId1,
                        principalSchema: "payments",
                        principalTable: "PaymentMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentChannels_PaymentGatewayId",
                schema: "payments",
                table: "PaymentChannels",
                column: "PaymentGatewayId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentChannels_PaymentGatewayId1",
                schema: "payments",
                table: "PaymentChannels",
                column: "PaymentGatewayId1");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentChannels_PaymentMethodId",
                schema: "payments",
                table: "PaymentChannels",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentChannels_PaymentMethodId1",
                schema: "payments",
                table: "PaymentChannels",
                column: "PaymentMethodId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentChannels",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "PaymentGateways",
                schema: "payments");

            migrationBuilder.DropTable(
                name: "PaymentMethods",
                schema: "payments");

            migrationBuilder.AddColumn<Guid>(
                name: "DefaultMethodId",
                schema: "payments",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PaymentGroups",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentProviders",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentProviders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedOn = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RestaurantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentGroupProviders",
                schema: "payments",
                columns: table => new
                {
                    PaymentGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentProvidersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentGroupProviders", x => new { x.PaymentGroupId, x.PaymentProvidersId });
                    table.ForeignKey(
                        name: "FK_PaymentGroupProviders_PaymentGroups_PaymentGroupId",
                        column: x => x.PaymentGroupId,
                        principalSchema: "payments",
                        principalTable: "PaymentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentGroupProviders_PaymentProviders_PaymentProvidersId",
                        column: x => x.PaymentProvidersId,
                        principalSchema: "payments",
                        principalTable: "PaymentProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentProviderPayments",
                schema: "payments",
                columns: table => new
                {
                    PaymentProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentProviderPayments", x => new { x.PaymentProviderId, x.PaymentsId });
                    table.ForeignKey(
                        name: "FK_PaymentProviderPayments_PaymentProviders_PaymentProviderId",
                        column: x => x.PaymentProviderId,
                        principalSchema: "payments",
                        principalTable: "PaymentProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentProviderPayments_Payments_PaymentsId",
                        column: x => x.PaymentsId,
                        principalSchema: "payments",
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentUser",
                schema: "payments",
                columns: table => new
                {
                    PaymentsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentUser", x => new { x.PaymentsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_PaymentUser_Payments_PaymentsId",
                        column: x => x.PaymentsId,
                        principalSchema: "payments",
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PaymentUser_Users_UsersId",
                        column: x => x.UsersId,
                        principalSchema: "payments",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_DefaultMethodId",
                schema: "payments",
                table: "Users",
                column: "DefaultMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGroupProviders_PaymentProvidersId",
                schema: "payments",
                table: "PaymentGroupProviders",
                column: "PaymentProvidersId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentProviderPayments_PaymentsId",
                schema: "payments",
                table: "PaymentProviderPayments",
                column: "PaymentsId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentUser_UsersId",
                schema: "payments",
                table: "PaymentUser",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Payments_DefaultMethodId",
                schema: "payments",
                table: "Users",
                column: "DefaultMethodId",
                principalSchema: "payments",
                principalTable: "Payments",
                principalColumn: "Id");
        }
    }
}
