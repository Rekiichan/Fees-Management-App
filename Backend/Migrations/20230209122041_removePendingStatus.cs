using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeeCollectorApplication.Migrations
{
    /// <inheritdoc />
    public partial class removePendingStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "pending_status",
                table: "Bills");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "pending_status",
                table: "Bills",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
