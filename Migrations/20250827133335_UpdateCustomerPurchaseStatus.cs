using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PJ_P_Installation_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCustomerPurchaseStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameColumn(
                name: "InvoiceNumber",
                table: "CustomerPurchases",
                newName: "OrderId");

            migrationBuilder.RenameColumn(
                name: "CustomerName",
                table: "CustomerPurchases",
                newName: "CustomerProject");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "CustomerPurchases",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "CustomerPurchases");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "CustomerPurchases",
                newName: "InvoiceNumber");

            migrationBuilder.RenameColumn(
                name: "CustomerProject",
                table: "CustomerPurchases",
                newName: "CustomerName");

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
    }
}
