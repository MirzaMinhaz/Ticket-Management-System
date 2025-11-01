using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateTicketCounters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TicketCounters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationCode = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    CounterName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    CounterCode = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    AddressDetails = table.Column<string>(type: "nvarchar(250)", nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    OperatingHours = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TicketCounters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TicketCounters_Locations_LocationCode",
                        column: x => x.LocationCode,
                        principalTable: "Locations",
                        principalColumn: "LocationCode",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TicketCounters_CounterCode",
                table: "TicketCounters",
                column: "CounterCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketCounters_CounterName",
                table: "TicketCounters",
                column: "CounterName");

            migrationBuilder.CreateIndex(
                name: "IX_TicketCounters_LocationCode",
                table: "TicketCounters",
                column: "LocationCode");
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
