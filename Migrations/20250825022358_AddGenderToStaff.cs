using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PJ_P_Installation_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class AddGenderToStaff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Gender",
                table: "Staffs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gender",
                table: "Staffs");
        }
    }
}
