using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTripTableAndRefactorTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Schedules_ScheduleId",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "ScheduleId",
                table: "Tickets",
                newName: "TripId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_ScheduleId",
                table: "Tickets",
                newName: "IX_Tickets_TripId");

            migrationBuilder.CreateTable(
                name: "Trips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ScheduleId = table.Column<int>(type: "int", nullable: false),
                    TripDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trips_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trips_ScheduleId",
                table: "Trips",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_Trips_TripDate",
                table: "Trips",
                column: "TripDate");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Trips_TripId",
                table: "Tickets",
                column: "TripId",
                principalTable: "Trips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Trips_TripId",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "Trips");

            migrationBuilder.RenameColumn(
                name: "TripId",
                table: "Tickets",
                newName: "ScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_Tickets_TripId",
                table: "Tickets",
                newName: "IX_Tickets_ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Schedules_ScheduleId",
                table: "Tickets",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
