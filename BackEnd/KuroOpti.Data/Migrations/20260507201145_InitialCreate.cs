using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KuroOpti.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase().Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "FuelStations",
                    columns: table => new
                    {
                        Id = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation(
                                "MySql:ValueGenerationStrategy",
                                MySqlValueGenerationStrategy.IdentityColumn
                            ),
                        ExternalId = table
                            .Column<string>(type: "longtext", nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        Name = table
                            .Column<string>(type: "longtext", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        Brand = table
                            .Column<string>(type: "longtext", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        Latitude = table.Column<decimal>(
                            type: "decimal(9,6)",
                            precision: 9,
                            scale: 6,
                            nullable: false
                        ),
                        Longitude = table.Column<decimal>(
                            type: "decimal(9,6)",
                            precision: 9,
                            scale: 6,
                            nullable: false
                        ),
                        Address = table
                            .Column<string>(type: "longtext", nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        City = table
                            .Column<string>(type: "longtext", nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_FuelStations", x => x.Id);
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "Users",
                    columns: table => new
                    {
                        Id = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation(
                                "MySql:ValueGenerationStrategy",
                                MySqlValueGenerationStrategy.IdentityColumn
                            ),
                        Email = table
                            .Column<string>(type: "varchar(255)", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        PasswordHash = table
                            .Column<string>(type: "longtext", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        Role = table
                            .Column<string>(type: "longtext", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Users", x => x.Id);
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "FuelPrices",
                    columns: table => new
                    {
                        Id = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation(
                                "MySql:ValueGenerationStrategy",
                                MySqlValueGenerationStrategy.IdentityColumn
                            ),
                        StationId = table.Column<int>(type: "int", nullable: false),
                        FuelType = table
                            .Column<string>(type: "longtext", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        Price = table.Column<decimal>(
                            type: "decimal(6,3)",
                            precision: 6,
                            scale: 3,
                            nullable: false
                        ),
                        ValidFrom = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                        ValidTo = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_FuelPrices", x => x.Id);
                        table.ForeignKey(
                            name: "FK_FuelPrices_FuelStations_StationId",
                            column: x => x.StationId,
                            principalTable: "FuelStations",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "Routes",
                    columns: table => new
                    {
                        Id = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation(
                                "MySql:ValueGenerationStrategy",
                                MySqlValueGenerationStrategy.IdentityColumn
                            ),
                        UserId = table.Column<int>(type: "int", nullable: false),
                        Name = table
                            .Column<string>(type: "longtext", nullable: true)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                        StartLat = table.Column<decimal>(
                            type: "decimal(9,6)",
                            precision: 9,
                            scale: 6,
                            nullable: false
                        ),
                        StartLng = table.Column<decimal>(
                            type: "decimal(9,6)",
                            precision: 9,
                            scale: 6,
                            nullable: false
                        ),
                        EndLat = table.Column<decimal>(
                            type: "decimal(9,6)",
                            precision: 9,
                            scale: 6,
                            nullable: false
                        ),
                        EndLng = table.Column<decimal>(
                            type: "decimal(9,6)",
                            precision: 9,
                            scale: 6,
                            nullable: false
                        ),
                        Polyline = table
                            .Column<string>(type: "longtext", nullable: false)
                            .Annotation("MySql:CharSet", "utf8mb4"),
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_Routes", x => x.Id);
                        table.ForeignKey(
                            name: "FK_Routes_Users_UserId",
                            column: x => x.UserId,
                            principalTable: "Users",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder
                .CreateTable(
                    name: "SearchLogs",
                    columns: table => new
                    {
                        Id = table
                            .Column<int>(type: "int", nullable: false)
                            .Annotation(
                                "MySql:ValueGenerationStrategy",
                                MySqlValueGenerationStrategy.IdentityColumn
                            ),
                        UserId = table.Column<int>(type: "int", nullable: true),
                        RouteId = table.Column<int>(type: "int", nullable: true),
                        SearchTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                        ResultsCount = table.Column<int>(type: "int", nullable: false),
                        MinPriceFound = table.Column<decimal>(
                            type: "decimal(65,30)",
                            nullable: true
                        ),
                    },
                    constraints: table =>
                    {
                        table.PrimaryKey("PK_SearchLogs", x => x.Id);
                        table.ForeignKey(
                            name: "FK_SearchLogs_Routes_RouteId",
                            column: x => x.RouteId,
                            principalTable: "Routes",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.SetNull
                        );
                        table.ForeignKey(
                            name: "FK_SearchLogs_Users_UserId",
                            column: x => x.UserId,
                            principalTable: "Users",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.SetNull
                        );
                    }
                )
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_FuelPrices_StationId",
                table: "FuelPrices",
                column: "StationId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Routes_UserId",
                table: "Routes",
                column: "UserId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_SearchLogs_RouteId",
                table: "SearchLogs",
                column: "RouteId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_SearchLogs_UserId",
                table: "SearchLogs",
                column: "UserId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "FuelPrices");

            migrationBuilder.DropTable(name: "SearchLogs");

            migrationBuilder.DropTable(name: "FuelStations");

            migrationBuilder.DropTable(name: "Routes");

            migrationBuilder.DropTable(name: "Users");
        }
    }
}
