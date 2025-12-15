using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PJ_P_Installation_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class addnewcustomerpurchaseschedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerPurchases_Schedules_ScheduleId",
                table: "CustomerPurchases");

            migrationBuilder.DropIndex(
                name: "IX_CustomerPurchases_ScheduleId",
                table: "CustomerPurchases");

            migrationBuilder.AddColumn<int>(
                name: "CustomerPurchaseId",
                table: "Schedules",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ScheduleId1",
                table: "CustomerPurchases",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_CustomerPurchaseId",
                table: "Schedules",
                column: "CustomerPurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerPurchases_ScheduleId1",
                table: "CustomerPurchases",
                column: "ScheduleId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerPurchases_Schedules_ScheduleId1",
                table: "CustomerPurchases",
                column: "ScheduleId1",
                principalTable: "Schedules",
                principalColumn: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Schedules_CustomerPurchases_CustomerPurchaseId",
                table: "Schedules",
                column: "CustomerPurchaseId",
                principalTable: "CustomerPurchases",
                principalColumn: "CustomerPurchaseId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerPurchases_Schedules_ScheduleId1",
                table: "CustomerPurchases");

            migrationBuilder.DropForeignKey(
                name: "FK_Schedules_CustomerPurchases_CustomerPurchaseId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_CustomerPurchaseId",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_CustomerPurchases_ScheduleId1",
                table: "CustomerPurchases");

            migrationBuilder.DropColumn(
                name: "CustomerPurchaseId",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "ScheduleId1",
                table: "CustomerPurchases");

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
    }
}
