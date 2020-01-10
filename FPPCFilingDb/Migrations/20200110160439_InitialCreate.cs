using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FPPCFilingDb.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Securities",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Issuer = table.Column<string>(nullable: true),
                    Ticker = table.Column<string>(nullable: true),
                    Category = table.Column<string>(nullable: true),
                    Reportable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Securities", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "HistoricalData",
                columns: table => new
                {
                    SecurityInfoID = table.Column<string>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    Open = table.Column<double>(nullable: false),
                    High = table.Column<double>(nullable: false),
                    Low = table.Column<double>(nullable: false),
                    Close = table.Column<double>(nullable: false),
                    Volume = table.Column<double>(nullable: false),
                    SecurityInfoID1 = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricalData", x => new { x.SecurityInfoID, x.Timestamp });
                    table.ForeignKey(
                        name: "FK_HistoricalData_Securities_SecurityInfoID1",
                        column: x => x.SecurityInfoID1,
                        principalTable: "Securities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalData_SecurityInfoID1",
                table: "HistoricalData",
                column: "SecurityInfoID1");

            migrationBuilder.CreateIndex(
                name: "IX_Securities_Issuer",
                table: "Securities",
                column: "Issuer",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Securities_Ticker",
                table: "Securities",
                column: "Ticker",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoricalData");

            migrationBuilder.DropTable(
                name: "Securities");
        }
    }
}
