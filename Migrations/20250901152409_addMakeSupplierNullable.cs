using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PJ_P_Installation_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class addMakeSupplierNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerPurchaseItems_Products_ProductId",
                table: "CustomerPurchaseItems");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "CustomerPurchaseItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerPurchaseItems_Products_ProductId",
                table: "CustomerPurchaseItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerPurchaseItems_Products_ProductId",
                table: "CustomerPurchaseItems");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "CustomerPurchaseItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerPurchaseItems_Products_ProductId",
                table: "CustomerPurchaseItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
