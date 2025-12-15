using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PJ_P_Installation_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderAndTrackingNumbersToPurchase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Purchases");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryTrackingNumber",
                table: "Purchases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OrderNumber",
                table: "Purchases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryTrackingNumber",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                table: "Purchases");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Purchases",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
