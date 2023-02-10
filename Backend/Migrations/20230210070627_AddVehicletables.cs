using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeeCollectorApplication.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicletables : Migration
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
                    Billdatetime = table.Column<DateTime>(name: "Bill_datetime", type: "datetime2", nullable: true),
                    price = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillHistories", x => x.Billid);
                });

            migrationBuilder.CreateTable(
                name: "Bills",
                columns: table => new
                {
                    Billid = table.Column<int>(name: "Bill_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    licenseplatenumber = table.Column<string>(name: "license_plate_number", type: "nvarchar(20)", maxLength: 20, nullable: false),
                    price = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bills", x => x.Billid);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    payid = table.Column<int>(name: "pay_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    licenseplatenumber = table.Column<string>(name: "license_plate_number", type: "nvarchar(20)", maxLength: 20, nullable: false),
                    paidprice = table.Column<float>(name: "paid_price", type: "real", nullable: false),
                    paidtime = table.Column<DateTime>(name: "paid_time", type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.payid);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    Vehicleid = table.Column<int>(name: "Vehicle_id", type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    licenseplatenumber = table.Column<string>(name: "license_plate_number", type: "nvarchar(20)", maxLength: 20, nullable: false),
                    imageurl = table.Column<string>(name: "image_url", type: "nvarchar(200)", maxLength: 200, nullable: false),
                    vehicletype = table.Column<string>(name: "vehicle_type", type: "nvarchar(max)", nullable: false),
                    timestart = table.Column<DateTime>(name: "time_start", type: "datetime2", maxLength: 20, nullable: false),
                    timeend = table.Column<DateTime>(name: "time_end", type: "datetime2", maxLength: 60, nullable: false),
                    location = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.Vehicleid);
                });

            migrationBuilder.CreateTable(
                name: "VehicleTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vehicletype = table.Column<string>(name: "vehicle_type", type: "nvarchar(max)", nullable: false),
                    price = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleTypes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BillHistories");

            migrationBuilder.DropTable(
                name: "Bills");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "VehicleTypes");
        }
    }
}
