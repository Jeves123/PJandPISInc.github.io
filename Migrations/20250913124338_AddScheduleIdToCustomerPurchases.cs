using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PJ_P_Installation_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduleIdToCustomerPurchases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LaborFeePercent",
                table: "CustomerPurchases");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "CustomerPurchases");

            migrationBuilder.DropColumn(
                name: "ProofImagePath",
                table: "CustomerPurchases");

            migrationBuilder.DropColumn(
                name: "ReferenceNumber",
                table: "CustomerPurchases");

            migrationBuilder.AddColumn<int>(
                name: "ScheduleId",
                table: "CustomerPurchases",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPurchases_ScheduleId",
                table: "CustomerPurchases",
                column: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerPurchases_Schedules_ScheduleId",
                table: "CustomerPurchases",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "ScheduleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerPurchases_Schedules_ScheduleId",
                table: "CustomerPurchases");

            migrationBuilder.DropIndex(
                name: "IX_CustomerPurchases_ScheduleId",
                table: "CustomerPurchases");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "CustomerPurchases");

            migrationBuilder.AddColumn<decimal>(
                name: "LaborFeePercent",
                table: "CustomerPurchases",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "CustomerPurchases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProofImagePath",
                table: "CustomerPurchases",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceNumber",
                table: "CustomerPurchases",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
