using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KuroOpti.Data.Migrations
{
    /// <inheritdoc />
    public partial class JustFuelStation2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Petrol95Price",
                table: "FuelStations",
                newName: "PetrolPrice");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PetrolPrice",
                table: "FuelStations",
                newName: "Petrol95Price");
        }
    }
}
