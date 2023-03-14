using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeeCollectorApplication.Migrations
{
    /// <inheritdoc />
    public partial class addFieldLongtitudeLatitudeforBills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Bills",
                type: "float",
                nullable: true,
                defaultValue: 16.047079);

            migrationBuilder.AddColumn<double>(
                name: "Longtitude",
                table: "Bills",
                type: "float",
                nullable: true,
                defaultValue: 108.206230);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "latitude",
                table: "Bills");

            migrationBuilder.DropColumn(
                name: "longtitude",
                table: "Bills");
        }
    }
}
