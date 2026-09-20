using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP_Finance.Migrations
{
    /// <inheritdoc />
    public partial class AddTabDateAndFixTabUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tabs_TabNumber",
                table: "Tabs");

            migrationBuilder.DropIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_TabId",
                table: "Orders");

            migrationBuilder.AddColumn<DateOnly>(
                name: "TabDate",
                table: "Tabs",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.CreateIndex(
                name: "IX_Tabs_TabNumber_TabDate",
                table: "Tabs",
                columns: new[] { "TabNumber", "TabDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TabId_OrderNumber",
                table: "Orders",
                columns: new[] { "TabId", "OrderNumber" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tabs_TabNumber_TabDate",
                table: "Tabs");

            migrationBuilder.DropIndex(
                name: "IX_Orders_TabId_OrderNumber",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TabDate",
                table: "Tabs");

            migrationBuilder.CreateIndex(
                name: "IX_Tabs_TabNumber",
                table: "Tabs",
                column: "TabNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders",
                column: "OrderNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TabId",
                table: "Orders",
                column: "TabId");
        }
    }
}
