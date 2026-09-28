using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureUserOtp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserOtps_UserId_OtpType_IsUsed",
                table: "UserOtps");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                table: "UserOtps",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "UserOtps",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_UserOtps_UserId_OtpType_IsUsed_ExpiresAt",
                table: "UserOtps",
                columns: new[] { "UserId", "OtpType", "IsUsed", "ExpiresAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserOtps_UserId_OtpType_IsUsed_ExpiresAt",
                table: "UserOtps");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                table: "UserOtps",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "UserOtps",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.CreateIndex(
                name: "IX_UserOtps_UserId_OtpType_IsUsed",
                table: "UserOtps",
                columns: new[] { "UserId", "OtpType", "IsUsed" });
        }
    }
}
