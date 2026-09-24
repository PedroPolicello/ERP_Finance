using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP_Finance.Migrations
{
    /// <inheritdoc />
    public partial class AddTableIdToTab : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TableNumber",
                table: "Tabs");

            migrationBuilder.AddColumn<Guid>(
                name: "TableId",
                table: "Tabs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tabs_TableId",
                table: "Tabs",
                column: "TableId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tabs_Tables_TableId",
                table: "Tabs",
                column: "TableId",
                principalTable: "Tables",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tabs_Tables_TableId",
                table: "Tabs");

            migrationBuilder.DropIndex(
                name: "IX_Tabs_TableId",
                table: "Tabs");

            migrationBuilder.DropColumn(
                name: "TableId",
                table: "Tabs");

            migrationBuilder.AddColumn<int>(
                name: "TableNumber",
                table: "Tabs",
                type: "int",
                nullable: true);
        }
    }
}
