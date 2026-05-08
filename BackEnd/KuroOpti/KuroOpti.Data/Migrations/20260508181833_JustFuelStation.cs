using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KuroOpti.Data.Migrations
{
    /// <inheritdoc />
    public partial class JustFuelStation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FuelPrices");

            migrationBuilder.DropColumn(
                name: "City",
                table: "FuelStations");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "FuelStations");

            migrationBuilder.RenameColumn(
                name: "Brand",
                table: "FuelStations",
                newName: "Municipality");

            migrationBuilder.UpdateData(
                table: "FuelStations",
                keyColumn: "Address",
                keyValue: null,
                column: "Address",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "FuelStations",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "DieselPrice",
                table: "FuelStations",
                type: "decimal(6,3)",
                precision: 6,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "LpgPrice",
                table: "FuelStations",
                type: "decimal(6,3)",
                precision: 6,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Petrol95Price",
                table: "FuelStations",
                type: "decimal(6,3)",
                precision: 6,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "FuelStations",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DieselPrice",
                table: "FuelStations");

            migrationBuilder.DropColumn(
                name: "LpgPrice",
                table: "FuelStations");

            migrationBuilder.DropColumn(
                name: "Petrol95Price",
                table: "FuelStations");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "FuelStations");

            migrationBuilder.RenameColumn(
                name: "Municipality",
                table: "FuelStations",
                newName: "Brand");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "FuelStations",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "FuelStations",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "FuelStations",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FuelPrices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StationId = table.Column<int>(type: "int", nullable: false),
                    FuelType = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Price = table.Column<decimal>(type: "decimal(6,3)", precision: 6, scale: 3, nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ValidTo = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FuelPrices_FuelStations_StationId",
                        column: x => x.StationId,
                        principalTable: "FuelStations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_FuelPrices_StationId",
                table: "FuelPrices",
                column: "StationId");
        }
    }
}
