using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

namespace coolgym_webapi.Migrations
{
    /// <inheritdoc />
    public partial class AddRentalCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "billing_invoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CompanyName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsDeleted = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_billing_invoices", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "equipments",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    model = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    manufacturer = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    serial_number = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false),
                    installation_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    power_watts = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    is_powered_on = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    active_status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    image = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    location_name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    location_address = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    usage_total_minutes = table.Column<int>(type: "int", nullable: false),
                    usage_today_minutes = table.Column<int>(type: "int", nullable: false),
                    usage_calories_today = table.Column<int>(type: "int", nullable: false),
                    control_power = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    control_current_level = table.Column<int>(type: "int", nullable: false),
                    control_set_level = table.Column<int>(type: "int", nullable: false),
                    control_min_level_range = table.Column<int>(type: "int", nullable: false),
                    control_max_level_range = table.Column<int>(type: "int", nullable: false),
                    control_status = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false),
                    maintenance_last_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    maintenance_next_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    created_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    is_deleted = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_equipments", x => x.id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "rental_items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                    type = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false),
                    model = table.Column<string>(type: "varchar(60)", maxLength: 60, nullable: false),
                    monthly_price_amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    monthly_price_currency = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    image_url = table.Column<string>(type: "varchar(256)", maxLength: 256, nullable: false),
                    is_available = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    created_date = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    updated_date = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    is_deleted = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rental_items", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_billing_invoices_IssuedAt",
                table: "billing_invoices",
                column: "IssuedAt");

            migrationBuilder.CreateIndex(
                name: "IX_billing_invoices_UserId",
                table: "billing_invoices",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "ix_equipments_serial_number",
                table: "equipments",
                column: "serial_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_rental_items_name_model",
                table: "rental_items",
                columns: new[] { "name", "model" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "billing_invoices");

            migrationBuilder.DropTable(
                name: "equipments");

            migrationBuilder.DropTable(
                name: "rental_items");
        }
    }
}
