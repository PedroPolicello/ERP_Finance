using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP_Finance.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexesOnTabNumberAndOrderNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tabs_TabNumber",
                table: "Tabs");

            migrationBuilder.DropIndex(
                name: "IX_Orders_OrderNumber",
                table: "Orders");
        }
    }
}
