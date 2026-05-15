using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KuroOpti.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixFuelStationColumnTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
ALTER TABLE FuelStations 
MODIFY COLUMN Name VARCHAR(255) NOT NULL,
MODIFY COLUMN Address VARCHAR(255) NOT NULL,
MODIFY COLUMN Municipality VARCHAR(255) NOT NULL;
"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder) { }
    }
}
