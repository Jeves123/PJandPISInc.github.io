using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PJ_P_Installation_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class removesupplierid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerPurchaseItems_Suppliers_SupplierId",
                table: "CustomerPurchaseItems");

            migrationBuilder.DropIndex(
                name: "IX_CustomerPurchaseItems_SupplierId",
                table: "CustomerPurchaseItems");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "CustomerPurchaseItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "CustomerPurchaseItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPurchaseItems_SupplierId",
                table: "CustomerPurchaseItems",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerPurchaseItems_Suppliers_SupplierId",
                table: "CustomerPurchaseItems",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "SupplierId");
        }
    }
}
