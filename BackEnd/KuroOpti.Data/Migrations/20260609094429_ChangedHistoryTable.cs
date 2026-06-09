using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KuroOpti.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangedHistoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DistanceKm",
                table: "RoutePlanningHistories",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EndLat",
                table: "RoutePlanningHistories",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "EndLng",
                table: "RoutePlanningHistories",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "FuelType",
                table: "RoutePlanningHistories",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Polyline",
                table: "RoutePlanningHistories",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<double>(
                name: "StartLat",
                table: "RoutePlanningHistories",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "StartLng",
                table: "RoutePlanningHistories",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DistanceKm",
                table: "RoutePlanningHistories");

            migrationBuilder.DropColumn(
                name: "EndLat",
                table: "RoutePlanningHistories");

            migrationBuilder.DropColumn(
                name: "EndLng",
                table: "RoutePlanningHistories");

            migrationBuilder.DropColumn(
                name: "FuelType",
                table: "RoutePlanningHistories");

            migrationBuilder.DropColumn(
                name: "Polyline",
                table: "RoutePlanningHistories");

            migrationBuilder.DropColumn(
                name: "StartLat",
                table: "RoutePlanningHistories");

            migrationBuilder.DropColumn(
                name: "StartLng",
                table: "RoutePlanningHistories");
        }
    }
}
