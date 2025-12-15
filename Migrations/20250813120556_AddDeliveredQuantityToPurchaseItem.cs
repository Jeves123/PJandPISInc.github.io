using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PJ_P_Installation_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveredQuantityToPurchaseItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeliveredQuantity",
                table: "PurchaseItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveredQuantity",
                table: "PurchaseItems");
        }
    }
}
