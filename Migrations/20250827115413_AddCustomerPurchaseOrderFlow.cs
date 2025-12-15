using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PJ_P_Installation_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerPurchaseOrderFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AmountPaid",
                table: "CustomerPurchases",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "InstallationFee",
                table: "CustomerPurchases",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresInstallation",
                table: "CustomerPurchases",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalAmount",
                table: "CustomerPurchases",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountPaid",
                table: "CustomerPurchases");

            migrationBuilder.DropColumn(
                name: "InstallationFee",
                table: "CustomerPurchases");

            migrationBuilder.DropColumn(
                name: "RequiresInstallation",
                table: "CustomerPurchases");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "CustomerPurchases");
        }
    }
}
