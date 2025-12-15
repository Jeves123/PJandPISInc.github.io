using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PJ_P_Installation_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class removedescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerPurchases_Schedules_ScheduleId1",
                table: "CustomerPurchases");

            migrationBuilder.DropIndex(
                name: "IX_CustomerPurchases_ScheduleId1",
                table: "CustomerPurchases");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "CustomerPurchases");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "CustomerPurchases");

            migrationBuilder.DropColumn(
                name: "ScheduleId1",
                table: "CustomerPurchases");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CustomerPurchases",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ScheduleId",
                table: "CustomerPurchases",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ScheduleId1",
                table: "CustomerPurchases",
                type: "int",
                nullable: true);

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
        }
    }
}
