using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERP_Finance.Migrations
{
    /// <inheritdoc />
    public partial class AddCancellationReasonToTab : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Tabs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Tabs");
        }
    }
}
