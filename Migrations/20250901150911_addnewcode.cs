using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PJ_P_Installation_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class addnewcode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "CustomerPurchaseItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "CustomerPurchaseItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPurchaseItems_SupplierId",
                table: "CustomerPurchaseItems",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerPurchaseItems_Suppliers_SupplierId",
                table: "CustomerPurchaseItems",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "SupplierId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerPurchaseItems_Suppliers_SupplierId",
                table: "CustomerPurchaseItems");

            migrationBuilder.DropIndex(
                name: "IX_CustomerPurchaseItems_SupplierId",
                table: "CustomerPurchaseItems");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "CustomerPurchaseItems");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "CustomerPurchaseItems");
        }
    }
}
