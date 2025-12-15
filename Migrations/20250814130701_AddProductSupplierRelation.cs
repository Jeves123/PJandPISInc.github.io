using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PJ_P_Installation_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class AddProductSupplierRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSupplier_Products_ProductId",
                table: "ProductSupplier");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductSupplier_Suppliers_SupplierId",
                table: "ProductSupplier");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductSupplier",
                table: "ProductSupplier");

            migrationBuilder.RenameTable(
                name: "ProductSupplier",
                newName: "ProductSuppliers");

            migrationBuilder.RenameIndex(
                name: "IX_ProductSupplier_SupplierId",
                table: "ProductSuppliers",
                newName: "IX_ProductSuppliers_SupplierId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductSuppliers",
                table: "ProductSuppliers",
                columns: new[] { "ProductId", "SupplierId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSuppliers_Products_ProductId",
                table: "ProductSuppliers",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSuppliers_Suppliers_SupplierId",
                table: "ProductSuppliers",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "SupplierId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSuppliers_Products_ProductId",
                table: "ProductSuppliers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductSuppliers_Suppliers_SupplierId",
                table: "ProductSuppliers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductSuppliers",
                table: "ProductSuppliers");

            migrationBuilder.RenameTable(
                name: "ProductSuppliers",
                newName: "ProductSupplier");

            migrationBuilder.RenameIndex(
                name: "IX_ProductSuppliers_SupplierId",
                table: "ProductSupplier",
                newName: "IX_ProductSupplier_SupplierId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductSupplier",
                table: "ProductSupplier",
                columns: new[] { "ProductId", "SupplierId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSupplier_Products_ProductId",
                table: "ProductSupplier",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSupplier_Suppliers_SupplierId",
                table: "ProductSupplier",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "SupplierId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
