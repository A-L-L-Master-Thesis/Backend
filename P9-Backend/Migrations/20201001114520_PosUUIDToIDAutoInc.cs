using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.Data.EntityFrameworkCore.Metadata;

namespace P9_Backend.Migrations
{
    public partial class PosUUIDToIDAutoInc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Position",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Latitude = table.Column<double>(nullable: false),
                    Longitude = table.Column<double>(nullable: false),
                    Altitude = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Position", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Drones",
                columns: table => new
                {
                    UUID = table.Column<string>(nullable: false),
                    LastUpdate = table.Column<DateTime>(nullable: false),
                    CurrentPositionID = table.Column<int>(nullable: true),
                    BatteryPercentage = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    IP = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drones", x => x.UUID);
                    table.ForeignKey(
                        name: "FK_Drones_Position_CurrentPositionID",
                        column: x => x.CurrentPositionID,
                        principalTable: "Position",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Drones_CurrentPositionID",
                table: "Drones",
                column: "CurrentPositionID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Drones");

            migrationBuilder.DropTable(
                name: "Position");
        }
    }
}
