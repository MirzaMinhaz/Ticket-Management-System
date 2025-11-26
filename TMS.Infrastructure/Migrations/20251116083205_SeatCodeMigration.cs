using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMS.Infrastructure.Migrations
{
    public partial class SeatCodeMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1️⃣ Drop foreign key from Tickets to Seats
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Seats_SeatId",
                table: "Tickets");

            // 2️⃣ Drop index on SeatId
            migrationBuilder.DropIndex(
                name: "IX_Tickets_SeatId",
                table: "Tickets");

            // 3️⃣ Drop the SeatId column
            migrationBuilder.DropColumn(
                name: "SeatId",
                table: "Tickets");

            // 4️⃣ Add SeatCode column to Tickets
            migrationBuilder.AddColumn<string>(
                name: "SeatCode",
                table: "Tickets",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            // 5️⃣ Add SeatNumber column to Tickets (if not already present)
            migrationBuilder.AddColumn<string>(
                name: "SeatNumber",
                table: "Tickets",
                type: "nvarchar(5)",
                maxLength: 5,
                nullable: false,
                defaultValue: "");

            // 6️⃣ Create indexes on new columns
            migrationBuilder.CreateIndex(
                name: "IX_Tickets_SeatCode",
                table: "Tickets",
                column: "SeatCode");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_SeatNumber",
                table: "Tickets",
                column: "SeatNumber");

            // 7️⃣ (Optional) Add foreign key if you want SeatCode to reference Seats.SeatCode
            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Seats_SeatCode",
                table: "Tickets",
                column: "SeatCode",
                principalTable: "Seats",
                principalColumn: "SeatCode",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverse the Up changes

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Seats_SeatCode",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_SeatCode",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_SeatNumber",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "SeatCode",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "SeatNumber",
                table: "Tickets");

            migrationBuilder.AddColumn<int>(
                name: "SeatId",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_SeatId",
                table: "Tickets",
                column: "SeatId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Seats_SeatId",
                table: "Tickets",
                column: "SeatId",
                principalTable: "Seats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
