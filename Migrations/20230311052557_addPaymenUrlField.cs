using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeeCollectorApplication.Migrations
{
    /// <inheritdoc />
    public partial class addPaymenUrlField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "longtitude",
                table: "Bills",
                newName: "Longtitude");

            migrationBuilder.RenameColumn(
                name: "latitude",
                table: "Bills",
                newName: "Latitude");

            migrationBuilder.AlterColumn<float>(
                name: "Longtitude",
                table: "Bills",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<float>(
                name: "Latitude",
                table: "Bills",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<string>(
                name: "PaymentUrl",
                table: "Bills",
                type: "nvarchar(150)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentUrl",
                table: "Bills");

            migrationBuilder.RenameColumn(
                name: "Longtitude",
                table: "Bills",
                newName: "longtitude");

            migrationBuilder.RenameColumn(
                name: "Latitude",
                table: "Bills",
                newName: "latitude");

            migrationBuilder.AlterColumn<double>(
                name: "longtitude",
                table: "Bills",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<double>(
                name: "latitude",
                table: "Bills",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");
        }
    }
}
