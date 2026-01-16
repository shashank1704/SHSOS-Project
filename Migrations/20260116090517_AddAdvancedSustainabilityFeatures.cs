using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SHSOS.Migrations
{
    /// <inheritdoc />
    public partial class AddAdvancedSustainabilityFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Alert",
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
                        principalTable: "Departments",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResourceThreshold",
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
                        principalTable: "Departments",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SustainabilityMetrics",
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
                        principalTable: "Departments",
                        principalColumn: "DepartmentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alert_DepartmentID",
                table: "Alert",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceThreshold_DepartmentID",
                table: "ResourceThreshold",
                column: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_SustainabilityMetrics_DepartmentID",
                table: "SustainabilityMetrics",
                column: "DepartmentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alert");

            migrationBuilder.DropTable(
                name: "ResourceThreshold");

            migrationBuilder.DropTable(
                name: "SustainabilityMetrics");
        }
    }
}
