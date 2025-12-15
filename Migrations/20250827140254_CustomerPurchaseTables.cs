using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PJ_P_Installation_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class CustomerPurchaseTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerPurchaseItems",
                columns: table => new
                {
                    CustomerPurchaseItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerPurchaseId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerPurchaseItems", x => x.CustomerPurchaseItemId);
                    table.ForeignKey(
                        name: "FK_CustomerPurchaseItems_CustomerPurchases_CustomerPurchaseId",
                        column: x => x.CustomerPurchaseId,
                        principalTable: "CustomerPurchases",
                        principalColumn: "CustomerPurchaseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerPurchaseItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPurchaseItems_CustomerPurchaseId",
                table: "CustomerPurchaseItems",
                column: "CustomerPurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPurchaseItems_ProductId",
                table: "CustomerPurchaseItems",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerPurchaseItems");
        }
    }
}
