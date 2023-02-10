using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeeCollectorApplication.Migrations
{
    /// <inheritdoc />
    public partial class addBillHistoryToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BillHistories",
                columns: table => new
                {
                    Billid = table.Column<int>(name: "Bill_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    licenseplatenumber = table.Column<string>(name: "license_plate_number", type: "nvarchar(20)", maxLength: 20, nullable: false),
                    price = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillHistories", x => x.Billid);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillHistories");
        }
    }
}
