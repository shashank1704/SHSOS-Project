using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SHSOS.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_SnotSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "snot");

            migrationBuilder.CreateTable(
                name: "hospitals",
                schema: "snot",
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
                schema: "snot",
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
                        principalSchema: "snot",
                        principalTable: "hospitals",
                        principalColumn: "HospitalID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Alert",
                schema: "snot",
                columns: table => new
                {
                    AlertID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentID = table.Column<int>(type: "int", nullable: false),
                    AlertType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ThresholdValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActualValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false),
                    ResolvedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResolutionNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alert", x => x.AlertID);
                    table.ForeignKey(
                        name: "FK_Alert_Departments_DepartmentID",
                        column: x => x.DepartmentID,
                        principalSchema: "snot",
                        principalTable: "Departments",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EnergyConsumption",
                schema: "snot",
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
                        principalSchema: "snot",
                        principalTable: "Departments",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResourceThreshold",
                schema: "snot",
                columns: table => new
                {
                    ThresholdID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentID = table.Column<int>(type: "int", nullable: false),
                    ResourceType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ThresholdName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    WarningThreshold = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CriticalThreshold = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceThreshold", x => x.ThresholdID);
                    table.ForeignKey(
                        name: "FK_ResourceThreshold_Departments_DepartmentID",
                        column: x => x.DepartmentID,
                        principalSchema: "snot",
                        principalTable: "Departments",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SustainabilityMetrics",
                schema: "snot",
                columns: table => new
                {
                    MetricID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentID = table.Column<int>(type: "int", nullable: false),
                    CalculationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SustainabilityScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnergyEfficiencyScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WaterEfficiencyScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WasteManagementScore = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCarbonEmissionKg = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CarbonReductionPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PotentialSavings = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostSavingsPercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EnergyPerBed = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WaterPerBed = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WastePerBed = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ComplianceViolations = table.Column<int>(type: "int", nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Recommendations = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SustainabilityMetrics", x => x.MetricID);
                    table.ForeignKey(
                        name: "FK_SustainabilityMetrics_Departments_DepartmentID",
                        column: x => x.DepartmentID,
                        principalSchema: "snot",
                        principalTable: "Departments",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WasteManagement",
                schema: "snot",
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
                        principalSchema: "snot",
                        principalTable: "Departments",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WaterConsumption",
                schema: "snot",
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
                        principalSchema: "snot",
                        principalTable: "Departments",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alert_DepartmentID",
                schema: "snot",
                table: "Alert",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_HospitalID",
                schema: "snot",
                table: "Departments",
                column: "HospitalID");

            migrationBuilder.CreateIndex(
                name: "IX_EnergyConsumption_DepartmentID",
                schema: "snot",
                table: "EnergyConsumption",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceThreshold_DepartmentID",
                schema: "snot",
                table: "ResourceThreshold",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_SustainabilityMetrics_DepartmentID",
                schema: "snot",
                table: "SustainabilityMetrics",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_WasteManagement_DepartmentID",
                schema: "snot",
                table: "WasteManagement",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_WaterConsumption_DepartmentID",
                schema: "snot",
                table: "WaterConsumption",
                column: "DepartmentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alert",
                schema: "snot");

            migrationBuilder.DropTable(
                name: "EnergyConsumption",
                schema: "snot");

            migrationBuilder.DropTable(
                name: "ResourceThreshold",
                schema: "snot");

            migrationBuilder.DropTable(
                name: "SustainabilityMetrics",
                schema: "snot");

            migrationBuilder.DropTable(
                name: "WasteManagement",
                schema: "snot");

            migrationBuilder.DropTable(
                name: "WaterConsumption",
                schema: "snot");

            migrationBuilder.DropTable(
                name: "Departments",
                schema: "snot");

            migrationBuilder.DropTable(
                name: "hospitals",
                schema: "snot");
        }
    }
}
