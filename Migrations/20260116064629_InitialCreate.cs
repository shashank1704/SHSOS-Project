using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SHSOS.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "hospitals",
                columns: table => new
                {
                    HospitalID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HospitalName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hospitals", x => x.HospitalID);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HospitalID = table.Column<int>(type: "int", nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FloorNumber = table.Column<int>(type: "int", nullable: false),
                    Inactive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentID);
                    table.ForeignKey(
                        name: "FK_Departments_hospitals_HospitalID",
                        column: x => x.HospitalID,
                        principalTable: "hospitals",
                        principalColumn: "HospitalID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EnergyConsumption",
                columns: table => new
                {
                    EnergyConsumptionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentID = table.Column<int>(type: "int", nullable: false),
                    ConsumptionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReadingTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    MeterReadingStart = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitsConsumedkWh = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UsageCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PeakHourFlag = table.Column<bool>(type: "bit", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CarbonEmissionsKg = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnergyConsumption", x => x.EnergyConsumptionID);
                    table.ForeignKey(
                        name: "FK_EnergyConsumption_Departments_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WasteManagement",
                columns: table => new
                {
                    WasteRecordID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentID = table.Column<int>(type: "int", nullable: false),
                    WasteType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WasteCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WasteWeight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SegregationStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisposalMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisposalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DisinfectionCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ComplianceStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CollectionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WasteManagement", x => x.WasteRecordID);
                    table.ForeignKey(
                        name: "FK_WasteManagement_Departments_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WaterConsumption",
                columns: table => new
                {
                    ConsumptionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentID = table.Column<int>(type: "int", nullable: false),
                    ConsumptionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReadingTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    ReadingEnd = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitsConsumedLiters = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LeakageDetected = table.Column<bool>(type: "bit", nullable: false),
                    WeatherCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WeatherCondition = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaterConsumption", x => x.ConsumptionID);
                    table.ForeignKey(
                        name: "FK_WaterConsumption_Departments_DepartmentID",
                        column: x => x.DepartmentID,
                        principalTable: "Departments",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Departments_HospitalID",
                table: "Departments",
                column: "HospitalID");

            migrationBuilder.CreateIndex(
                name: "IX_EnergyConsumption_DepartmentID",
                table: "EnergyConsumption",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_WasteManagement_DepartmentID",
                table: "WasteManagement",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_WaterConsumption_DepartmentID",
                table: "WaterConsumption",
                column: "DepartmentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EnergyConsumption");

            migrationBuilder.DropTable(
                name: "WasteManagement");

            migrationBuilder.DropTable(
                name: "WaterConsumption");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropTable(
                name: "hospitals");
        }
    }
}
