using Microsoft.EntityFrameworkCore.Migrations;

namespace P9_Backend.Migrations
{
    public partial class AddBearingToPosition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Bearing",
                table: "Position",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bearing",
                table: "Position");
        }
    }
}
