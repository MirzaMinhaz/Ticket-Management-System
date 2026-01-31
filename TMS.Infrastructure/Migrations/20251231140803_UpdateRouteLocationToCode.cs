using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMS.Infrastructure.Migrations
{
    public partial class UpdateRouteLocationToCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. REMOVED the CreateIndex for Locations.LocationCode because it already exists.

            // 2. Drop the OLD Foreign Keys
            migrationBuilder.DropForeignKey(name: "FK_Routes_Locations_DepartureLocationId", table: "Routes");
            migrationBuilder.DropForeignKey(name: "FK_Routes_Locations_DestinationLocationId", table: "Routes");

            // 3. Drop the OLD Indexes
            migrationBuilder.DropIndex(name: "IX_Routes_DepartureLocationId", table: "Routes");
            migrationBuilder.DropIndex(name: "IX_Routes_DestinationLocationId", table: "Routes");
            migrationBuilder.DropIndex(name: "IX_Routes_DepartureLocationId_DestinationLocationId_RouteName", table: "Routes");

            // 4. Rename the Columns
            migrationBuilder.RenameColumn(name: "DepartureLocationId", table: "Routes", newName: "DepartureLocationCode");
            migrationBuilder.RenameColumn(name: "DestinationLocationId", table: "Routes", newName: "DestinationLocationCode");

            // 5. Alter Column Types to String
            migrationBuilder.AlterColumn<string>(
                name: "DepartureLocationCode",
                table: "Routes",
                type: "nvarchar(10)",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "DestinationLocationCode",
                table: "Routes",
                type: "nvarchar(10)",
                nullable: false);

            // 6. Create NEW Indexes for Routes
            migrationBuilder.CreateIndex(
                name: "IX_Routes_DepartureLocationCode",
                table: "Routes",
                column: "DepartureLocationCode");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_DestinationLocationCode",
                table: "Routes",
                column: "DestinationLocationCode");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_DepartureLocationCode_DestinationLocationCode_RouteName",
                table: "Routes",
                columns: new[] { "DepartureLocationCode", "DestinationLocationCode", "RouteName" },
                unique: true);

            // 7. Create NEW Foreign Keys
            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Locations_DepartureLocationCode",
                table: "Routes",
                column: "DepartureLocationCode",
                principalTable: "Locations",
                principalColumn: "LocationCode",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Locations_DestinationLocationCode",
                table: "Routes",
                column: "DestinationLocationCode",
                principalTable: "Locations",
                principalColumn: "LocationCode",
                onDelete: ReferentialAction.Restrict);
        }
    }
}