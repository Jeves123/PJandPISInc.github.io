using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PJ_P_Installation_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class addmakesuppliernull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerPurchaseItems_Suppliers_SupplierId",
                table: "CustomerPurchaseItems");

            migrationBuilder.AlterColumn<int>(
                name: "SupplierId",
                table: "CustomerPurchaseItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerPurchaseItems_Suppliers_SupplierId",
                table: "CustomerPurchaseItems",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "SupplierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerPurchaseItems_Suppliers_SupplierId",
                table: "CustomerPurchaseItems");

            migrationBuilder.AlterColumn<int>(
                name: "SupplierId",
                table: "CustomerPurchaseItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerPurchaseItems_Suppliers_SupplierId",
                table: "CustomerPurchaseItems",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "SupplierId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
